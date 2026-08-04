using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Pooling;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.7 / 23.10 — track spawn planner + obstacle pool execution.
    /// Discovers <see cref="TrackSpawnMarker"/>s when segments activate, plans
    /// slots, and for Obstacle category immediately pool-Gets catalog prefabs
    /// at marker lanes. Coins / power-ups remain plan-only.
    /// Distinct from <c>Features.Multiplayer.Spawning.SpawnManager</c>.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SpawnManager : SceneSingleton<SpawnManager>
    {
        [Header("Profile")]
        [Tooltip("Map-specific spawn groups (swap Kuwait / Dubai / Doha / Muscat assets).")]
        [SerializeField] private SpawnProfile spawnProfile;

        [Header("Obstacle Gameplay (Sprint 23.10)")]
        [Tooltip("Prefab / data catalog for obstacle WarmPools + weighted pick.")]
        [SerializeField] private ObstacleCatalog obstacleCatalog;

        [Tooltip("Prepared session difficulty filter — no spacing/weight balancing yet.")]
        [SerializeField] private ObstacleDifficultyLevel obstacleDifficulty = ObstacleDifficultyLevel.Medium;

        [Tooltip(
            "When true, Obstacle plans are pool-executed on segment register. " +
            "Sprint 23.10 always executes for playability (RaceManager Running gate can come later).")]
        [SerializeField] private bool executeObstaclePlans = true;

        [Header("Wiring")]
        [Tooltip("Source of SegmentActivated / SegmentReleased. Auto-finds if unset.")]
        [SerializeField] private EndlessTrackGenerator trackGenerator;

        [Header("Planning")]
        [Tooltip("0 = time-seeded. Non-zero reproduces the same plan sequence.")]
        [SerializeField] private int randomSeed;

        [Tooltip("Optional verbose plan logs — leave off for mobile shipping builds.")]
        [SerializeField] private bool logPlans;

        private readonly List<PlannedSpawnSlot> _planned = new List<PlannedSpawnSlot>(64);
        private readonly Dictionary<SpawnCategory, float> _lastAcceptedZ = new Dictionary<SpawnCategory, float>(8);
        private readonly Dictionary<SpawnCategory, int> _plannedCounts = new Dictionary<SpawnCategory, int>(8);
        private readonly List<TrackSpawnMarker> _markerScratch = new List<TrackSpawnMarker>(32);
        private readonly Dictionary<int, List<GameObject>> _liveObstaclesBySegment =
            new Dictionary<int, List<GameObject>>(16);
        private readonly List<PlannedSpawnSlot> _executeScratch = new List<PlannedSpawnSlot>(16);

        private SeededRandom _rng;
        private int _rngSeedApplied = int.MinValue;
        private EndlessTrackGenerator _subscribedGenerator;
        private bool _poolsWarmed;

        public SpawnProfile Profile => spawnProfile;
        public ObstacleCatalog ObstacleCatalog => obstacleCatalog;
        public ObstacleDifficultyLevel ObstacleDifficulty => obstacleDifficulty;
        public bool ExecuteObstaclePlans => executeObstaclePlans;
        public IReadOnlyList<PlannedSpawnSlot> PlannedSlots => _planned;
        public IReadOnlyDictionary<SpawnCategory, int> PlannedCounts => _plannedCounts;
        public int PlannedCount => _planned.Count;

        protected override void Awake()
        {
            base.Awake();
            EnsureRng();
            ResetCategoryState();
        }

        private void OnEnable()
        {
            SubscribeGenerator(ResolveGenerator());
        }

        private void Start()
        {
            SubscribeGenerator(ResolveGenerator());
            WarmPools(transform);
        }

        private void OnDisable()
        {
            UnsubscribeGenerator();
            ReleaseAllLiveObstacles();
        }

        /// <summary>Swap map profile at runtime (future map catalog).</summary>
        public void SetProfile(SpawnProfile profile)
        {
            spawnProfile = profile;
            ClearAllPlans();
        }

        /// <summary>Assigns obstacle prefab catalog.</summary>
        public void SetObstacleCatalog(ObstacleCatalog catalog)
        {
            obstacleCatalog = catalog;
            _poolsWarmed = false;
        }

        /// <summary>Prepared difficulty tier (filter only — no balancing).</summary>
        public void SetObstacleDifficulty(ObstacleDifficultyLevel difficulty)
        {
            obstacleDifficulty = difficulty;
        }

        /// <summary>Clears planned slots, spacing cursors, and releases live obstacles.</summary>
        public void ClearAllPlans()
        {
            ReleaseAllLiveObstacles();
            _planned.Clear();
            ResetCategoryState();
        }

        /// <summary>
        /// Registers markers from an activated segment, plans groups, then executes
        /// Obstacle category via object pool (when enabled).
        /// </summary>
        public void RegisterSegment(TrackSegment segment)
        {
            if (segment == null || spawnProfile == null)
            {
                return;
            }

            // Idempotent: late subscribe + event can both see the same segment.
            UnregisterSegment(segment);

            EnsureRng();
            IReadOnlyList<TrackSpawnMarker> markers = segment.SpawnMarkers;
            if (markers == null || markers.Count == 0)
            {
                return;
            }

            int segmentId = segment.GetInstanceID();
            IReadOnlyList<SpawnGroupSettings> groups = spawnProfile.Groups;
            if (groups == null)
            {
                return;
            }

            for (int g = 0; g < groups.Count; g++)
            {
                SpawnGroupSettings settings = groups[g];
                if (settings == null || !settings.Enabled)
                {
                    continue;
                }

                CollectMarkers(_markerScratch, markers, settings.Category);
                if (_markerScratch.Count == 0)
                {
                    continue;
                }

                SortMarkersByWorldZ(_markerScratch);
                EvaluateGroup(settings, segmentId);
            }

            if (executeObstaclePlans)
            {
                ExecuteObstaclePlansForSegment(segmentId);
            }
        }

        /// <summary>Drops plans and releases pooled obstacles for a recycled segment.</summary>
        public void UnregisterSegment(TrackSegment segment)
        {
            if (segment == null)
            {
                return;
            }

            int segmentId = segment.GetInstanceID();
            ReleaseLiveObstacles(segmentId);

            if (_planned.Count == 0)
            {
                return;
            }

            for (int i = _planned.Count - 1; i >= 0; i--)
            {
                if (_planned[i].SegmentInstanceId != segmentId)
                {
                    continue;
                }

                PlannedSpawnSlot removed = _planned[i];
                _planned.RemoveAt(i);
                if (_plannedCounts.TryGetValue(removed.Category, out int count) && count > 0)
                {
                    _plannedCounts[removed.Category] = count - 1;
                }
            }
        }

        /// <summary>
        /// Copies planned slots for one category into <paramref name="buffer"/> (cleared first).
        /// </summary>
        public void CopyPlannedSlots(SpawnCategory category, List<PlannedSpawnSlot> buffer)
        {
            if (buffer == null)
            {
                return;
            }

            buffer.Clear();
            for (int i = 0; i < _planned.Count; i++)
            {
                if (_planned[i].Category == category)
                {
                    buffer.Add(_planned[i]);
                }
            }
        }

        /// <summary>Preloads obstacle catalog prefabs into <see cref="ObjectPoolManager"/>.</summary>
        public void WarmPools(Transform poolParent = null)
        {
            ObjectPoolManager pools = ObjectPoolManager.Instance;
            if (obstacleCatalog == null || pools == null)
            {
                return;
            }

            obstacleCatalog.WarmPools(pools, poolParent != null ? poolParent : transform);
            _poolsWarmed = true;
        }

        /// <summary>Resolves a prefab for <paramref name="data"/> from the obstacle catalog.</summary>
        public bool TryGetObstaclePrefab(ObstacleData data, out GameObject prefab)
        {
            prefab = null;
            return obstacleCatalog != null && obstacleCatalog.TryGetPrefab(data, out prefab);
        }

        /// <summary>
        /// Pool-Gets <paramref name="prefab"/> at the planned pose. Non-obstacle
        /// categories are not executed this sprint.
        /// </summary>
        public bool TryExecutePlannedSlot(in PlannedSpawnSlot slot, GameObject prefab, Transform parent = null)
        {
            if (prefab == null || slot.Category != SpawnCategory.Obstacle)
            {
                return false;
            }

            ObjectPoolManager pools = ObjectPoolManager.Instance;
            if (pools == null)
            {
                return false;
            }

            if (!_poolsWarmed)
            {
                WarmPools(transform);
            }

            Transform poolParent = parent != null ? parent : transform;
            GameObject instance = pools.Get(prefab, slot.WorldPosition, slot.WorldRotation, poolParent);
            if (instance == null)
            {
                return false;
            }

            Obstacle obstacle = instance.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                obstacle.ApplyPlannedSlot(slot, slot.Lane);
            }
            else
            {
                instance.transform.SetPositionAndRotation(slot.WorldPosition, slot.WorldRotation);
            }

            TrackLiveObstacle(slot.SegmentInstanceId, instance);
            return true;
        }

        /// <summary>
        /// Resolves catalog prefab for <paramref name="data"/> (or weighted pick when null),
        /// then pool-Gets at the planned lane pose.
        /// </summary>
        public bool TryExecuteObstacleSlot(
            in PlannedSpawnSlot slot,
            ObstacleData data,
            RunnerLane lane,
            Transform parent = null)
        {
            if (slot.Category != SpawnCategory.Obstacle)
            {
                return false;
            }

            GameObject prefab = null;
            ObstacleData resolvedData = data;

            if (resolvedData != null)
            {
                if (!TryGetObstaclePrefab(resolvedData, out prefab) || prefab == null)
                {
                    return false;
                }
            }
            else
            {
                if (obstacleCatalog == null ||
                    !obstacleCatalog.TryPickEntry(_rng, obstacleDifficulty, out ObstacleCatalogEntry entry) ||
                    entry == null ||
                    entry.Prefab == null)
                {
                    return false;
                }

                prefab = entry.Prefab;
                resolvedData = entry.Data;
            }

            PlannedSpawnSlot laneSlot = new PlannedSpawnSlot(
                slot.Category,
                slot.WorldPosition,
                slot.WorldRotation,
                slot.SegmentInstanceId,
                slot.MarkerInstanceId,
                lane);

            if (!TryExecutePlannedSlot(laneSlot, prefab, parent))
            {
                return false;
            }

            if (resolvedData != null &&
                _liveObstaclesBySegment.TryGetValue(slot.SegmentInstanceId, out List<GameObject> list) &&
                list != null &&
                list.Count > 0)
            {
                GameObject last = list[list.Count - 1];
                Obstacle obstacle = last != null ? last.GetComponent<Obstacle>() : null;
                if (obstacle != null && obstacle.Data != resolvedData)
                {
                    obstacle.BindData(resolvedData);
                }
            }

            return true;
        }

        private void ExecuteObstaclePlansForSegment(int segmentId)
        {
            if (obstacleCatalog == null)
            {
                return;
            }

            EnsureRng();
            _executeScratch.Clear();
            for (int i = 0; i < _planned.Count; i++)
            {
                PlannedSpawnSlot slot = _planned[i];
                if (slot.SegmentInstanceId == segmentId && slot.Category == SpawnCategory.Obstacle)
                {
                    _executeScratch.Add(slot);
                }
            }

            for (int i = 0; i < _executeScratch.Count; i++)
            {
                PlannedSpawnSlot slot = _executeScratch[i];
                if (!obstacleCatalog.TryPickEntry(_rng, obstacleDifficulty, out ObstacleCatalogEntry entry) ||
                    entry == null)
                {
                    continue;
                }

                TryExecuteObstacleSlot(slot, entry.Data, slot.Lane, transform);
            }
        }

        private void TrackLiveObstacle(int segmentId, GameObject instance)
        {
            if (!_liveObstaclesBySegment.TryGetValue(segmentId, out List<GameObject> list) || list == null)
            {
                list = new List<GameObject>(4);
                _liveObstaclesBySegment[segmentId] = list;
            }

            list.Add(instance);
        }

        private void ReleaseLiveObstacles(int segmentId)
        {
            if (!_liveObstaclesBySegment.TryGetValue(segmentId, out List<GameObject> list) || list == null)
            {
                return;
            }

            ObjectPoolManager pools = ObjectPoolManager.Instance;
            for (int i = 0; i < list.Count; i++)
            {
                GameObject instance = list[i];
                if (instance == null)
                {
                    continue;
                }

                if (pools != null)
                {
                    pools.Release(instance);
                }
                else
                {
                    instance.SetActive(false);
                }
            }

            list.Clear();
            _liveObstaclesBySegment.Remove(segmentId);
        }

        private void ReleaseAllLiveObstacles()
        {
            if (_liveObstaclesBySegment.Count == 0)
            {
                return;
            }

            List<int> keys = new List<int>(_liveObstaclesBySegment.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                ReleaseLiveObstacles(keys[i]);
            }
        }

        private void EvaluateGroup(SpawnGroupSettings settings, int segmentId)
        {
            SpawnCategory category = settings.Category;
            if (!_lastAcceptedZ.TryGetValue(category, out float lastZ))
            {
                lastZ = float.NegativeInfinity;
                _lastAcceptedZ[category] = lastZ;
            }

            float probability = settings.SpawnProbability;
            float density = settings.SpawnDensity;
            float minSpacing = settings.MinimumSpacing;
            float maxSpacing = Mathf.Max(minSpacing, settings.MaximumSpacing);

            for (int i = 0; i < _markerScratch.Count; i++)
            {
                TrackSpawnMarker marker = _markerScratch[i];
                if (marker == null)
                {
                    continue;
                }

                if (density < 1f && NextFloat() > density)
                {
                    continue;
                }

                Transform markerTransform = marker.transform;
                float z = markerTransform.position.z;
                float gap = z - lastZ;

                if (gap < minSpacing && !float.IsNegativeInfinity(lastZ))
                {
                    continue;
                }

                bool forceByMaxGap = !float.IsNegativeInfinity(lastZ) && gap > maxSpacing;
                if (!forceByMaxGap && NextFloat() > probability)
                {
                    continue;
                }

                RunnerLane lane = marker.ResolveLane();
                PlannedSpawnSlot slot = new PlannedSpawnSlot(
                    category,
                    markerTransform.position,
                    markerTransform.rotation,
                    segmentId,
                    marker.GetInstanceID(),
                    lane);

                _planned.Add(slot);
                lastZ = z;
                _lastAcceptedZ[category] = lastZ;

                if (!_plannedCounts.TryGetValue(category, out int count))
                {
                    count = 0;
                }

                _plannedCounts[category] = count + 1;

                if (logPlans)
                {
                    Debug.Log(
                        $"SpawnManager plan {category} lane={lane} @ {slot.WorldPosition} (profile={spawnProfile.ProfileId})",
                        this);
                }
            }
        }

        private static void CollectMarkers(
            List<TrackSpawnMarker> buffer,
            IReadOnlyList<TrackSpawnMarker> markers,
            SpawnCategory category)
        {
            buffer.Clear();
            for (int i = 0; i < markers.Count; i++)
            {
                TrackSpawnMarker marker = markers[i];
                if (marker != null && marker.Category == category)
                {
                    buffer.Add(marker);
                }
            }
        }

        private static void SortMarkersByWorldZ(List<TrackSpawnMarker> markers)
        {
            for (int i = 1; i < markers.Count; i++)
            {
                TrackSpawnMarker key = markers[i];
                if (key == null)
                {
                    continue;
                }

                float keyZ = key.transform.position.z;
                int j = i - 1;
                while (j >= 0)
                {
                    TrackSpawnMarker other = markers[j];
                    float otherZ = other != null ? other.transform.position.z : float.NegativeInfinity;
                    if (otherZ <= keyZ)
                    {
                        break;
                    }

                    markers[j + 1] = markers[j];
                    j--;
                }

                markers[j + 1] = key;
            }
        }

        private void HandleSegmentActivated(TrackSegment segment) => RegisterSegment(segment);

        private void HandleSegmentReleased(TrackSegment segment) => UnregisterSegment(segment);

        private EndlessTrackGenerator ResolveGenerator()
        {
            if (trackGenerator != null)
            {
                return trackGenerator;
            }

            return FindObjectOfType<EndlessTrackGenerator>();
        }

        private void SubscribeGenerator(EndlessTrackGenerator generator)
        {
            if (generator == null || generator == _subscribedGenerator)
            {
                return;
            }

            UnsubscribeGenerator();
            ClearAllPlans();
            _subscribedGenerator = generator;
            trackGenerator = generator;
            _subscribedGenerator.SegmentActivated += HandleSegmentActivated;
            _subscribedGenerator.SegmentReleased += HandleSegmentReleased;
            _subscribedGenerator.ForEachActiveSegment(RegisterSegment);
        }

        private void UnsubscribeGenerator()
        {
            if (_subscribedGenerator == null)
            {
                return;
            }

            _subscribedGenerator.SegmentActivated -= HandleSegmentActivated;
            _subscribedGenerator.SegmentReleased -= HandleSegmentReleased;
            _subscribedGenerator = null;
        }

        private void EnsureRng()
        {
            if (_rng != null && _rngSeedApplied == randomSeed)
            {
                return;
            }

            _rngSeedApplied = randomSeed;
            int seed = randomSeed != 0 ? randomSeed : Environment.TickCount;
            _rng = new SeededRandom(seed);
        }

        private float NextFloat()
        {
            EnsureRng();
            return _rng.NextFloat01();
        }

        private void ResetCategoryState()
        {
            _lastAcceptedZ.Clear();
            _plannedCounts.Clear();

            _lastAcceptedZ[SpawnCategory.Obstacle] = float.NegativeInfinity;
            _lastAcceptedZ[SpawnCategory.Coin] = float.NegativeInfinity;
            _lastAcceptedZ[SpawnCategory.Gem] = float.NegativeInfinity;
            _lastAcceptedZ[SpawnCategory.PowerUp] = float.NegativeInfinity;
            _lastAcceptedZ[SpawnCategory.Decoration] = float.NegativeInfinity;
            _lastAcceptedZ[SpawnCategory.Npc] = float.NegativeInfinity;

            _plannedCounts[SpawnCategory.Obstacle] = 0;
            _plannedCounts[SpawnCategory.Coin] = 0;
            _plannedCounts[SpawnCategory.Gem] = 0;
            _plannedCounts[SpawnCategory.PowerUp] = 0;
            _plannedCounts[SpawnCategory.Decoration] = 0;
            _plannedCounts[SpawnCategory.Npc] = 0;
        }
    }
}

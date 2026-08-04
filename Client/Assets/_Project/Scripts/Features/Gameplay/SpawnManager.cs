using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Pooling;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.7 / 23.10 / 23.12 — track spawn planner + obstacle / collectible
    /// pool execution. Discovers <see cref="TrackSpawnMarker"/>s when segments
    /// activate, plans slots, and pool-Gets Obstacle / Coin / Gem catalog prefabs.
    /// Distinct from <c>Features.Multiplayer.Spawning.SpawnManager</c>.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SpawnManager : SceneSingleton<SpawnManager>
    {
        private const float DefaultLaneSpacing = 2.2f;

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

        [Header("Collectibles (Sprint 23.12)")]
        [Tooltip("Coin / Gem prefab catalog for WarmPools + spawn.")]
        [SerializeField] private CollectibleCatalog collectibleCatalog;

        [Tooltip("When true, Coin / Gem plans are pool-executed on segment register.")]
        [SerializeField] private bool executeCollectiblePlans = true;

        [Tooltip("Default layout when randomizeCollectiblePattern is off.")]
        [SerializeField] private CollectiblePattern defaultCoinPattern = CollectiblePattern.Line;

        [Tooltip("When true, Coin slots pick Single / Line / Arc at random.")]
        [SerializeField] private bool randomizeCoinPattern = true;

        [Tooltip("Gems always use Single unless this is enabled (then same picker as coins).")]
        [SerializeField] private bool allowGemPatterns;

        [SerializeField] private int lineCount = 5;
        [SerializeField] private float lineSpacingZ = 1.4f;
        [SerializeField] private float arcHeight = 0.55f;
        [SerializeField] private float laneSpacing = DefaultLaneSpacing;
        [SerializeField] private float laneCenterX;

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
        private readonly Dictionary<int, List<GameObject>> _liveCollectiblesBySegment =
            new Dictionary<int, List<GameObject>>(16);
        private readonly List<PlannedSpawnSlot> _executeScratch = new List<PlannedSpawnSlot>(16);

        private SeededRandom _rng;
        private int _rngSeedApplied = int.MinValue;
        private EndlessTrackGenerator _subscribedGenerator;
        private bool _poolsWarmed;

        public SpawnProfile Profile => spawnProfile;
        public ObstacleCatalog ObstacleCatalog => obstacleCatalog;
        public CollectibleCatalog CollectibleCatalog => collectibleCatalog;
        public ObstacleDifficultyLevel ObstacleDifficulty => obstacleDifficulty;
        public bool ExecuteObstaclePlans => executeObstaclePlans;
        public bool ExecuteCollectiblePlans => executeCollectiblePlans;
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
            ReleaseAllLiveCollectibles();
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

        /// <summary>Assigns collectible prefab catalog.</summary>
        public void SetCollectibleCatalog(CollectibleCatalog catalog)
        {
            collectibleCatalog = catalog;
            _poolsWarmed = false;
        }

        /// <summary>Prepared difficulty tier (filter only — no balancing).</summary>
        public void SetObstacleDifficulty(ObstacleDifficultyLevel difficulty)
        {
            obstacleDifficulty = difficulty;
        }

        /// <summary>Clears planned slots, spacing cursors, and releases live spawns.</summary>
        public void ClearAllPlans()
        {
            ReleaseAllLiveObstacles();
            ReleaseAllLiveCollectibles();
            _planned.Clear();
            ResetCategoryState();
        }

        /// <summary>
        /// Registers markers from an activated segment, plans groups, then executes
        /// Obstacle / Coin / Gem categories via object pool (when enabled).
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

            if (executeCollectiblePlans)
            {
                ExecuteCollectiblePlansForSegment(segmentId, SpawnCategory.Coin);
                ExecuteCollectiblePlansForSegment(segmentId, SpawnCategory.Gem);
            }
        }

        /// <summary>Drops plans and releases pooled spawns for a recycled segment.</summary>
        public void UnregisterSegment(TrackSegment segment)
        {
            if (segment == null)
            {
                return;
            }

            int segmentId = segment.GetInstanceID();
            ReleaseLiveObstacles(segmentId);
            ReleaseLiveCollectibles(segmentId);

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

        /// <summary>Preloads obstacle + collectible catalog prefabs into <see cref="ObjectPoolManager"/>.</summary>
        public void WarmPools(Transform poolParent = null)
        {
            ObjectPoolManager pools = ObjectPoolManager.Instance;
            if (pools == null)
            {
                return;
            }

            Transform parent = poolParent != null ? poolParent : transform;
            if (obstacleCatalog != null)
            {
                obstacleCatalog.WarmPools(pools, parent);
            }

            if (collectibleCatalog != null)
            {
                collectibleCatalog.WarmPools(pools, parent);
            }

            _poolsWarmed = true;
        }

        /// <summary>Resolves a prefab for <paramref name="data"/> from the obstacle catalog.</summary>
        public bool TryGetObstaclePrefab(ObstacleData data, out GameObject prefab)
        {
            prefab = null;
            return obstacleCatalog != null && obstacleCatalog.TryGetPrefab(data, out prefab);
        }

        /// <summary>Resolves a Coin / Gem prefab from the collectible catalog.</summary>
        public bool TryGetCollectiblePrefab(CollectibleType type, out GameObject prefab)
        {
            prefab = null;
            return collectibleCatalog != null && collectibleCatalog.TryGetPrefab(type, out prefab);
        }

        /// <summary>
        /// Pool-Gets <paramref name="prefab"/> at the planned pose for Obstacle category.
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

        /// <summary>
        /// Pool-spawns Coin / Gem at the planned marker using <paramref name="pattern"/>.
        /// </summary>
        public bool TryExecuteCollectibleSlot(
            in PlannedSpawnSlot slot,
            CollectiblePattern pattern,
            Transform parent = null)
        {
            if (slot.Category != SpawnCategory.Coin && slot.Category != SpawnCategory.Gem)
            {
                return false;
            }

            CollectibleType type = slot.Category == SpawnCategory.Gem
                ? CollectibleType.Gem
                : CollectibleType.Coin;

            if (!TryGetCollectiblePrefab(type, out GameObject prefab) || prefab == null)
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
            CollectiblePattern resolved = pattern;
            if (type == CollectibleType.Gem && !allowGemPatterns)
            {
                resolved = CollectiblePattern.Single;
            }

            int spawned = 0;
            switch (resolved)
            {
                case CollectiblePattern.Line:
                    spawned = SpawnCollectibleLine(slot, prefab, poolParent);
                    break;
                case CollectiblePattern.Arc:
                    spawned = SpawnCollectibleArc(slot, prefab, poolParent);
                    break;
                default:
                    spawned = SpawnCollectibleAt(slot, prefab, slot.WorldPosition, slot.Lane, poolParent) ? 1 : 0;
                    break;
            }

            return spawned > 0;
        }

        private int SpawnCollectibleLine(in PlannedSpawnSlot slot, GameObject prefab, Transform parent)
        {
            int count = Mathf.Max(1, lineCount);
            float spacing = Mathf.Max(0.25f, lineSpacingZ);
            int spawned = 0;
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = slot.WorldPosition;
                pos.z += i * spacing;
                if (SpawnCollectibleAt(slot, prefab, pos, slot.Lane, parent))
                {
                    spawned++;
                }
            }

            return spawned;
        }

        private int SpawnCollectibleArc(in PlannedSpawnSlot slot, GameObject prefab, Transform parent)
        {
            float spacing = laneSpacing > 0.1f ? laneSpacing : DefaultLaneSpacing;
            float height = Mathf.Max(0f, arcHeight);
            RunnerLane[] lanes = { RunnerLane.Left, RunnerLane.Center, RunnerLane.Right };
            int spawned = 0;
            for (int i = 0; i < lanes.Length; i++)
            {
                RunnerLane lane = lanes[i];
                Vector3 pos = slot.WorldPosition;
                pos.x = RunnerLaneMath.LaneX(lane, laneCenterX, spacing);
                // Parabola peaking at center: y = base + height * (1 - ((i-1)^2))
                float t = i - 1;
                pos.y = slot.WorldPosition.y + (height * (1f - (t * t)));
                if (SpawnCollectibleAt(slot, prefab, pos, lane, parent))
                {
                    spawned++;
                }
            }

            return spawned;
        }

        private bool SpawnCollectibleAt(
            in PlannedSpawnSlot slot,
            GameObject prefab,
            Vector3 worldPosition,
            RunnerLane lane,
            Transform parent)
        {
            ObjectPoolManager pools = ObjectPoolManager.Instance;
            if (pools == null)
            {
                return false;
            }

            GameObject instance = pools.Get(prefab, worldPosition, slot.WorldRotation, parent);
            if (instance == null)
            {
                return false;
            }

            Collectible collectible = instance.GetComponent<Collectible>();
            if (collectible != null)
            {
                collectible.ApplyWorldPose(worldPosition, slot.WorldRotation, lane);
            }
            else
            {
                instance.transform.SetPositionAndRotation(worldPosition, slot.WorldRotation);
            }

            TrackLiveCollectible(slot.SegmentInstanceId, instance);
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

        private void ExecuteCollectiblePlansForSegment(int segmentId, SpawnCategory category)
        {
            if (collectibleCatalog == null)
            {
                return;
            }

            EnsureRng();
            _executeScratch.Clear();
            for (int i = 0; i < _planned.Count; i++)
            {
                PlannedSpawnSlot slot = _planned[i];
                if (slot.SegmentInstanceId == segmentId && slot.Category == category)
                {
                    _executeScratch.Add(slot);
                }
            }

            for (int i = 0; i < _executeScratch.Count; i++)
            {
                PlannedSpawnSlot slot = _executeScratch[i];
                CollectiblePattern pattern = ResolvePattern(category);
                TryExecuteCollectibleSlot(slot, pattern, transform);
            }
        }

        private CollectiblePattern ResolvePattern(SpawnCategory category)
        {
            if (category == SpawnCategory.Gem && !allowGemPatterns)
            {
                return CollectiblePattern.Single;
            }

            if (!randomizeCoinPattern)
            {
                return defaultCoinPattern;
            }

            EnsureRng();
            float roll = _rng.NextFloat01();
            if (roll < 0.25f)
            {
                return CollectiblePattern.Single;
            }

            if (roll < 0.7f)
            {
                return CollectiblePattern.Line;
            }

            return CollectiblePattern.Arc;
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

        private void TrackLiveCollectible(int segmentId, GameObject instance)
        {
            if (!_liveCollectiblesBySegment.TryGetValue(segmentId, out List<GameObject> list) || list == null)
            {
                list = new List<GameObject>(8);
                _liveCollectiblesBySegment[segmentId] = list;
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

        private void ReleaseLiveCollectibles(int segmentId)
        {
            if (!_liveCollectiblesBySegment.TryGetValue(segmentId, out List<GameObject> list) || list == null)
            {
                return;
            }

            ObjectPoolManager pools = ObjectPoolManager.Instance;
            for (int i = 0; i < list.Count; i++)
            {
                GameObject instance = list[i];
                if (instance == null || !instance.activeInHierarchy)
                {
                    // Already collected + released to pool (inactive).
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
            _liveCollectiblesBySegment.Remove(segmentId);
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

        private void ReleaseAllLiveCollectibles()
        {
            if (_liveCollectiblesBySegment.Count == 0)
            {
                return;
            }

            List<int> keys = new List<int>(_liveCollectiblesBySegment.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                ReleaseLiveCollectibles(keys[i]);
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

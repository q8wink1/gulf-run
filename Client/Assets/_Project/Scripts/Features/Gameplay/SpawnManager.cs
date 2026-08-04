using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Pooling;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.7 — centralized track spawn planner. Discovers
    /// <see cref="TrackSpawnMarker"/>s when segments activate, evaluates each
    /// category group independently (probability / density / spacing), and
    /// stores <see cref="PlannedSpawnSlot"/>s only. Does not Instantiate or
    /// pool gameplay content yet; designed so a future sprint can call
    /// <see cref="ObjectPoolManager"/> at planned poses.
    /// Sprint 23.9 adds <see cref="ObstacleCatalog"/> WarmPools / execute stubs
    /// without placing obstacles.
    /// Distinct from <c>Features.Multiplayer.Spawning.SpawnManager</c> (player slots).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SpawnManager : SceneSingleton<SpawnManager>
    {
        [Header("Profile")]
        [Tooltip("Map-specific spawn groups (swap Kuwait / Dubai / Doha / Muscat assets).")]
        [SerializeField] private SpawnProfile spawnProfile;

        [Header("Obstacle Foundation (Sprint 23.9)")]
        [Tooltip("Prefab / data catalog for obstacle WarmPools hooks. Does not spawn.")]
        [SerializeField] private ObstacleCatalog obstacleCatalog;

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

        private System.Random _rng;
        private int _rngSeedApplied = int.MinValue;
        private EndlessTrackGenerator _subscribedGenerator;

        public SpawnProfile Profile => spawnProfile;
        public ObstacleCatalog ObstacleCatalog => obstacleCatalog;
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
        }

        private void OnDisable()
        {
            UnsubscribeGenerator();
        }

        /// <summary>Swap map profile at runtime (future map catalog).</summary>
        public void SetProfile(SpawnProfile profile)
        {
            spawnProfile = profile;
            ClearAllPlans();
        }

        /// <summary>Assigns obstacle prefab catalog without executing spawns.</summary>
        public void SetObstacleCatalog(ObstacleCatalog catalog)
        {
            obstacleCatalog = catalog;
        }

        /// <summary>Clears every planned slot and spacing cursors.</summary>
        public void ClearAllPlans()
        {
            _planned.Clear();
            ResetCategoryState();
        }

        /// <summary>
        /// Registers markers from an activated segment and dry-runs category groups.
        /// Safe to call from <see cref="EndlessTrackGenerator"/> events.
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
        }

        /// <summary>Drops planned slots that belonged to a recycled segment.</summary>
        public void UnregisterSegment(TrackSegment segment)
        {
            if (segment == null || _planned.Count == 0)
            {
                return;
            }

            int segmentId = segment.GetInstanceID();
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
        /// Zero allocation beyond the caller's list growth.
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

        /// <summary>
        /// Future pool warm-up. Preloads obstacle catalog prefabs when assigned;
        /// still does not place any content on the track.
        /// </summary>
        public void WarmPools(Transform poolParent = null)
        {
            ObjectPoolManager pools = ObjectPoolManager.Instance;
            if (obstacleCatalog != null && pools != null)
            {
                obstacleCatalog.WarmPools(pools, poolParent);
            }
        }

        /// <summary>Resolves a prefab for <paramref name="data"/> from the obstacle catalog.</summary>
        public bool TryGetObstaclePrefab(ObstacleData data, out GameObject prefab)
        {
            prefab = null;
            return obstacleCatalog != null && obstacleCatalog.TryGetPrefab(data, out prefab);
        }

        /// <summary>
        /// Future: spawn pooled content at a planned slot. Always returns false this sprint.
        /// </summary>
        public bool TryExecutePlannedSlot(in PlannedSpawnSlot slot, GameObject prefab, Transform parent = null)
        {
            _ = slot;
            _ = prefab;
            _ = parent;
            return false;
        }

        /// <summary>
        /// Future obstacle execute path: resolve catalog prefab then pool-Get at the plan.
        /// Always returns false this sprint (no random / no Instantiate).
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

            if (!TryGetObstaclePrefab(data, out GameObject prefab) || prefab == null)
            {
                return false;
            }

            // Prefab + lane reserved for a future pool Get + IObstaclePlacementTarget.ApplyPlannedSlot.
            _ = lane;
            _ = parent;
            _ = prefab;
            return false;
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

                PlannedSpawnSlot slot = new PlannedSpawnSlot(
                    category,
                    markerTransform.position,
                    markerTransform.rotation,
                    segmentId,
                    marker.GetInstanceID());

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
                        $"SpawnManager plan {category} @ {slot.WorldPosition} (profile={spawnProfile.ProfileId})",
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
            _rng = new System.Random(seed);
        }

        private float NextFloat()
        {
            EnsureRng();
            return (float)_rng.NextDouble();
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

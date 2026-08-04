using System;
using System.Collections.Generic;
using GulfRun.Core.Pooling;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.6 — endless +Z track: pools modular <see cref="TrackSegment"/>
    /// prefabs from a <see cref="TrackSegmentSet"/>, spawns ahead of the runner,
    /// and returns far-behind segments to the pool. Selection alternates for now;
    /// <see cref="TrackSegmentSet.TrySelectWeighted"/> is ready for future random maps.
    /// Sprint 23.7 — raises <see cref="SegmentActivated"/> / <see cref="SegmentReleased"/>
    /// so <see cref="SpawnManager"/> can plan marker slots without Instantiating content.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EndlessTrackGenerator : MonoBehaviour
    {
        [Header("Track Tuning")]
        [Tooltip("Design length for segments in this map set (prefab Length should match).")]
        [SerializeField] private float segmentLength = 40f;

        [Tooltip("Target number of live segments (also drives preload + initial fill).")]
        [SerializeField] private int activeSegments = 6;

        [Tooltip("Keep the track frontier at least this far ahead of the player.")]
        [SerializeField] private float spawnDistance = 80f;

        [Tooltip("Recycle a segment once its exit is this far behind the player.")]
        [SerializeField] private float despawnDistance = 40f;

        [Header("Data")]
        [SerializeField] private TrackSegmentSet segmentSet;

        [Header("Runtime")]
        [SerializeField] private Transform followTarget;
        [SerializeField] private Transform segmentParent;
        [SerializeField] private int preloadPerPrefab = 3;

        private readonly Queue<TrackSegment> _active = new Queue<TrackSegment>();
        private float _frontierZ;
        private int _alternateIndex;
        private bool _warnedMissingPool;
        private bool _warnedMissingSet;

        /// <summary>Fired after a segment is placed and enqueued (Sprint 23.7 spawn planning).</summary>
        public event Action<TrackSegment> SegmentActivated;

        /// <summary>Fired just before a segment returns to the pool (Sprint 23.7 cleanup).</summary>
        public event Action<TrackSegment> SegmentReleased;

        public float SegmentLength => segmentLength;
        public int ActiveSegments => activeSegments;
        public float SpawnDistance => spawnDistance;
        public float DespawnDistance => despawnDistance;
        public int ActiveCount => _active.Count;
        public float FrontierZ => _frontierZ;

        /// <summary>
        /// Invokes <paramref name="action"/> for each live segment (queue order).
        /// Used by <see cref="SpawnManager"/> to catch up after late subscribe.
        /// </summary>
        public void ForEachActiveSegment(Action<TrackSegment> action)
        {
            if (action == null)
            {
                return;
            }

            foreach (TrackSegment segment in _active)
            {
                if (segment != null)
                {
                    action(segment);
                }
            }
        }

        private void Awake()
        {
            if (segmentParent == null)
            {
                segmentParent = transform;
            }

            if (followTarget == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    followTarget = player.transform;
                }
            }
        }

        private void Start()
        {
            PreloadPools();
            FillInitialBuffer();
        }

        private void Update()
        {
            float playerZ = followTarget != null ? followTarget.position.z : 0f;
            SpawnAhead(playerZ);
            DespawnBehind(playerZ);
        }

        /// <summary>Releases every active segment and rebuilds the initial buffer.</summary>
        public void ResetTrack()
        {
            while (_active.Count > 0)
            {
                ReleaseSegment(_active.Dequeue());
            }

            _frontierZ = 0f;
            _alternateIndex = 0;
            FillInitialBuffer();
        }

        private void PreloadPools()
        {
            ObjectPoolManager pool = ObjectPoolManager.Instance;
            if (pool == null || segmentSet == null)
            {
                return;
            }

            int count = Mathf.Max(1, preloadPerPrefab);
            IReadOnlyList<TrackSegmentEntry> entries = segmentSet.Segments;
            for (int i = 0; i < entries.Count; i++)
            {
                GameObject prefab = entries[i] != null ? entries[i].Prefab : null;
                if (prefab != null)
                {
                    pool.Preload(prefab, count, segmentParent);
                }
            }
        }

        private void FillInitialBuffer()
        {
            int target = Mathf.Max(1, activeSegments);
            int guard = 0;
            while (_active.Count < target && guard < target + 8)
            {
                if (!SpawnNextSegment())
                {
                    break;
                }

                guard++;
            }

            // Also honour spawnDistance from z=0 so the first stretch is covered.
            SpawnAhead(0f);
        }

        private void SpawnAhead(float playerZ)
        {
            float needFrontier = playerZ + Mathf.Max(0f, spawnDistance);
            int guard = 0;
            while (_frontierZ < needFrontier && guard < 32)
            {
                if (!SpawnNextSegment())
                {
                    break;
                }

                guard++;
            }
        }

        private void DespawnBehind(float playerZ)
        {
            float recycleBefore = playerZ - Mathf.Max(0f, despawnDistance);
            while (_active.Count > 0)
            {
                TrackSegment oldest = _active.Peek();
                if (oldest == null)
                {
                    _active.Dequeue();
                    continue;
                }

                if (oldest.EndZ < recycleBefore)
                {
                    _active.Dequeue();
                    ReleaseSegment(oldest);
                }
                else
                {
                    break;
                }
            }
        }

        private bool SpawnNextSegment()
        {
            GameObject prefab = SelectNextPrefab();
            if (prefab == null)
            {
                if (!_warnedMissingSet)
                {
                    Debug.LogWarning("EndlessTrackGenerator: TrackSegmentSet has no prefabs.", this);
                    _warnedMissingSet = true;
                }

                return false;
            }

            ObjectPoolManager pool = ObjectPoolManager.Instance;
            if (pool == null)
            {
                if (!_warnedMissingPool)
                {
                    Debug.LogError("EndlessTrackGenerator: ObjectPoolManager missing — cannot pool segments.", this);
                    _warnedMissingPool = true;
                }

                return false;
            }

            Vector3 position = new Vector3(0f, 0f, _frontierZ);
            GameObject instance = pool.Get(prefab, position, Quaternion.identity, segmentParent);
            if (instance == null)
            {
                return false;
            }

            TrackSegment segment = instance.GetComponent<TrackSegment>();
            if (segment == null)
            {
                Debug.LogError($"Track prefab '{prefab.name}' is missing TrackSegment.", instance);
                pool.Release(instance);
                return false;
            }

            // Prefab Length is authoritative; Inspector Segment Length is the design target.
            float length = segment.Length;
            segment.PlaceAtStartZ(_frontierZ);
            _frontierZ += length;
            _active.Enqueue(segment);
            SegmentActivated?.Invoke(segment);
            return true;
        }

        private void ReleaseSegment(TrackSegment segment)
        {
            if (segment == null)
            {
                return;
            }

            SegmentReleased?.Invoke(segment);

            ObjectPoolManager pool = ObjectPoolManager.Instance;
            if (pool != null)
            {
                pool.Release(segment.gameObject);
            }
            else
            {
                segment.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Alternates through the set for now. Swap to
        /// <see cref="TrackSegmentSet.TrySelectWeighted"/> when map randomisation lands.
        /// </summary>
        private GameObject SelectNextPrefab()
        {
            if (segmentSet == null || segmentSet.Count == 0)
            {
                return null;
            }

            int count = segmentSet.Count;
            for (int attempt = 0; attempt < count; attempt++)
            {
                int index = _alternateIndex % count;
                _alternateIndex++;
                GameObject prefab = segmentSet.GetPrefab(index);
                if (prefab != null)
                {
                    return prefab;
                }
            }

            return null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            segmentLength = Mathf.Max(1f, segmentLength);
            activeSegments = Mathf.Max(1, activeSegments);
            spawnDistance = Mathf.Max(0f, spawnDistance);
            despawnDistance = Mathf.Max(0f, despawnDistance);
            preloadPerPrefab = Mathf.Max(1, preloadPerPrefab);
        }
#endif
    }
}

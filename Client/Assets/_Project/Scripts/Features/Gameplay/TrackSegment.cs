using System.Collections.Generic;
using GulfRun.Core.Pooling;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.6 — modular endless-track piece (+Z). Root marks the entry /
    /// start edge; content spans forward for <see cref="Length"/>. Exit is at
    /// local Z = Length so segments connect seamlessly. Spawn markers are
    /// placeholders only (no content spawning yet). Sprint 23.9 adds obstacle
    /// marker query hooks for future placement without spawning.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TrackSegment : MonoBehaviour, IPoolable
    {
        [SerializeField] private float length = 40f;
        [SerializeField] private Transform entryPoint;
        [SerializeField] private Transform exitPoint;
        [SerializeField] private TrackSpawnMarker[] spawnMarkers;

        public float Length => length > 0.01f ? length : 0.01f;
        public Transform EntryPoint => entryPoint != null ? entryPoint : transform;
        public Transform ExitPoint => exitPoint != null ? exitPoint : transform;
        public IReadOnlyList<TrackSpawnMarker> SpawnMarkers => spawnMarkers;

        /// <summary>World Z of the segment start (entry).</summary>
        public float StartZ => transform.position.z;

        /// <summary>World Z of the segment end (exit).</summary>
        public float EndZ => transform.position.z + Length;

        public void OnSpawned()
        {
        }

        public void OnDespawned()
        {
        }

        /// <summary>Places this segment so its entry sits at <paramref name="worldStartZ"/>.</summary>
        public void PlaceAtStartZ(float worldStartZ)
        {
            Vector3 p = transform.position;
            p.z = worldStartZ;
            transform.position = p;
            SyncEndpoints();
        }

        /// <summary>
        /// Sprint 23.9 — copies obstacle-category markers into <paramref name="buffer"/>
        /// (cleared first). Hook for future obstacle placement; does not spawn.
        /// </summary>
        public void CopyObstacleMarkers(List<TrackSpawnMarker> buffer)
        {
            if (buffer == null)
            {
                return;
            }

            buffer.Clear();
            if (spawnMarkers == null)
            {
                return;
            }

            for (int i = 0; i < spawnMarkers.Length; i++)
            {
                TrackSpawnMarker marker = spawnMarkers[i];
                if (marker != null && marker.Category == SpawnCategory.Obstacle)
                {
                    buffer.Add(marker);
                }
            }
        }

        private void SyncEndpoints()
        {
            if (entryPoint != null)
            {
                entryPoint.localPosition = Vector3.zero;
            }

            if (exitPoint != null)
            {
                exitPoint.localPosition = new Vector3(0f, 0f, Length);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (spawnMarkers == null || spawnMarkers.Length == 0)
            {
                spawnMarkers = GetComponentsInChildren<TrackSpawnMarker>(includeInactive: true);
            }

            SyncEndpoints();
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 start = transform.TransformPoint(Vector3.zero);
            Vector3 end = transform.TransformPoint(new Vector3(0f, 0f, Length));
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(start, end);
            Gizmos.DrawWireCube(start, new Vector3(0.4f, 0.4f, 0.4f));
            Gizmos.DrawWireCube(end, new Vector3(0.4f, 0.4f, 0.4f));
        }
#endif
    }
}

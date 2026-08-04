using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.7 — dry-run spawn plan for one marker. Holds transform data only;
    /// future sprints may <c>ObjectPoolManager.Get</c> at this pose. Never owns a
    /// live gameplay instance.
    /// </summary>
    public readonly struct PlannedSpawnSlot
    {
        public readonly SpawnCategory Category;
        public readonly Vector3 WorldPosition;
        public readonly Quaternion WorldRotation;
        public readonly int SegmentInstanceId;
        public readonly int MarkerInstanceId;

        public PlannedSpawnSlot(
            SpawnCategory category,
            Vector3 worldPosition,
            Quaternion worldRotation,
            int segmentInstanceId,
            int markerInstanceId)
        {
            Category = category;
            WorldPosition = worldPosition;
            WorldRotation = worldRotation;
            SegmentInstanceId = segmentInstanceId;
            MarkerInstanceId = markerInstanceId;
        }
    }
}

using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.7 / 23.10 / 23.12 — spawn plan for one marker. Holds transform + lane;
    /// SpawnManager may <c>ObjectPoolManager.Get</c> at this pose for obstacles
    /// and collectibles. Never owns a live gameplay instance.
    /// </summary>
    public readonly struct PlannedSpawnSlot
    {
        public readonly SpawnCategory Category;
        public readonly Vector3 WorldPosition;
        public readonly Quaternion WorldRotation;
        public readonly int SegmentInstanceId;
        public readonly int MarkerInstanceId;
        public readonly RunnerLane Lane;

        public PlannedSpawnSlot(
            SpawnCategory category,
            Vector3 worldPosition,
            Quaternion worldRotation,
            int segmentInstanceId,
            int markerInstanceId,
            RunnerLane lane = RunnerLane.Center)
        {
            Category = category;
            WorldPosition = worldPosition;
            WorldRotation = worldRotation;
            SegmentInstanceId = segmentInstanceId;
            MarkerInstanceId = markerInstanceId;
            Lane = lane;
        }
    }
}

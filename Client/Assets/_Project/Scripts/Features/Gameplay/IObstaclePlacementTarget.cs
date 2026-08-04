using GulfRun.Domain;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.9 — contract so <see cref="SpawnManager"/> / track planners can
    /// configure an obstacle at a planned pose without owning spawn execution.
    /// </summary>
    public interface IObstaclePlacementTarget
    {
        ObstacleType Type { get; }
        RunnerLane Lane { get; }
        ObstacleData Data { get; }
        bool IsObstacleEnabled { get; }

        /// <summary>Applies world pose + lane from a dry-run plan (no Instantiate).</summary>
        void ApplyPlannedSlot(in PlannedSpawnSlot slot, RunnerLane lane);

        void SetObstacleEnabled(bool enabled);
    }
}

using GulfRun.Core.Pooling;
using GulfRun.Domain;
using GulfRun.Features.EndlessRunner.Difficulty;
using GulfRun.Features.EndlessRunner.Distance;
using GulfRun.Features.EndlessRunner.GameLoop;
using GulfRun.Features.EndlessRunner.Scoring;
using GulfRun.Features.EndlessRunner.Speed;
using GulfRun.Features.EndlessRunner.Spawning;
using GulfRun.Features.EndlessRunner.WorldGeneration;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner
{
    /// <summary>
    /// Editor/development-build-only on-screen readout of the endless-runner
    /// session: game loop state, current chunk, current speed, distance,
    /// spawn statistics per category, and object-pool usage. Reads other
    /// systems only through their public scene-scoped Instance accessors —
    /// contains no gameplay logic of its own.
    ///
    /// Lives inside this feature (not the shared GulfRun.Debug assembly)
    /// because GulfRun.Debug is foundational tooling referenced BY features,
    /// so it must never reference a specific feature back — the same
    /// convention Sprint 2's PlayerDebugView established.
    /// </summary>
    public sealed class RunnerDebugView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private bool showOnScreenDebug = true;

        private void OnGUI()
        {
            if (!showOnScreenDebug)
            {
                return;
            }

            int y = 90;
            const int lineHeight = 18;
            const int width = 420;

            void Line(string text)
            {
                GUI.Label(new Rect(10, y, width, lineHeight), text);
                y += lineHeight;
            }

            Line($"[Runner] State: {(GameLoopController.Instance != null ? GameLoopController.Instance.State.ToString() : "n/a")}");

            if (GameLoopController.Instance != null
                && GameLoopController.Instance.State == GameLoopState.Countdown
                && CountdownController.Instance != null)
            {
                Line($"Countdown: {CountdownController.Instance.DisplayText}");
            }

            Line($"Distance: {(DistanceTracker.Instance != null ? DistanceTracker.Instance.DistanceMeters : 0):F1} m");
            Line($"Speed: {(GameSpeedController.Instance != null ? GameSpeedController.Instance.CurrentSpeed : 0f):F2} m/s");
            Line($"Difficulty: {(DifficultyController.Instance != null ? DifficultyController.Instance.Current01 : 0f):F2}");

            if (ScoreController.Instance != null)
            {
                Line($"Score: {ScoreController.Instance.TotalScore:F0} (distance {ScoreController.Instance.DistanceScore:F0} + coins {ScoreController.Instance.CoinScore:F0} x{ScoreController.Instance.Multiplier:F1})");
            }

            if (WorldGenerator.Instance != null)
            {
                string latest = WorldGenerator.Instance.LatestChunk != null ? WorldGenerator.Instance.LatestChunk.name : "none";
                Line($"Active Chunks: {WorldGenerator.Instance.ActiveChunkCount} (latest: {latest})");
            }

            if (ChunkContentSpawner.Instance != null)
            {
                foreach (var entry in ChunkContentSpawner.Instance.SpawnCounts)
                {
                    Line($"Spawned {entry.Key}: {entry.Value}");
                }
            }

            if (ObjectPoolManager.Instance != null)
            {
                foreach (PoolStats stat in ObjectPoolManager.Instance.GetAllStats())
                {
                    Line($"Pool [{stat.PoolName}]: {stat.Active} active / {stat.Inactive} idle");
                }
            }
        }
#endif
    }
}

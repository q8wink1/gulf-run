namespace GulfRun.Domain
{
    /// <summary>
    /// Pure "difficulty progression" math shared by <c>TrapAuthority</c>:
    /// how often traps spawn and how many may be active at once both scale
    /// with the same normalized 0..1 difficulty value the endless-runner's
    /// <c>DifficultyController</c> already computes from distance
    /// (<c>Core.Services.IDifficultyProvider</c>) — reusing that one signal
    /// instead of a second, parallel difficulty curve.
    /// </summary>
    public static class TrapDifficulty
    {
        /// <summary>Interval shrinks from <paramref name="maxIntervalAtZeroDifficulty"/> (race start) toward <paramref name="minIntervalAtMaxDifficulty"/> (fully ramped) as difficulty rises — traps get more frequent, never less.</summary>
        public static float ResolveSpawnIntervalSeconds(float minIntervalAtMaxDifficulty, float maxIntervalAtZeroDifficulty, float difficulty01)
        {
            float t = Clamp01(difficulty01);
            return maxIntervalAtZeroDifficulty + (minIntervalAtMaxDifficulty - maxIntervalAtZeroDifficulty) * t;
        }

        /// <summary>Concurrent trap cap grows from <paramref name="baseMaxConcurrent"/> toward <paramref name="baseMaxConcurrent"/> + <paramref name="extraAtMaxDifficulty"/> as difficulty rises.</summary>
        public static int ResolveMaxConcurrent(int baseMaxConcurrent, int extraAtMaxDifficulty, float difficulty01)
        {
            float t = Clamp01(difficulty01);
            return baseMaxConcurrent + (int)System.Math.Round(extraAtMaxDifficulty * t);
        }

        private static float Clamp01(float value) => value < 0f ? 0f : value > 1f ? 1f : value;
    }
}

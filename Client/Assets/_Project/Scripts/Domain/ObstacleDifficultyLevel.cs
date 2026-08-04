namespace GulfRun.Domain
{
    /// <summary>
    /// Sprint 23.10 — prepared obstacle session difficulty tiers.
    /// Filtering hooks exist; spawn balancing is intentionally deferred.
    /// </summary>
    public enum ObstacleDifficultyLevel
    {
        Easy = 0,
        Medium = 1,
        Hard = 2
    }

    /// <summary>Maps session difficulty to the max <see cref="ObstacleData"/> difficulty int allowed.</summary>
    public static class ObstacleDifficultyLevelRules
    {
        /// <summary>
        /// Easy → data difficulty ≤ 2, Medium ≤ 3, Hard ≤ 5.
        /// Prepared only — weights / spacing are not retuned yet.
        /// </summary>
        public static int MaxObstacleDataDifficulty(ObstacleDifficultyLevel level)
        {
            switch (level)
            {
                case ObstacleDifficultyLevel.Easy:
                    return 2;
                case ObstacleDifficultyLevel.Medium:
                    return 3;
                case ObstacleDifficultyLevel.Hard:
                    return 5;
                default:
                    return 5;
            }
        }
    }
}

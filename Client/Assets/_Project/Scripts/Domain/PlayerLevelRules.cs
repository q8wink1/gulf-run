namespace GulfRun.Domain
{
    /// <summary>Current Level / Current XP / XP Required For Next Level — the exact "Level Display" field set <c>Design/GDD/P024-LEVEL-SYSTEM-v1.0.md</c> §5 requires.</summary>
    public readonly struct PlayerLevelProgress
    {
        public int Level { get; }
        public int CurrentXp { get; }
        public int XpRequiredForNextLevel { get; }

        public PlayerLevelProgress(int level, int currentXp, int xpRequiredForNextLevel)
        {
            Level = level;
            CurrentXp = currentXp;
            XpRequiredForNextLevel = xpRequiredForNextLevel;
        }

        public float Progress01 => XpRequiredForNextLevel > 0 ? (float)CurrentXp / XpRequiredForNextLevel : 0f;
    }

    /// <summary>
    /// Pure Player Level rules for P024 (Level System): "start at Level 1",
    /// "gain XP", "level up when enough XP is earned", "unused XP carries
    /// over", "cannot lose Player Levels" (LVL-001..004, LVL-PRG-001..003) —
    /// implemented against total lifetime XP so those last two are true by
    /// construction (a monotonically non-decreasing input can only ever
    /// resolve to the same or a higher level). P024 §10 explicitly leaves
    /// the XP Formula/Sources/Maximum Level "Not defined"; this uses one
    /// honest placeholder formula (XP per completed match) documented here
    /// so Level Display has a real, live number instead of an invented
    /// static value, pending a real backend-authoritative formula (P024 §7:
    /// "Level and XP data are synchronized with the backend").
    /// </summary>
    public static class PlayerLevelRules
    {
        /// <summary>Placeholder XP Source (P024 §10: "XP Sources — Not defined") — one completed match, win or lose.</summary>
        public const int XpPerMatch = 50;

        private const int BaseXpForLevel2 = 200;
        private const int XpGrowthPerLevel = 50;

        public static int XpRequiredForLevel(int level)
        {
            int safeLevel = level < 1 ? 1 : level;
            return BaseXpForLevel2 + (safeLevel - 1) * XpGrowthPerLevel;
        }

        public static PlayerLevelProgress ResolveFromMatchesPlayed(int matchesPlayed)
        {
            int totalXp = matchesPlayed > 0 ? matchesPlayed * XpPerMatch : 0;

            int level = 1;
            int remaining = totalXp;
            int required = XpRequiredForLevel(level);
            while (remaining >= required)
            {
                remaining -= required;
                level++;
                required = XpRequiredForLevel(level);
            }

            return new PlayerLevelProgress(level, remaining, required);
        }
    }
}

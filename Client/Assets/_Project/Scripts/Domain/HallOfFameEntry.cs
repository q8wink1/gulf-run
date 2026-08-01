namespace GulfRun.Domain
{
    /// <summary>
    /// One permanent Hall of Fame record. "A player's achievement remains
    /// permanently recorded even after losing Rank #1" (Sprint 9 brief) is
    /// enforced entirely by how these are produced/stored, not by anything
    /// in this struct itself: <c>Core.Backend.LocalOnlineBackendService</c>
    /// only ever appends new entries (e.g. once per season/week/month
    /// rollover) — it never deletes or overwrites one just because the
    /// underlying leaderboard has since moved on.
    /// </summary>
    public readonly struct HallOfFameEntry
    {
        public readonly HallOfFameCategory Category;

        /// <summary>Only meaningful when <see cref="Category"/> is <see cref="HallOfFameCategory.BestInCountry"/>; null otherwise.</summary>
        public readonly GulfCountry? Country;

        public readonly PlayerId Player;
        public readonly string Nickname;
        public readonly int Score;

        /// <summary>Human-readable "when this was achieved", e.g. "Season 3", "Week 12 2026".</summary>
        public readonly string AchievedLabel;

        public HallOfFameEntry(HallOfFameCategory category, GulfCountry? country, PlayerId player, string nickname, int score, string achievedLabel)
        {
            Category = category;
            Country = country;
            Player = player;
            Nickname = nickname;
            Score = score;
            AchievedLabel = achievedLabel;
        }
    }
}

namespace GulfRun.Domain
{
    /// <summary>
    /// A player's whole competitive-season state in one synchronizable
    /// value: which numbered season, which <see cref="League"/> tier, and
    /// the trophy count within it. The brief lists "Season Progress" as
    /// its own item under Networking — this is the exact payload that
    /// implies.
    /// </summary>
    public readonly struct SeasonProgress
    {
        public readonly int SeasonNumber;
        public readonly League CurrentLeague;
        public readonly int TrophyCount;

        public SeasonProgress(int seasonNumber, League currentLeague, int trophyCount)
        {
            SeasonNumber = seasonNumber;
            CurrentLeague = currentLeague;
            TrophyCount = trophyCount;
        }

        public static SeasonProgress Initial(int seasonNumber) => new SeasonProgress(seasonNumber, League.Bronze, 0);
    }
}

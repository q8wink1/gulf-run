namespace GulfRun.Domain
{
    /// <summary>
    /// One ranked row on any <see cref="RankingScope"/> leaderboard —
    /// the "Top 1 / Top 10 / Top 100" rows the brief requires, and also
    /// the row shape used to resolve any single player's rank.
    /// </summary>
    public readonly struct LeaderboardEntry
    {
        public readonly int Rank;
        public readonly PlayerId Player;
        public readonly string Nickname;
        public readonly GulfCountry Country;
        public readonly int TrophyCount;
        public readonly int Wins;

        public LeaderboardEntry(int rank, PlayerId player, string nickname, GulfCountry country, int trophyCount, int wins)
        {
            Rank = rank;
            Player = player;
            Nickname = nickname;
            Country = country;
            TrophyCount = trophyCount;
            Wins = wins;
        }
    }
}

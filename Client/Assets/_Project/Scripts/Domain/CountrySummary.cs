namespace GulfRun.Domain
{
    /// <summary>
    /// The per-country aggregate the brief's Country Rankings section
    /// requires alongside the Top 1/10/100 rows: "Total Players, Wins,
    /// Trophies" for one <see cref="GulfCountry"/>'s whole registered
    /// population, not just its top entries.
    /// </summary>
    public readonly struct CountrySummary
    {
        public readonly GulfCountry Country;
        public readonly int TotalPlayers;
        public readonly int TotalWins;
        public readonly int TotalTrophies;

        public CountrySummary(GulfCountry country, int totalPlayers, int totalWins, int totalTrophies)
        {
            Country = country;
            TotalPlayers = totalPlayers;
            TotalWins = totalWins;
            TotalTrophies = totalTrophies;
        }
    }
}

namespace GulfRun.Domain
{
    /// <summary>The permanent record categories from the Sprint 9 Hall of Fame brief.</summary>
    public enum HallOfFameCategory
    {
        BestInWorld,
        BestInGulf,

        /// <summary>Meaningful only combined with a <see cref="GulfCountry"/> — see <see cref="HallOfFameEntry.Country"/>.</summary>
        BestInCountry,
        WeeklyChampion,
        MonthlyChampion,
        SeasonChampion,
        TournamentChampion
    }
}

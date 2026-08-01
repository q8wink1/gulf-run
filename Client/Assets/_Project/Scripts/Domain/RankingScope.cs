namespace GulfRun.Domain
{
    /// <summary>
    /// The six leaderboards the Sprint 9 brief requires. "Gulf" and
    /// "Country" both filter the same underlying player pool by
    /// <see cref="GulfCountry"/> (Country = exactly one nation; Gulf = any
    /// of the 8 launch nations) while "World" applies no country filter at
    /// all — see <c>Core.Backend.LocalOnlineBackendService</c>'s remarks on
    /// why World and Gulf happen to produce identical results today.
    /// Weekly/Monthly/Seasonal instead filter by *time window* over the
    /// same trophy pool rather than by geography.
    /// </summary>
    public enum RankingScope
    {
        World,
        Gulf,
        Country,
        Weekly,
        Monthly,
        Seasonal
    }
}

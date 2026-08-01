namespace GulfRun.Domain
{
    /// <summary>
    /// Groups the "Country Events" the brief lists (8 National Days,
    /// Ramadan/Eid Championships, Summer/Winter Events, and "future
    /// regional tournaments") so new entries never need a new enum member —
    /// only a new <c>Features.Online.Configuration.CountryEventCatalogConfig</c>
    /// row tagged with one of these existing categories.
    /// </summary>
    public enum CountryEventCategory
    {
        NationalDay,
        ReligiousEvent,
        SeasonalEvent,
        RegionalTournament
    }
}

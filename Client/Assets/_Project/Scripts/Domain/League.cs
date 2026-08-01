namespace GulfRun.Domain
{
    /// <summary>
    /// The 8 competitive tiers from the Sprint 9 brief, ordered lowest to
    /// highest. Declaration order doubles as tier order — see
    /// <see cref="LeagueRules.ResolveLeague"/>, which resolves a trophy
    /// count against a same-ordered threshold list rather than hardcoding
    /// each tier's cutoff in code.
    /// </summary>
    public enum League
    {
        Bronze,
        Silver,
        Gold,
        Platinum,
        Diamond,
        Master,
        GrandMaster,
        Legend
    }
}

namespace GulfRun.Domain
{
    /// <summary>
    /// Pure Gem-unlock math for Premium Cosmetics ("Unlock using Gems" —
    /// Sprint 8 brief). Deliberately trivial today (a single affordability
    /// check) but kept as its own pure Domain type — exactly like
    /// <see cref="RaceElimination"/>/<see cref="RaceRanking"/> in Sprint 7 —
    /// so a future backend-validated purchase flow (P012/P039) re-runs the
    /// identical rule server-side with zero duplicated logic.
    /// </summary>
    public static class CosmeticUnlockRules
    {
        /// <summary>Traditional Outfits are always free (price 0) and are never gated by this check — see COS-EQ/COS-OWN rules in P022.</summary>
        public static bool CanAfford(int currentGems, int priceGems) => priceGems <= 0 || currentGems >= priceGems;
    }
}

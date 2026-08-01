namespace GulfRun.Domain
{
    /// <summary>
    /// Cosmetic rarity tiers for the Locker (Sprint 16). Visual-only —
    /// never affects gameplay stats (COS / CHR-005). Ordering is intentional
    /// so <see cref="LockerSortMode.Rarity"/> can sort by enum ordinal.
    /// </summary>
    public enum CosmeticRarity
    {
        Common = 0,
        Rare = 1,
        Epic = 2,
        Legendary = 3,
        Mythic = 4
    }
}

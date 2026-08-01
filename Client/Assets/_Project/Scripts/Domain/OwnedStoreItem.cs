namespace GulfRun.Domain
{
    /// <summary>
    /// One permanently-owned Store product that has no home in an existing
    /// per-feature inventory yet — Visual Effects and Profile Frames have no
    /// <see cref="CosmeticSlot"/> of their own today (see Sprint 10 report
    /// Remaining TODOs), so ownership of those lives in this generic ledger
    /// (<c>IStoreBackendService</c>) instead. Outfits/Emotes/Victory Poses
    /// purchased through the Store are granted into the real
    /// <see cref="CosmeticInventory"/> via <c>ICosmeticGrantService</c> and
    /// do NOT also appear here — this ledger is exclusively for content
    /// types the rest of the game doesn't have a slot for yet.
    /// </summary>
    public readonly struct OwnedStoreItem
    {
        public readonly StoreItemId ItemId;
        public readonly StoreSection Section;
        public readonly double AcquiredAtSeconds;

        public OwnedStoreItem(StoreItemId itemId, StoreSection section, double acquiredAtSeconds)
        {
            ItemId = itemId;
            Section = section;
            AcquiredAtSeconds = acquiredAtSeconds;
        }
    }
}

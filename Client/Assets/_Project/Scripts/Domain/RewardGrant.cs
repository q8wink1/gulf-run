namespace GulfRun.Domain
{
    /// <summary>
    /// One concrete reward payout. <see cref="Amount"/> is only meaningful
    /// for <see cref="RewardType.Coins"/>/<see cref="RewardType.Gems"/>;
    /// <see cref="Cosmetic"/> is only meaningful for the cosmetic-shaped
    /// types (Skin/Outfit/VictoryPose/LimitedCosmetic), reusing Sprint 8's
    /// <see cref="CosmeticId"/> rather than inventing a parallel identifier
    /// system for the same kind of content. Titles/Badges/Profile
    /// Frames/Champion Effects have no gameplay asset yet, so
    /// <see cref="DisplayName"/> alone carries them for now (see Sprint 9
    /// report Remaining TODOs).
    /// </summary>
    public readonly struct RewardGrant
    {
        public readonly RewardType Type;
        public readonly int Amount;
        public readonly CosmeticId Cosmetic;
        public readonly string DisplayName;

        public RewardGrant(RewardType type, int amount, CosmeticId cosmetic, string displayName)
        {
            Type = type;
            Amount = amount;
            Cosmetic = cosmetic;
            DisplayName = displayName;
        }
    }
}

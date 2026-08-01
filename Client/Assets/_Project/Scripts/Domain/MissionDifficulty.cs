namespace GulfRun.Domain
{
    /// <summary>Daily Mission difficulty band — "Reward scales automatically" (Sprint 11 brief) by multiplying a pool entry's base reward by a difficulty-indexed, catalog-configured multiplier (see <c>MissionPoolCatalogConfig.GetRewardMultiplier</c>). Never serialized by ordinal in a way that would break if reordered (only <see cref="Domain.RewardType"/> has that constraint in this project), but kept append-only regardless for consistency.</summary>
    public enum MissionDifficulty
    {
        Easy,
        Medium,
        Hard
    }
}

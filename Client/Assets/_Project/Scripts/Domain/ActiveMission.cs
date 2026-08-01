namespace GulfRun.Domain
{
    /// <summary>
    /// One of the local player's 3 currently-active Daily Missions. A
    /// mutable class (matching <see cref="BattlePassStatus"/>'s style) so
    /// <c>Core.Backend.IProgressionBackendService</c> can own the single
    /// authoritative instance and mutate its progress/claimed state
    /// in-place, exactly the "backend records, feature manager only reads
    /// and applies the reward effect" split
    /// <c>Features.Store.BattlePass.BattlePassManager</c> established for
    /// Battle Pass tiers.
    /// <para>
    /// Every field is snapshotted from the chosen
    /// <c>MissionPoolCatalogConfig.MissionPoolEntry</c> at generation time
    /// (including the already-difficulty-scaled <see cref="RewardAmount"/>) —
    /// editing the pool later never retroactively changes an in-progress
    /// mission, the same "snapshot the entry" behavior
    /// <see cref="PurchaseTransaction"/> already uses for its price string.
    /// </para>
    /// </summary>
    public sealed class ActiveMission
    {
        public MissionId SourceMissionId { get; }
        public string DisplayName { get; }
        public MissionType Type { get; }
        public MissionDifficulty Difficulty { get; }
        public int TargetAmount { get; }
        public RewardType RewardType { get; }
        public int RewardAmount { get; }
        public CosmeticId RewardCosmeticId { get; }
        public bool IsTemporaryCosmeticReward { get; }
        public TemporaryCosmeticDuration RewardDuration { get; }

        /// <summary>Coins granted instead, if a temporary-cosmetic reward would be a duplicate of an already-permanently-owned cosmetic (brief: "Never reward temporary duplicate. Instead reward: ... Alternative reward").</summary>
        public int FallbackCoinsAmount { get; }

        public int CurrentAmount { get; private set; }

        public bool IsClaimed { get; set; }

        public bool IsCompleted => CurrentAmount >= TargetAmount;

        public ActiveMission(MissionId sourceMissionId, string displayName, MissionType type, MissionDifficulty difficulty, int targetAmount, RewardType rewardType, int rewardAmount, CosmeticId rewardCosmeticId, bool isTemporaryCosmeticReward, TemporaryCosmeticDuration rewardDuration, int fallbackCoinsAmount)
        {
            SourceMissionId = sourceMissionId;
            DisplayName = displayName;
            Type = type;
            Difficulty = difficulty;
            TargetAmount = targetAmount > 0 ? targetAmount : 1;
            RewardType = rewardType;
            RewardAmount = rewardAmount;
            RewardCosmeticId = rewardCosmeticId;
            IsTemporaryCosmeticReward = isTemporaryCosmeticReward;
            RewardDuration = rewardDuration;
            FallbackCoinsAmount = fallbackCoinsAmount;
            CurrentAmount = 0;
            IsClaimed = false;
        }

        /// <summary>Adds progress, clamped to <see cref="TargetAmount"/>. A no-op once claimed (a claimed mission's slot is about to be replaced by the next daily generation anyway).</summary>
        public void AddProgress(int amount)
        {
            if (amount <= 0 || IsClaimed)
            {
                return;
            }

            int updated = CurrentAmount + amount;
            CurrentAmount = updated > TargetAmount ? TargetAmount : updated;
        }
    }
}

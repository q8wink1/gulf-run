using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Progression.Configuration
{
    /// <summary>
    /// The full Daily Missions pool — brief: "Mission pool must be
    /// configurable. Random missions every day." Every entry's target
    /// amount, difficulty, and BASE reward is authored data;
    /// <c>Missions.MissionManager</c> picks 3 distinct entries at random
    /// each reset and scales the reward by <see cref="GetRewardMultiplier"/>
    /// (brief: "Reward scales automatically") — no price/amount is
    /// hardcoded in code.
    /// </summary>
    [CreateAssetMenu(fileName = "MissionPoolCatalogConfig", menuName = "GulfRun/Progression/Mission Pool Catalog Config")]
    public sealed class MissionPoolCatalogConfig : ScriptableObject
    {
        [Serializable]
        public sealed class MissionPoolEntry
        {
            [SerializeField] private string id;
            [SerializeField] private string displayName = string.Empty;
            [SerializeField] private MissionType type;
            [SerializeField] private MissionDifficulty difficulty;
            [SerializeField] private int targetAmount = 1;

            [Header("Base Reward (scaled by the catalog's per-difficulty multiplier for Coins/Gems/BattlePassXp)")]
            [SerializeField] private RewardType rewardType;
            [SerializeField] private int rewardAmount;

            [Tooltip("Only meaningful for ExclusiveOutfit/ExclusiveEmote/VictoryPose/ExclusiveSkin/LimitedCosmetic reward types.")]
            [SerializeField] private string rewardCosmeticId;

            [Tooltip("If true, rewardCosmeticId is granted as a TEMPORARY (expiring) cosmetic rather than a permanent one — brief 'TEMPORARY COSMETICS'.")]
            [SerializeField] private bool isTemporaryCosmeticReward;

            [SerializeField] private TemporaryCosmeticDuration temporaryDuration = TemporaryCosmeticDuration.TwoDays;

            [Tooltip("Coins granted instead if a temporary-cosmetic reward would duplicate an already-permanently-owned cosmetic (brief: 'Never reward temporary duplicate. Instead reward: Coins/Small Gems/Alternative reward').")]
            [SerializeField] private int fallbackCoinsAmount = 50;

            public MissionId Id => new MissionId(id);
            public string DisplayName => string.IsNullOrEmpty(displayName) ? id : displayName;
            public MissionType Type => type;
            public MissionDifficulty Difficulty => difficulty;
            public int TargetAmount => targetAmount;
            public RewardType RewardType => rewardType;
            public int RewardAmount => rewardAmount;
            public CosmeticId RewardCosmeticId => new CosmeticId(rewardCosmeticId);
            public bool IsTemporaryCosmeticReward => isTemporaryCosmeticReward;
            public TemporaryCosmeticDuration TemporaryDuration => temporaryDuration;
            public int FallbackCoinsAmount => fallbackCoinsAmount;
        }

        [Header("Reward Scaling (brief: 'Reward scales automatically')")]
        [SerializeField] private float easyRewardMultiplier = 1f;
        [SerializeField] private float mediumRewardMultiplier = 1.5f;
        [SerializeField] private float hardRewardMultiplier = 2f;

        [SerializeField] private List<MissionPoolEntry> missions = new List<MissionPoolEntry>();

        public IReadOnlyList<MissionPoolEntry> Missions => missions;

        public float GetRewardMultiplier(MissionDifficulty difficulty)
        {
            switch (difficulty)
            {
                case MissionDifficulty.Easy: return easyRewardMultiplier;
                case MissionDifficulty.Medium: return mediumRewardMultiplier;
                case MissionDifficulty.Hard: return hardRewardMultiplier;
                default: return 1f;
            }
        }
    }
}

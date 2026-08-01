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

            public static MissionPoolEntry Create(string id, string displayName, MissionType type, MissionDifficulty difficulty, int targetAmount, RewardType rewardType, int rewardAmount, int fallbackCoinsAmount = 50)
            {
                return new MissionPoolEntry
                {
                    id = id,
                    displayName = displayName,
                    type = type,
                    difficulty = difficulty,
                    targetAmount = targetAmount,
                    rewardType = rewardType,
                    rewardAmount = rewardAmount,
                    rewardCosmeticId = string.Empty,
                    isTemporaryCosmeticReward = false,
                    temporaryDuration = TemporaryCosmeticDuration.TwoDays,
                    fallbackCoinsAmount = fallbackCoinsAmount
                };
            }
        }

        [Header("Reward Scaling (brief: 'Reward scales automatically')")]
        [SerializeField] private float easyRewardMultiplier = 1f;
        [SerializeField] private float mediumRewardMultiplier = 1.5f;
        [SerializeField] private float hardRewardMultiplier = 2f;

        [SerializeField] private List<MissionPoolEntry> missions = new List<MissionPoolEntry>();

        public IReadOnlyList<MissionPoolEntry> Missions => missions ?? (IReadOnlyList<MissionPoolEntry>)Array.Empty<MissionPoolEntry>();

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

        /// <summary>
        /// Runtime fallback when Boot wiring or catalog content is missing —
        /// keeps Daily Mission generation from crashing the Lobby UI.
        /// </summary>
        public static MissionPoolCatalogConfig CreateDefault()
        {
            var catalog = CreateInstance<MissionPoolCatalogConfig>();
            catalog.name = "MissionPoolCatalogConfig_RuntimeDefault";
            catalog.easyRewardMultiplier = 1f;
            catalog.mediumRewardMultiplier = 1.5f;
            catalog.hardRewardMultiplier = 2f;
            catalog.missions = new List<MissionPoolEntry>
            {
                MissionPoolEntry.Create("mission_finish_races_easy", "Finish 2 Races", MissionType.FinishRaces, MissionDifficulty.Easy, 2, RewardType.Coins, 100),
                MissionPoolEntry.Create("mission_collect_coins_easy", "Collect 50 Coins", MissionType.CollectCoins, MissionDifficulty.Easy, 50, RewardType.Coins, 50),
                MissionPoolEntry.Create("mission_login_easy", "Login Today", MissionType.LoginToday, MissionDifficulty.Easy, 1, RewardType.Coins, 50),
                MissionPoolEntry.Create("mission_jump_easy", "Jump 15 Times", MissionType.PerformJumps, MissionDifficulty.Easy, 15, RewardType.Coins, 60),
                MissionPoolEntry.Create("mission_open_boxes_easy", "Open 3 Item Boxes", MissionType.OpenItemBoxes, MissionDifficulty.Easy, 3, RewardType.Coins, 80),
                MissionPoolEntry.Create("mission_use_weapons_easy", "Use 5 Weapons", MissionType.UseWeapons, MissionDifficulty.Easy, 5, RewardType.Coins, 80)
            };
            return catalog;
        }
    }
}

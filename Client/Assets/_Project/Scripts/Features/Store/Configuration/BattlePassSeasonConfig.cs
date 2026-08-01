using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Store.Configuration
{
    /// <summary>
    /// The active Premium Monthly Battle Pass season — "Paid only ... Every
    /// month includes: Exclusive Outfits, Exclusive Emotes, Exclusive
    /// Victory Poses, Exclusive Profile Frames, Exclusive Effects, Coins,
    /// Gems, Titles" (Sprint 10 brief). Every tier's XP requirement and
    /// reward is authored data — swapping in Season 2 later is authoring a
    /// new asset and pointing <c>BattlePassManager</c> at it, never a code
    /// change. Reuses Sprint 9's <see cref="RewardType"/>/<see cref="RewardGrant"/>
    /// shapes rather than inventing a parallel reward vocabulary, the same
    /// "generalize, don't duplicate" call this project made when
    /// <c>ChampionshipManager</c> first consumed those types.
    /// </summary>
    [CreateAssetMenu(fileName = "BattlePassSeasonConfig", menuName = "GulfRun/Store/Battle Pass Season Config")]
    public sealed class BattlePassSeasonConfig : ScriptableObject
    {
        [Serializable]
        public sealed class BattlePassTierEntry
        {
            [SerializeField] private int tier;

            [Tooltip("Cumulative Battle Pass XP required to reach this tier.")]
            [SerializeField] private int xpRequired;

            [SerializeField] private RewardType rewardType;

            [Tooltip("Coins/Gems amount when rewardType is Coins/Gems; ignored otherwise.")]
            [SerializeField] private int rewardAmount;

            [Tooltip("Only meaningful for ExclusiveOutfit/ExclusiveEmote/VictoryPose: the CosmeticId a claim grants into the real CosmeticInventory via ICosmeticGrantService.")]
            [SerializeField] private string rewardCosmeticId;

            [SerializeField] private string rewardDisplayName = string.Empty;

            public int Tier => tier;
            public int XpRequired => xpRequired;
            public RewardType RewardType => rewardType;
            public int RewardAmount => rewardAmount;
            public CosmeticId RewardCosmeticId => new CosmeticId(rewardCosmeticId);
            public string RewardDisplayName => string.IsNullOrEmpty(rewardDisplayName) ? rewardType.ToString() : rewardDisplayName;
        }

        [SerializeField] private int seasonNumber = 1;
        [SerializeField] private string seasonDisplayName = string.Empty;
        [SerializeField, TextArea] private string description = string.Empty;
        [SerializeField] private string premiumPriceCurrencyCode = "USD";
        [SerializeField] private float premiumPriceAmount = 9.99f;

        [Tooltip("Battle Pass XP granted per completed match (see PlayerStatEventService.LocalMatchCompleted) — the same 'Playing matches' progression source the Coins system already uses.")]
        [SerializeField] private int xpPerMatch = 50;

        [Tooltip("Extra XP granted for a race win, on top of xpPerMatch.")]
        [SerializeField] private int bonusXpPerWin = 50;

        [SerializeField] private List<BattlePassTierEntry> tiers = new List<BattlePassTierEntry>();

        public int SeasonNumber => seasonNumber;
        public string SeasonDisplayName => string.IsNullOrEmpty(seasonDisplayName) ? "Season " + seasonNumber : seasonDisplayName;
        public string Description => description;
        public RealMoneyPrice PremiumPrice => new RealMoneyPrice(premiumPriceCurrencyCode, premiumPriceAmount);
        public int XpPerMatch => xpPerMatch;
        public int BonusXpPerWin => bonusXpPerWin;
        public IReadOnlyList<BattlePassTierEntry> Tiers => tiers;

        public int TotalTierCount => tiers.Count;

        /// <summary>Highest tier whose <see cref="BattlePassTierEntry.XpRequired"/> is at-or-below <paramref name="xp"/>, or 0 if none reached yet.</summary>
        public int ResolveTierForXp(int xp)
        {
            int resolved = 0;
            for (int i = 0; i < tiers.Count; i++)
            {
                if (tiers[i] != null && xp >= tiers[i].XpRequired && tiers[i].Tier > resolved)
                {
                    resolved = tiers[i].Tier;
                }
            }

            return resolved;
        }

        public BattlePassTierEntry GetTier(int tierNumber)
        {
            for (int i = 0; i < tiers.Count; i++)
            {
                if (tiers[i] != null && tiers[i].Tier == tierNumber)
                {
                    return tiers[i];
                }
            }

            return null;
        }
    }
}

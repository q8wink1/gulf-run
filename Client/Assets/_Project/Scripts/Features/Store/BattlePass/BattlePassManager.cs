using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Backend;
using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Store.Configuration;
using UnityEngine;

namespace GulfRun.Features.Store.BattlePass
{
    /// <summary>
    /// Owns the Premium Monthly Battle Pass lifecycle: purchasing the
    /// premium unlock (real money), earning XP from
    /// <see cref="PlayerStatEventService.LocalMatchCompleted"/> (the same
    /// "Playing matches" progression source Coins already use — Sprint 7's
    /// <c>RaceRewardApplier</c>), and claiming a reached tier's reward.
    /// Kept separate from <see cref="StoreManager"/> for the same
    /// single-responsibility reason Sprint 9 split
    /// <c>LeagueManager</c>/<c>ChampionshipManager</c> into two managers
    /// instead of one.
    /// <para>
    /// Sprint 11 addition: implements <see cref="IBattlePassXpGrantService"/>
    /// so <c>Features.Progression</c> (Daily Mission / Login Reward "Battle
    /// Pass XP" rewards) can add XP here with zero compile-time reference
    /// to this (Features.Store) assembly — the same "implement the Core
    /// interface" shape this class already uses for <see cref="ICosmeticGrantService"/>
    /// via <c>PlayerLoadoutManager</c>.
    /// </para>
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BattlePassManager : Singleton<BattlePassManager>, IBattlePassXpGrantService, IEventBannerSource
    {
        [SerializeField] private BattlePassSeasonConfig season;

        public BattlePassSeasonConfig Season => season;

        public BattlePassStatus Status => StoreBackendService.Current.GetBattlePassStatus();

        protected override void OnInitialize()
        {
            EventBannerRegistry.Register(this);
        }

        private void OnEnable()
        {
            PlayerStatEventService.LocalMatchCompleted += HandleLocalMatchCompleted;
            BattlePassXpGrantService.Current = this;
        }

        private void OnDisable()
        {
            PlayerStatEventService.LocalMatchCompleted -= HandleLocalMatchCompleted;

            if (BattlePassXpGrantService.Current == (IBattlePassXpGrantService)this)
            {
                BattlePassXpGrantService.Current = null;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventBannerRegistry.Unregister(this);
        }

        /// <summary>Sprint 13 (Main Menu Event Banner): the current season, promoting the Premium upgrade while it is not yet unlocked.</summary>
        public IReadOnlyList<string> GetActiveBannerMessages()
        {
            if (season == null)
            {
                return System.Array.Empty<string>();
            }

            return Status.IsPremiumUnlocked
                ? new[] { season.SeasonDisplayName + " Battle Pass — Tier " + CurrentTier() }
                : new[] { season.SeasonDisplayName + " Battle Pass is live — unlock Premium now!" };
        }

        /// <summary>IBattlePassXpGrantService — see class remarks.</summary>
        void IBattlePassXpGrantService.AddXp(int amount) => StoreBackendService.Current.AddBattlePassXp(amount);

        /// <summary>Real-money purchase of the "Paid only" premium track (brief: "Premium Monthly Battle Pass. Paid only.").</summary>
        public PurchaseResult PurchasePremium()
        {
            if (season == null)
            {
                return PurchaseResult.ValidationFailed;
            }

            if (Status.IsPremiumUnlocked)
            {
                return PurchaseResult.AlreadyOwned;
            }

            PurchaseTransaction transaction = StoreBackendService.Current.PurchaseWithRealMoney("battlepass_season_" + season.SeasonNumber, StoreSection.BattlePass, season.PremiumPrice, true);
            if (transaction.Result != PurchaseResult.Success)
            {
                return transaction.Result;
            }

            StoreBackendService.Current.SetBattlePassPremiumUnlocked(true);
            StoreNotificationBridge.Raise(NotificationType.PurchaseSuccess, "Premium Battle Pass unlocked for " + season.SeasonDisplayName + "!");
            return PurchaseResult.Success;
        }

        /// <summary>Restores a previously-purchased premium unlock (Purchase System brief: "Restore Purchases") — the Battle Pass is this project's one durable/non-consumable product.</summary>
        public bool RestorePremium()
        {
            var history = StoreBackendService.Current.RestorePurchases();
            for (int i = 0; i < history.Count; i++)
            {
                if (history[i].Section == StoreSection.BattlePass)
                {
                    StoreBackendService.Current.SetBattlePassPremiumUnlocked(true);
                    return true;
                }
            }

            return false;
        }

        /// <summary>Claims tier <paramref name="tier"/>'s reward: requires premium unlocked, the tier already reached, and not yet claimed. Applies the same reward-shaped effects <c>ChampionshipManager.ApplyHeadlineReward</c> established (Coins/Gems to the wallet, cosmetic-shaped rewards into the real CosmeticInventory), extended here to cover every <see cref="RewardType"/> the Battle Pass actually uses.</summary>
        public bool TryClaimTier(int tier)
        {
            if (season == null || !Status.IsPremiumUnlocked)
            {
                return false;
            }

            BattlePassSeasonConfig.BattlePassTierEntry entry = season.GetTier(tier);
            if (entry == null || tier > CurrentTier() || Status.IsTierClaimed(tier))
            {
                return false;
            }

            ApplyTierReward(entry);
            StoreBackendService.Current.MarkBattlePassTierClaimed(tier);
            StoreNotificationBridge.Raise(NotificationType.PurchaseSuccess, "Claimed Battle Pass Tier " + tier + ": " + entry.RewardDisplayName);
            return true;
        }

        public int CurrentTier() => season != null ? season.ResolveTierForXp(Status.CurrentXp) : 0;

        private void HandleLocalMatchCompleted(PlayerMatchOutcome outcome)
        {
            if (season == null)
            {
                return;
            }

            int gained = season.XpPerMatch + (outcome.FinishPosition == 1 ? season.BonusXpPerWin : 0);
            StoreBackendService.Current.AddBattlePassXp(gained);
        }

        private static void ApplyTierReward(BattlePassSeasonConfig.BattlePassTierEntry entry)
        {
            switch (entry.RewardType)
            {
                case RewardType.Coins:
                    EconomyManager.Instance?.AddCoins(entry.RewardAmount);
                    break;
                case RewardType.Gems:
                    EconomyManager.Instance?.AddGems(entry.RewardAmount);
                    break;
                case RewardType.ExclusiveOutfit:
                case RewardType.ExclusiveEmote:
                case RewardType.VictoryPose:
                case RewardType.ExclusiveSkin:
                case RewardType.LimitedCosmetic:
                    if (!entry.RewardCosmeticId.IsNone)
                    {
                        CosmeticGrantService.Current?.GrantCosmetic(entry.RewardCosmeticId);
                    }

                    break;
                case RewardType.ProfileFrame:
                case RewardType.ChampionEffect:
                case RewardType.Title:
                case RewardType.Badge:
                    // No dedicated inventory slot exists yet for these types
                    // (see Sprint 10 report Remaining TODOs) — tracked in the
                    // Store's own ledger so ownership is still real, not just
                    // a notification.
                    StoreBackendService.Current.GrantStoreItem(new StoreItemId("battlepass_tier_" + entry.Tier + "_" + entry.RewardType), StoreSection.BattlePass);
                    break;
            }
        }
    }
}

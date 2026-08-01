using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Backend;
using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Store.Configuration;
using UnityEngine;

namespace GulfRun.Features.Store
{
    /// <summary>
    /// Composition root for the Store/Economy feature — the same role
    /// <c>Features.Online.Profile.ProfileManager</c> plays for the Online
    /// Ecosystem and <c>Features.Character.Loadout.PlayerLoadoutManager</c>
    /// plays for Character/Customization. Owns every catalog reference and
    /// every purchase flow (Gem Packages/Coin Packs/Store Items/Special
    /// Offers — Battle Pass purchases live in <see cref="BattlePass.BattlePassManager"/>,
    /// its own single-responsibility manager). Every purchase follows the
    /// same two-step shape: (1) <see cref="Core.Backend.IStoreBackendService"/>
    /// validates/records the transaction — "server-side purchase validation"
    /// (brief "Security" section), a real backend would reject here; (2)
    /// only on <see cref="PurchaseResult.Success"/> does this manager apply
    /// the actual local effect (credit Coins/Gems, grant a Cosmetic, or add
    /// to the Store's own item ledger) — the identical
    /// "backend records, feature manager applies" split
    /// <c>Features.Online.Championships.ChampionshipManager</c> established
    /// in Sprint 9.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class StoreManager : Singleton<StoreManager>, IEventBannerSource
    {
        [SerializeField] private GemPackageCatalogConfig gemPackageCatalog;
        [SerializeField] private CoinPackCatalogConfig coinPackCatalog;
        [SerializeField] private StoreItemCatalogConfig storeItemCatalog;
        [SerializeField] private SpecialOfferCatalogConfig specialOfferCatalog;

        public GemPackageCatalogConfig GemPackageCatalog => gemPackageCatalog;
        public CoinPackCatalogConfig CoinPackCatalog => coinPackCatalog;
        public StoreItemCatalogConfig StoreItemCatalog => storeItemCatalog;
        public SpecialOfferCatalogConfig SpecialOfferCatalog => specialOfferCatalog;

        /// <summary>The most recent purchase attempt's outcome — read by the Store view for Purchase Confirmation UI and by <c>StoreDebugView</c>.</summary>
        public PurchaseResult LastPurchaseResult { get; private set; } = PurchaseResult.Cancelled;

        public string LastPurchaseDisplayName { get; private set; } = string.Empty;

        public event Action PurchaseCompleted;

        protected override void OnInitialize()
        {
            EventBannerRegistry.Register(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventBannerRegistry.Unregister(this);
        }

        /// <summary>Sprint 13 (Main Menu Event Banner "Limited Offers"): any currently-active Special Offer.</summary>
        public IReadOnlyList<string> GetActiveBannerMessages()
        {
            if (specialOfferCatalog == null)
            {
                return Array.Empty<string>();
            }

            var messages = new List<string>();
            foreach (SpecialOfferCatalogConfig.SpecialOfferEntry offer in specialOfferCatalog.Offers)
            {
                if (offer != null && offer.IsActive)
                {
                    messages.Add("Limited Offer: " + offer.DisplayName + "!");
                }
            }

            return messages;
        }

        // --- Gem Packages ---

        public PurchaseResult PurchaseGemPackage(StoreItemId id)
        {
            GemPackageCatalogConfig.GemPackageEntry package = gemPackageCatalog != null ? gemPackageCatalog.GetPackage(id) : null;
            if (package == null)
            {
                return Fail(PurchaseResult.ValidationFailed, id.Value);
            }

            PurchaseTransaction transaction = StoreBackendService.Current.PurchaseWithRealMoney(id.Value, StoreSection.Gems, package.Price, false);
            if (transaction.Result != PurchaseResult.Success)
            {
                return Fail(transaction.Result, package.DisplayName);
            }

            EconomyManager.Instance?.AddGems(package.TotalGemAmount);
            return Succeed(package.DisplayName, "+" + package.TotalGemAmount + " Gems");
        }

        // --- Coin Packs ---

        public PurchaseResult PurchaseCoinPack(StoreItemId id)
        {
            CoinPackCatalogConfig.CoinPackEntry pack = coinPackCatalog != null ? coinPackCatalog.GetPack(id) : null;
            if (pack == null)
            {
                return Fail(PurchaseResult.ValidationFailed, id.Value);
            }

            PurchaseTransaction transaction = StoreBackendService.Current.PurchaseWithRealMoney(id.Value, StoreSection.Coins, pack.Price, false);
            if (transaction.Result != PurchaseResult.Success)
            {
                return Fail(transaction.Result, pack.DisplayName);
            }

            EconomyManager.Instance?.AddCoins(pack.TotalCoinAmount);
            return Succeed(pack.DisplayName, "+" + pack.TotalCoinAmount + " Coins");
        }

        // --- Store Items (Characters/Outfits/Emotes/VictoryPoses/VisualEffects/ProfileFrames) ---

        public PurchaseResult PurchaseStoreItem(StoreItemId id)
        {
            StoreItemCatalogConfig.StoreItemEntry entry = storeItemCatalog != null && storeItemCatalog.TryGetEntry(id, out StoreItemCatalogConfig.StoreItemEntry found) ? found : null;
            if (entry == null)
            {
                return Fail(PurchaseResult.ValidationFailed, id.Value);
            }

            if (OwnsStoreItemEntry(entry))
            {
                return Fail(PurchaseResult.AlreadyOwned, entry.DisplayName);
            }

            if (!TryChargeCurrency(entry.Currency, entry.PriceAmount, entry.RealMoneyPrice, entry.Section, id.Value, out string priceDisplay))
            {
                return Fail(PurchaseResult.InsufficientFunds, entry.DisplayName);
            }

            GrantStoreItemEntry(entry, id);
            return Succeed(entry.DisplayName, priceDisplay);
        }

        // --- Special Offers (Limited Offers / Bundles) ---

        public PurchaseResult PurchaseSpecialOffer(StoreItemId id)
        {
            SpecialOfferCatalogConfig.SpecialOfferEntry offer = specialOfferCatalog != null ? specialOfferCatalog.GetOffer(id) : null;
            if (offer == null || !offer.IsActive)
            {
                return Fail(PurchaseResult.ValidationFailed, id.Value);
            }

            if (!TryChargeCurrency(offer.Currency, offer.PriceAmount, offer.RealMoneyPrice, StoreSection.SpecialOffers, id.Value, out string priceDisplay))
            {
                return Fail(PurchaseResult.InsufficientFunds, offer.DisplayName);
            }

            for (int i = 0; i < offer.BundledStoreItemIds.Count; i++)
            {
                var bundledId = new StoreItemId(offer.BundledStoreItemIds[i]);
                if (storeItemCatalog != null && storeItemCatalog.TryGetEntry(bundledId, out StoreItemCatalogConfig.StoreItemEntry bundledEntry) && !OwnsStoreItemEntry(bundledEntry))
                {
                    GrantStoreItemEntry(bundledEntry, bundledId);
                }
            }

            return Succeed(offer.DisplayName, priceDisplay);
        }

        public bool OwnsStoreItem(StoreItemId id)
        {
            return storeItemCatalog != null && storeItemCatalog.TryGetEntry(id, out StoreItemCatalogConfig.StoreItemEntry entry) && OwnsStoreItemEntry(entry);
        }

        /// <summary>
        /// Sprint 11 "PERMANENT PURCHASE" upsell: for a Store entry linked
        /// to a cosmetic the local player currently owns only TEMPORARILY
        /// (a Daily Mission / Login Reward grant), returns its remaining
        /// time so the Store can show "Remaining Time" + let the existing
        /// Buy flow double as "Unlock Permanently" at the entry's normal
        /// Gem/Coin price. Returns false for anything else (not linked to a
        /// cosmetic, not owned at all, or already permanently owned).
        /// </summary>
        public bool TryGetTemporaryCosmeticExpiry(StoreItemId id, out double expiresAtSeconds)
        {
            expiresAtSeconds = 0d;
            if (storeItemCatalog == null || !storeItemCatalog.TryGetEntry(id, out StoreItemCatalogConfig.StoreItemEntry entry) || entry.LinkedCosmeticId.IsNone)
            {
                return false;
            }

            if (CosmeticGrantService.Current == null || CosmeticGrantService.Current.OwnsCosmeticPermanently(entry.LinkedCosmeticId))
            {
                return false;
            }

            IReadOnlyList<TemporaryCosmeticOwnership> temporary = CosmeticGrantService.Current.GetTemporaryCosmetics();
            for (int i = 0; i < temporary.Count; i++)
            {
                if (temporary[i].Id == entry.LinkedCosmeticId)
                {
                    expiresAtSeconds = temporary[i].ExpiresAtSeconds;
                    return true;
                }
            }

            return false;
        }

        // --- Internal helpers ---

        private bool OwnsStoreItemEntry(StoreItemCatalogConfig.StoreItemEntry entry)
        {
            if (entry.Currency == StoreCurrency.Free)
            {
                return true;
            }

            if (!entry.LinkedCosmeticId.IsNone)
            {
                // Sprint 11: a TEMPORARY grant is deliberately NOT "already
                // owned" here — the Store must keep offering the Buy flow
                // so a temporary owner can pay to Unlock Permanently
                // (brief "PERMANENT PURCHASE"). Only a permanent grant
                // counts as owned.
                return CosmeticGrantService.Current != null && CosmeticGrantService.Current.OwnsCosmeticPermanently(entry.LinkedCosmeticId);
            }

            return StoreBackendService.Current.OwnsStoreItem(entry.Id);
        }

        private void GrantStoreItemEntry(StoreItemCatalogConfig.StoreItemEntry entry, StoreItemId id)
        {
            if (!entry.LinkedCosmeticId.IsNone)
            {
                CosmeticGrantService.Current?.GrantCosmetic(entry.LinkedCosmeticId);
                return;
            }

            // Characters (all launch characters are already unlocked per the
            // Sprint 8 brief) and any content type with no dedicated slot yet
            // (Visual Effects/Profile Frames) are tracked in the Store's own
            // generic ledger — see Domain.OwnedStoreItem remarks.
            StoreBackendService.Current.GrantStoreItem(id, entry.Section);
        }

        private bool TryChargeCurrency(StoreCurrency currency, int priceAmount, RealMoneyPrice realMoneyPrice, StoreSection section, string productId, out string priceDisplay)
        {
            switch (currency)
            {
                case StoreCurrency.Free:
                    priceDisplay = "Free";
                    return true;

                case StoreCurrency.Gems:
                    priceDisplay = priceAmount + " Gems";
                    if (EconomyManager.Instance == null || !EconomyManager.Instance.TrySpendGems(priceAmount))
                    {
                        return false;
                    }

                    StoreBackendService.Current.RecordPremiumCurrencyPurchase(productId, section, priceDisplay);
                    return true;

                case StoreCurrency.Coins:
                    priceDisplay = priceAmount + " Coins";
                    if (EconomyManager.Instance == null || !EconomyManager.Instance.TrySpendCoins(priceAmount))
                    {
                        return false;
                    }

                    StoreBackendService.Current.RecordPremiumCurrencyPurchase(productId, section, priceDisplay);
                    return true;

                case StoreCurrency.RealMoney:
                    priceDisplay = realMoneyPrice.DisplayString;
                    StoreBackendService.Current.PurchaseWithRealMoney(productId, section, realMoneyPrice, false);
                    return true;

                default:
                    priceDisplay = string.Empty;
                    return false;
            }
        }

        private PurchaseResult Succeed(string displayName, string priceDisplay)
        {
            LastPurchaseResult = PurchaseResult.Success;
            LastPurchaseDisplayName = displayName;
            StoreNotificationBridge.Raise(NotificationType.PurchaseSuccess, "Purchased " + displayName + " (" + priceDisplay + ")");
            PurchaseCompleted?.Invoke();
            return PurchaseResult.Success;
        }

        private PurchaseResult Fail(PurchaseResult result, string displayName)
        {
            LastPurchaseResult = result;
            LastPurchaseDisplayName = displayName;
            PurchaseCompleted?.Invoke();
            return result;
        }
    }
}

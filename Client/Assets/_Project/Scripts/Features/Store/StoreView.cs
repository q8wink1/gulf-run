using System;
using GulfRun.Core;
using GulfRun.Core.Backend;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Store.BattlePass;
using GulfRun.Features.Store.Configuration;
using UnityEngine;

namespace GulfRun.Features.Store
{
    /// <summary>
    /// The modern Store screen — every section the Sprint 10 brief lists
    /// (Gems/Coins/Battle Pass/Characters/Outfits/Emotes/Victory Poses/
    /// Visual Effects/Special Offers), plus a Profile Frames tab and a
    /// "My Purchases" tab (Purchase History / Restore Purchases). A
    /// <see cref="SceneSingleton{T}"/> like every other Sprint 9 online
    /// screen. Every Buy button routes through <see cref="StoreManager"/>/
    /// <see cref="BattlePassManager"/> and reports the result inline
    /// (Purchase Confirmation) — never mutates a wallet or inventory
    /// directly.
    /// </summary>
    public sealed class StoreView : SceneSingleton<StoreView>, IMenuScreenOpener
    {
        private static readonly StoreSection[] AllTabs =
        {
            StoreSection.SpecialOffers, StoreSection.Gems, StoreSection.Coins, StoreSection.BattlePass,
            StoreSection.Characters, StoreSection.Outfits, StoreSection.Emotes, StoreSection.VictoryPoses,
            StoreSection.VisualEffects, StoreSection.ProfileFrames
        };

        private bool _open;
        private bool _showHistory;
        private StoreSection _section = StoreSection.SpecialOffers;
        private Vector2 _scroll;
        private string _lastFeedback = string.Empty;
        private GUIStyle _titleStyle;
        private GUIStyle _tabStyle;
        private GUIStyle _rowStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _feedbackStyle;

        private void OnEnable() => MenuScreenRouter.Register(MenuScreen.Store, this);

        private void OnDisable() => MenuScreenRouter.Unregister(MenuScreen.Store, this);

        /// <summary>Sprint 13 (Main Menu Right Menu "Store" button) — <see cref="IMenuScreenOpener"/>. Opens on the Special Offers tab.</summary>
        public void OpenScreen(MenuScreen screen)
        {
            _open = true;
            _showHistory = false;
            _section = StoreSection.SpecialOffers;
        }

        public void Close() => _open = false;

        private void OnGUI()
        {
            EnsureStyles();

            if (GUI.Button(new Rect(870, 10, 140, 34), _open ? "Close Store" : "Store"))
            {
                _open = !_open;
            }

            if (!_open)
            {
                return;
            }

            DrawPanel();
        }

        private void DrawPanel()
        {
            const float panelWidth = 700f;
            const float panelHeight = 620f;
            float x = 10f;
            float y = 90f;

            GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty);
            GUI.Label(new Rect(x + 14f, y + 8f, panelWidth - 28f, 26f), "STORE", _titleStyle);

            if (GUI.Button(new Rect(x + panelWidth - 150f, y + 6f, 136f, 24f), _showHistory ? "Back to Store" : "My Purchases"))
            {
                _showHistory = !_showHistory;
            }

            float rowY = y + 42f;

            if (!string.IsNullOrEmpty(_lastFeedback))
            {
                GUI.Label(new Rect(x + 14f, rowY, panelWidth - 28f, 20f), _lastFeedback, _feedbackStyle);
                rowY += 22f;
            }

            if (_showHistory)
            {
                DrawPurchaseHistory(x + 14f, rowY, panelWidth - 28f, y + panelHeight - rowY - 14f);
                return;
            }

            DrawTabs(x + 14f, rowY, panelWidth - 28f);
            rowY += 32f;

            Rect viewport = new Rect(x + 14f, rowY, panelWidth - 28f, y + panelHeight - rowY - 14f);
            switch (_section)
            {
                case StoreSection.Gems:
                    DrawGemPackages(viewport);
                    break;
                case StoreSection.Coins:
                    DrawCoinPacks(viewport);
                    break;
                case StoreSection.BattlePass:
                    DrawBattlePass(viewport);
                    break;
                case StoreSection.SpecialOffers:
                    DrawSpecialOffers(viewport);
                    break;
                default:
                    DrawStoreItems(viewport, _section);
                    break;
            }
        }

        private void DrawTabs(float x, float y, float width)
        {
            float tabWidth = width / AllTabs.Length;
            for (int i = 0; i < AllTabs.Length; i++)
            {
                bool active = _section == AllTabs[i];
                GUI.color = active ? Color.yellow : Color.white;
                if (GUI.Button(new Rect(x + i * tabWidth, y, tabWidth - 2f, 26f), ShortLabel(AllTabs[i]), _tabStyle))
                {
                    _section = AllTabs[i];
                }

                GUI.color = Color.white;
            }
        }

        private static string ShortLabel(StoreSection section)
        {
            switch (section)
            {
                case StoreSection.SpecialOffers: return "Offers";
                case StoreSection.BattlePass: return "Pass";
                case StoreSection.VictoryPoses: return "Poses";
                case StoreSection.VisualEffects: return "Effects";
                case StoreSection.ProfileFrames: return "Frames";
                default: return section.ToString();
            }
        }

        private void DrawGemPackages(Rect viewport)
        {
            GemPackageCatalogConfig catalog = StoreManager.Instance != null ? StoreManager.Instance.GemPackageCatalog : null;
            if (catalog == null)
            {
                return;
            }

            float rowH = 30f;
            Rect content = new Rect(0f, 0f, viewport.width - 20f, catalog.Packages.Count * rowH);
            _scroll = GUI.BeginScrollView(viewport, _scroll, content);
            for (int i = 0; i < catalog.Packages.Count; i++)
            {
                GemPackageCatalogConfig.GemPackageEntry entry = catalog.Packages[i];
                string label = entry.DisplayName + " — " + entry.TotalGemAmount + " Gems" + (entry.BonusGemAmount > 0 ? " (+" + entry.BonusGemAmount + " bonus)" : string.Empty) + (entry.IsLimitedOffer ? " [Limited]" : string.Empty);
                DrawBuyRow(i * rowH, content.width, label, entry.Price.DisplayString, () => SetFeedback(StoreManager.Instance.PurchaseGemPackage(entry.Id), entry.DisplayName));
            }

            GUI.EndScrollView();
        }

        private void DrawCoinPacks(Rect viewport)
        {
            CoinPackCatalogConfig catalog = StoreManager.Instance != null ? StoreManager.Instance.CoinPackCatalog : null;
            if (catalog == null)
            {
                return;
            }

            float rowH = 30f;
            Rect content = new Rect(0f, 0f, viewport.width - 20f, catalog.Packs.Count * rowH);
            _scroll = GUI.BeginScrollView(viewport, _scroll, content);
            for (int i = 0; i < catalog.Packs.Count; i++)
            {
                CoinPackCatalogConfig.CoinPackEntry entry = catalog.Packs[i];
                string label = entry.DisplayName + " — " + entry.TotalCoinAmount + " Coins" + (entry.BonusCoinAmount > 0 ? " (+" + entry.BonusCoinAmount + " bonus)" : string.Empty) + (entry.IsLimitedOffer ? " [Limited]" : string.Empty);
                DrawBuyRow(i * rowH, content.width, label, entry.Price.DisplayString, () => SetFeedback(StoreManager.Instance.PurchaseCoinPack(entry.Id), entry.DisplayName));
            }

            GUI.EndScrollView();
        }

        private void DrawStoreItems(Rect viewport, StoreSection section)
        {
            StoreItemCatalogConfig catalog = StoreManager.Instance != null ? StoreManager.Instance.StoreItemCatalog : null;
            if (catalog == null)
            {
                return;
            }

            var items = catalog.GetBySection(section);
            float rowH = 30f;
            Rect content = new Rect(0f, 0f, viewport.width - 20f, items.Count * rowH);
            _scroll = GUI.BeginScrollView(viewport, _scroll, content);
            for (int i = 0; i < items.Count; i++)
            {
                StoreItemCatalogConfig.StoreItemEntry entry = items[i];
                bool owned = StoreManager.Instance.OwnsStoreItem(entry.Id);

                // Sprint 11 "PERMANENT PURCHASE" upsell: a temporary
                // (Daily Mission / Login Reward) grant of this item's
                // linked cosmetic is never "Owned" here (see
                // StoreManager.OwnsStoreItemEntry), so it still reaches
                // this branch — show its remaining time and let the normal
                // Buy button double as "Unlock Permanently".
                double expiresAtSeconds = 0d;
                bool isTemporaryUpsell = !owned && StoreManager.Instance.TryGetTemporaryCosmeticExpiry(entry.Id, out expiresAtSeconds);
                string label = entry.DisplayName + (string.IsNullOrEmpty(entry.CollectionTag) ? string.Empty : " [" + entry.CollectionTag + "]") + (isTemporaryUpsell ? " [Temporary: " + FormatRemaining(expiresAtSeconds) + " left]" : string.Empty);
                string price = owned ? "Owned" : PriceLabel(entry.Currency, entry.PriceAmount, entry.RealMoneyPrice) + (entry.IsOnSale ? " (-" + entry.SaleDiscountPercent + "%)" : string.Empty);
                string buyLabel = isTemporaryUpsell ? "Unlock" : "Buy";
                DrawBuyRow(i * rowH, content.width, label, price, owned ? (Action)null : () => SetFeedback(StoreManager.Instance.PurchaseStoreItem(entry.Id), entry.DisplayName), buyLabel);
            }

            GUI.EndScrollView();
        }

        private void DrawSpecialOffers(Rect viewport)
        {
            SpecialOfferCatalogConfig catalog = StoreManager.Instance != null ? StoreManager.Instance.SpecialOfferCatalog : null;
            if (catalog == null)
            {
                return;
            }

            float rowH = 30f;
            Rect content = new Rect(0f, 0f, viewport.width - 20f, catalog.Offers.Count * rowH);
            _scroll = GUI.BeginScrollView(viewport, _scroll, content);
            for (int i = 0; i < catalog.Offers.Count; i++)
            {
                SpecialOfferCatalogConfig.SpecialOfferEntry entry = catalog.Offers[i];
                if (!entry.IsActive)
                {
                    continue;
                }

                string label = entry.DisplayName + " [" + entry.AssociatedEventLabel + "] (" + entry.BundledStoreItemIds.Count + " items)";
                string price = PriceLabel(entry.Currency, entry.PriceAmount, entry.RealMoneyPrice);
                DrawBuyRow(i * rowH, content.width, label, price, () => SetFeedback(StoreManager.Instance.PurchaseSpecialOffer(entry.Id), entry.DisplayName));
            }

            GUI.EndScrollView();
        }

        private void DrawBattlePass(Rect viewport)
        {
            BattlePassManager manager = BattlePassManager.Instance;
            if (manager == null || manager.Season == null)
            {
                return;
            }

            BattlePassStatus status = manager.Status;
            float rowH = 30f;
            int tierCount = manager.Season.TotalTierCount;
            Rect content = new Rect(0f, 0f, viewport.width - 20f, (tierCount + 1) * rowH);
            _scroll = GUI.BeginScrollView(viewport, _scroll, content);

            string headerLine = manager.Season.SeasonDisplayName + " — Tier " + manager.CurrentTier() + "/" + tierCount + " (" + status.CurrentXp + " XP) — Premium: " + (status.IsPremiumUnlocked ? "Unlocked" : "Locked");
            GUI.Label(new Rect(0f, 0f, content.width - 190f, rowH), headerLine, _labelStyle);
            if (!status.IsPremiumUnlocked)
            {
                if (GUI.Button(new Rect(content.width - 180f, 2f, 180f, rowH - 4f), "Unlock Premium (" + manager.Season.PremiumPrice.DisplayString + ")"))
                {
                    SetFeedback(manager.PurchasePremium(), "Premium Battle Pass");
                }
            }
            else if (GUI.Button(new Rect(content.width - 180f, 2f, 180f, rowH - 4f), "Restore Purchases"))
            {
                _lastFeedback = manager.RestorePremium() ? "Restored Premium Battle Pass." : "Nothing to restore.";
            }

            for (int i = 0; i < tierCount; i++)
            {
                BattlePassSeasonConfig.BattlePassTierEntry tier = manager.Season.Tiers[i];
                float rowTop = (i + 1) * rowH;
                bool reached = tier.Tier <= manager.CurrentTier();
                bool claimed = status.IsTierClaimed(tier.Tier);
                string state = claimed ? "Claimed" : reached ? "Ready" : "Locked";
                GUI.Label(new Rect(0f, rowTop, content.width - 130f, rowH - 2f), "Tier " + tier.Tier + " (" + tier.XpRequired + " XP): " + tier.RewardDisplayName + " — " + state, _labelStyle);

                if (reached && !claimed)
                {
                    if (GUI.Button(new Rect(content.width - 120f, rowTop, 120f, rowH - 4f), "Claim"))
                    {
                        _lastFeedback = manager.TryClaimTier(tier.Tier) ? "Claimed Tier " + tier.Tier + "." : "Unable to claim.";
                    }
                }
            }

            GUI.EndScrollView();
        }

        private void DrawPurchaseHistory(float x, float y, float width, float height)
        {
            var history = StoreBackendService.Current.GetPurchaseHistory();
            float rowH = 22f;
            Rect viewport = new Rect(x, y, width, height);
            Rect content = new Rect(0f, 0f, width - 20f, history.Count * rowH);
            _scroll = GUI.BeginScrollView(viewport, _scroll, content);
            for (int i = 0; i < history.Count; i++)
            {
                PurchaseTransaction t = history[i];
                string line = t.ProductId + " — " + t.PriceDisplay + " — " + t.Result;
                GUI.Label(new Rect(0f, i * rowH, content.width, rowH - 2f), line, _labelStyle);
            }

            GUI.EndScrollView();
        }

        private void DrawBuyRow(float rowY, float width, string label, string priceLabel, Action onBuy, string buyLabel = "Buy")
        {
            GUI.Label(new Rect(0f, rowY, width - 190f, 26f), label, _labelStyle);
            GUI.Label(new Rect(width - 190f, rowY, 90f, 26f), priceLabel, _labelStyle);
            if (onBuy != null)
            {
                if (GUI.Button(new Rect(width - 96f, rowY, 96f, 26f), buyLabel, _rowStyle))
                {
                    onBuy();
                }
            }
        }

        private static string FormatRemaining(double expiresAtSeconds)
        {
            double remaining = expiresAtSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (remaining <= 0d)
            {
                return "expired";
            }

            TimeSpan span = TimeSpan.FromSeconds(remaining);
            return span.Days > 0 ? span.Days + "d " + span.Hours + "h" : span.Hours + "h " + span.Minutes + "m";
        }

        private static string PriceLabel(StoreCurrency currency, int amount, RealMoneyPrice realMoneyPrice)
        {
            switch (currency)
            {
                case StoreCurrency.Gems: return amount + " Gems";
                case StoreCurrency.Coins: return amount + " Coins";
                case StoreCurrency.RealMoney: return realMoneyPrice.DisplayString;
                default: return "Free";
            }
        }

        private void SetFeedback(PurchaseResult result, string displayName)
        {
            _lastFeedback = displayName + ": " + result;
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _titleStyle.normal.textColor = Color.white;

            _tabStyle = new GUIStyle(GUI.skin.button) { fontSize = 11 };
            _rowStyle = new GUIStyle(GUI.skin.button) { fontSize = 12 };

            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleLeft };
            _labelStyle.normal.textColor = Color.white;

            _feedbackStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _feedbackStyle.normal.textColor = Color.green;
        }
    }
}

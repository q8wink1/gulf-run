using GulfRun.Core.Backend;
using GulfRun.Core.Managers;
using GulfRun.Domain;
using GulfRun.Features.Store.BattlePass;
using GulfRun.Features.Store.Inventory;
using UnityEngine;

namespace GulfRun.Features.Store
{
    /// <summary>
    /// Debug panel required by the Sprint 10 brief: Wallet Values, Purchase
    /// Status, Owned Items, Battle Pass Level. Same on-screen, dev-build-only
    /// placeholder style as <c>OnlineDebugView</c>/<c>MultiplayerDebugView</c>.
    /// <c>panelX: 2710</c> is <c>Gameplay.unity</c>'s next free slot after
    /// Sprint 9's <c>OnlineDebugView</c> at <c>panelX: 2260</c>.
    /// </summary>
    public sealed class StoreDebugView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private bool showOnScreenDebug = true;
        [SerializeField] private int panelX = 2710;
        [SerializeField] private int panelY = 10;

        private void OnGUI()
        {
            if (!showOnScreenDebug)
            {
                return;
            }

            int y = panelY;
            const int lineHeight = 18;
            const int width = 440;

            void Line(string text)
            {
                GUI.Label(new Rect(panelX, y, width, lineHeight), text);
                y += lineHeight;
            }

            Line("[Store] Coins: " + (EconomyManager.Instance != null ? EconomyManager.Instance.Coins : 0));
            Line("[Store] Gems: " + (EconomyManager.Instance != null ? EconomyManager.Instance.Gems : 0));

            if (StoreManager.Instance != null)
            {
                Line("Last Purchase: " + StoreManager.Instance.LastPurchaseDisplayName + " (" + StoreManager.Instance.LastPurchaseResult + ")");
            }

            if (InventoryManager.Instance != null)
            {
                Line("Owned Items (all): " + InventoryManager.Instance.TotalOwnedItemCount);
                Line("Owned Cosmetics: " + InventoryManager.Instance.GetOwnedCosmetics().Count);
                Line("Owned Store-Ledger Items: " + InventoryManager.Instance.GetOwnedStoreItems().Count);
            }

            if (BattlePassManager.Instance != null && BattlePassManager.Instance.Season != null)
            {
                Line("Battle Pass Level: Tier " + BattlePassManager.Instance.CurrentTier() + "/" + BattlePassManager.Instance.Season.TotalTierCount + " (" + BattlePassManager.Instance.Status.CurrentXp + " XP)");
                Line("Battle Pass Premium: " + BattlePassManager.Instance.Status.IsPremiumUnlocked);
            }

            Line("Store Backend Status: " + (StoreBackendService.Current is LocalStoreBackendService ? "Mock/Local (in-memory)" : "Custom"));
            Line("Purchase History Count: " + StoreBackendService.Current.GetPurchaseHistory().Count);
        }
#endif
    }
}

using System;
using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Backend
{
    /// <summary>
    /// The single seam every Sprint 10 Store/Economy manager talks to
    /// instead of any concrete payment/backend system — the same "swap the
    /// implementation, zero caller changes" contract <see cref="IOnlineBackendService"/>
    /// already gives the Online Ecosystem. Deliberately scoped to what a
    /// real backend actually owns: transaction validation/history/refunds
    /// and the generic Store-item ledger (Visual Effects/Profile Frames —
    /// content types with no other home yet, see <see cref="Domain.OwnedStoreItem"/>)
    /// and Battle Pass progress. Applying an entitlement to the local game
    /// state (crediting Coins/Gems, granting a Cosmetic) is deliberately
    /// NOT this interface's job — that stays in <c>Features.Store.StoreManager</c>/
    /// <c>BattlePassManager</c>, exactly the same split
    /// <c>Features.Online.Championships.ChampionshipManager</c> already
    /// established between "the backend records what happened" and "the
    /// feature manager applies the local effect."
    /// "Server-side purchase validation / anti-cheat protection / secure
    /// transactions" (brief "Security" section) is satisfied by this
    /// abstraction itself: every purchase call is routed through here, so a
    /// real backend performing genuine server-side receipt validation is a
    /// single <see cref="StoreBackendService.Current"/> swap, not a rewrite.
    /// </summary>
    public interface IStoreBackendService
    {
        // --- Purchases ---

        /// <summary>
        /// Validates and records a real-money purchase (Gem Package, Coin
        /// Pack, or the premium Battle Pass). A real implementation would
        /// verify a platform receipt here (Transaction Validation); the
        /// mock always succeeds. Never mutates Coins/Gems/inventory itself —
        /// the caller applies the entitlement once <see cref="PurchaseResult.Success"/>
        /// comes back.
        /// </summary>
        PurchaseTransaction PurchaseWithRealMoney(string productId, StoreSection section, RealMoneyPrice price, bool isRestorable);

        /// <summary>Validates and records a Gems/Coins-priced purchase (a Store Item or Special Offer bundle). Same "record only, caller applies the effect" contract as <see cref="PurchaseWithRealMoney"/>.</summary>
        PurchaseTransaction RecordPremiumCurrencyPurchase(string productId, StoreSection section, string priceDisplay);

        IReadOnlyList<PurchaseTransaction> GetPurchaseHistory();

        /// <summary>Every past <see cref="PurchaseTransaction.IsRestorable"/> entry — durable, non-consumable products (today: the premium Battle Pass) a player is entitled to re-claim after e.g. a reinstall.</summary>
        IReadOnlyList<PurchaseTransaction> RestorePurchases();

        /// <summary>Refund Protection: succeeds only while still inside the transaction's refund window (see <see cref="PurchaseTransaction.RefundWindowExpiresAtSeconds"/>) and marks it <see cref="PurchaseResult.RefundIssued"/>. Never re-applies the entitlement removal itself (mirrors the purchase-side split above) — the caller is responsible for reversing local state.</summary>
        bool TryRefund(string transactionId, double nowSeconds, out PurchaseTransaction refunded);

        event Action PurchaseHistoryChanged;

        // --- Generic Store-item ledger (Visual Effects / Profile Frames / future types) ---

        bool OwnsStoreItem(StoreItemId id);

        void GrantStoreItem(StoreItemId id, StoreSection section);

        IReadOnlyList<OwnedStoreItem> GetOwnedStoreItems();

        event Action InventoryChanged;

        // --- Battle Pass ---

        BattlePassStatus GetBattlePassStatus();

        void SetBattlePassPremiumUnlocked(bool unlocked);

        void AddBattlePassXp(int amount);

        void MarkBattlePassTierClaimed(int tier);

        event Action BattlePassChanged;
    }
}

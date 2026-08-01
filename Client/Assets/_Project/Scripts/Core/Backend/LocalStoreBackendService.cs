using System;
using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Backend
{
    /// <summary>
    /// In-memory mock <see cref="IStoreBackendService"/> — the Sprint 10
    /// counterpart to <see cref="LocalOnlineBackendService"/>: every
    /// purchase always validates successfully (no real payment gateway
    /// exists — see Sprint 10 report Remaining TODOs) but is otherwise
    /// tracked exactly like a real backend would (transaction ledger,
    /// generic item ledger, Battle Pass progress), so the whole Store UI has
    /// real, interactive, swappable-later data from the first frame.
    /// </summary>
    public sealed class LocalStoreBackendService : IStoreBackendService
    {
        /// <summary>Refund Protection window — 48 in-game hours, an honest placeholder balance value (P045) exactly like every other economy number in this project.</summary>
        private const double RefundWindowSeconds = 48 * 60 * 60;

        private readonly List<PurchaseTransaction> _history = new List<PurchaseTransaction>();
        private readonly HashSet<string> _ownedStoreItems = new HashSet<string>();
        private readonly List<OwnedStoreItem> _ownedStoreItemList = new List<OwnedStoreItem>();
        private readonly BattlePassStatus _battlePass = new BattlePassStatus(1);

        private int _transactionCounter;

        public event Action PurchaseHistoryChanged;
        public event Action InventoryChanged;
        public event Action BattlePassChanged;

        public PurchaseTransaction PurchaseWithRealMoney(string productId, StoreSection section, RealMoneyPrice price, bool isRestorable)
        {
            var transaction = new PurchaseTransaction(
                NextTransactionId(),
                productId,
                section,
                price.DisplayString,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                PurchaseResult.Success,
                isRestorable,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds() + RefundWindowSeconds);

            _history.Insert(0, transaction);
            PurchaseHistoryChanged?.Invoke();
            return transaction;
        }

        public PurchaseTransaction RecordPremiumCurrencyPurchase(string productId, StoreSection section, string priceDisplay)
        {
            var transaction = new PurchaseTransaction(
                NextTransactionId(),
                productId,
                section,
                priceDisplay,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                PurchaseResult.Success,
                false,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds() + RefundWindowSeconds);

            _history.Insert(0, transaction);
            PurchaseHistoryChanged?.Invoke();
            return transaction;
        }

        public IReadOnlyList<PurchaseTransaction> GetPurchaseHistory() => _history;

        public IReadOnlyList<PurchaseTransaction> RestorePurchases()
        {
            var restored = new List<PurchaseTransaction>();
            for (int i = 0; i < _history.Count; i++)
            {
                if (_history[i].IsRestorable && _history[i].Result == PurchaseResult.Success)
                {
                    restored.Add(_history[i]);
                }
            }

            return restored;
        }

        public bool TryRefund(string transactionId, double nowSeconds, out PurchaseTransaction refunded)
        {
            for (int i = 0; i < _history.Count; i++)
            {
                PurchaseTransaction candidate = _history[i];
                if (!string.Equals(candidate.TransactionId, transactionId, StringComparison.Ordinal))
                {
                    continue;
                }

                if (candidate.Result != PurchaseResult.Success || nowSeconds > candidate.RefundWindowExpiresAtSeconds)
                {
                    refunded = default;
                    return false;
                }

                refunded = new PurchaseTransaction(candidate.TransactionId, candidate.ProductId, candidate.Section, candidate.PriceDisplay, candidate.TimestampSeconds, PurchaseResult.RefundIssued, candidate.IsRestorable, candidate.RefundWindowExpiresAtSeconds);
                _history[i] = refunded;
                PurchaseHistoryChanged?.Invoke();
                return true;
            }

            refunded = default;
            return false;
        }

        public bool OwnsStoreItem(StoreItemId id) => !id.IsNone && _ownedStoreItems.Contains(id.Value);

        public void GrantStoreItem(StoreItemId id, StoreSection section)
        {
            if (id.IsNone || !_ownedStoreItems.Add(id.Value))
            {
                return;
            }

            _ownedStoreItemList.Add(new OwnedStoreItem(id, section, DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
            InventoryChanged?.Invoke();
        }

        public IReadOnlyList<OwnedStoreItem> GetOwnedStoreItems() => _ownedStoreItemList;

        public BattlePassStatus GetBattlePassStatus() => _battlePass;

        public void SetBattlePassPremiumUnlocked(bool unlocked)
        {
            if (_battlePass.IsPremiumUnlocked == unlocked)
            {
                return;
            }

            _battlePass.IsPremiumUnlocked = unlocked;
            BattlePassChanged?.Invoke();
        }

        public void AddBattlePassXp(int amount)
        {
            if (amount == 0)
            {
                return;
            }

            _battlePass.CurrentXp += amount;
            BattlePassChanged?.Invoke();
        }

        public void MarkBattlePassTierClaimed(int tier)
        {
            if (_battlePass.IsTierClaimed(tier))
            {
                return;
            }

            _battlePass.MarkTierClaimed(tier);
            BattlePassChanged?.Invoke();
        }

        private string NextTransactionId()
        {
            _transactionCounter++;
            return "TXN-" + DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() + "-" + _transactionCounter.ToString("0000");
        }
    }
}

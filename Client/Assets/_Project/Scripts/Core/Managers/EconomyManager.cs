using System;
using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Coordinates currencies, inventory value, and monetization-adjacent rules
    /// on the client, always deferring authoritative validation to the backend.
    /// References: P012 (Economy System), P045 (Monetization System).
    ///
    /// Sprint 7 note: implements a minimal in-memory Coins wallet — the first
    /// real currency this manager tracks — so the Race Finish reward flow
    /// (<c>Features.RaceFinish.Rewards.RaceRewardApplier</c>) has somewhere
    /// real to credit a race's <see cref="Domain.RaceRewardBreakdown.TotalReward"/>.
    /// Exactly the same "in-memory default, swap the storage later" posture
    /// Sprint 3's <see cref="SaveManager"/> already established for
    /// best-distance/best-score/coins-collected — this does NOT persist
    /// across application restarts and is NOT the authoritative backend
    /// wallet (P012/P039); replace the storage here once those systems land.
    ///
    /// Sprint 8 note: adds the same treatment for Gems, the premium currency
    /// Premium Cosmetics unlock with (<c>Features.Character.Loadout.
    /// PlayerLoadoutManager</c>). A small starting balance is granted so the
    /// Character Menu's unlock flow is exercisable end-to-end with no shop/
    /// purchase system yet (P013/P045 remain the eventual source of truth
    /// for actually acquiring Gems).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EconomyManager : Singleton<EconomyManager>
    {
        [Tooltip("Starting Gems balance so the Character Menu's Premium Cosmetics unlock flow (Sprint 8) is exercisable with no Shop/IAP system yet (P013/P045).")]
        [SerializeField] private int startingGems = 500;

        private int _coins;
        private int _gems;

        public int Coins => _coins;
        public int Gems => _gems;

        /// <summary>Raised whenever <see cref="Coins"/> changes.</summary>
        public event Action<int> CoinsChanged;

        /// <summary>Raised whenever <see cref="Gems"/> changes.</summary>
        public event Action<int> GemsChanged;

        protected override void OnInitialize()
        {
            // TODO(Sprint 2+): Implement full currency/inventory client-side
            // cache (inventory value) once backend economy endpoints are
            // available (P012/P039). Coins/Gems are handled below.
            _gems = startingGems > 0 ? startingGems : 0;
        }

        /// <summary>Credits (or, for a negative amount, debits down to zero) the local Coins wallet. A no-op for zero.</summary>
        public void AddCoins(int amount)
        {
            if (amount == 0)
            {
                return;
            }

            int updated = _coins + amount;
            _coins = updated < 0 ? 0 : updated;
            CoinsChanged?.Invoke(_coins);
        }

        /// <summary>Credits (or, for a negative amount, debits down to zero) the local Gems wallet. A no-op for zero.</summary>
        public void AddGems(int amount)
        {
            if (amount == 0)
            {
                return;
            }

            int updated = _gems + amount;
            _gems = updated < 0 ? 0 : updated;
            GemsChanged?.Invoke(_gems);
        }

        /// <summary>Atomically spends Gems if (and only if) the current balance can afford <paramref name="amount"/> (see <see cref="Domain.CosmeticUnlockRules"/>). Returns false and leaves the balance untouched otherwise.</summary>
        public bool TrySpendGems(int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            if (_gems < amount)
            {
                return false;
            }

            _gems -= amount;
            GemsChanged?.Invoke(_gems);
            return true;
        }
    }
}

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
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EconomyManager : Singleton<EconomyManager>
    {
        private int _coins;

        public int Coins => _coins;

        /// <summary>Raised whenever <see cref="Coins"/> changes.</summary>
        public event Action<int> CoinsChanged;

        protected override void OnInitialize()
        {
            // TODO(Sprint 2+): Implement full currency/inventory client-side
            // cache (Gems, inventory value) once backend economy endpoints
            // are available (P012/P039). Coins are handled below (Sprint 7).
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
    }
}

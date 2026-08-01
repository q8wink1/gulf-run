using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Backend;
using GulfRun.Core.Services;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Store.Inventory
{
    /// <summary>
    /// Read-only aggregation over every "what does the local player own"
    /// source for the Inventory screen (brief: "Display all owned:
    /// Characters, Skins, Outfits, Emotes, Victory Poses, Effects") — the
    /// same aggregator role <c>Features.Online.Profile.ProfileManager</c>
    /// plays for the Player Profile screen, just for ownership instead of
    /// identity/stats. Reads through <see cref="ICosmeticGrantService"/>
    /// (real Sprint 8 cosmetics) and <see cref="IStoreBackendService"/>
    /// (Store-only ledger items: Visual Effects/Profile Frames) — never a
    /// third, duplicate copy of ownership state.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class InventoryManager : Singleton<InventoryManager>
    {
        [Tooltip("Total launch character count — all unlocked from the start (Sprint 8 brief); shown as 'X/Total Unlocked' since Features.Store cannot reference Features.Character's catalog directly.")]
        [SerializeField] private int totalLaunchCharacterCount = 12;

        protected override void OnInitialize()
        {
        }

        public int TotalLaunchCharacterCount => totalLaunchCharacterCount;

        /// <summary>All launch Characters are unlocked from the start (Sprint 8 brief) — honestly reported as a count rather than a per-id list, since Features.Store never references Features.Character's catalog (see class remarks).</summary>
        public int UnlockedCharacterCount => totalLaunchCharacterCount;

        public IReadOnlyList<CosmeticId> GetOwnedCosmetics() =>
            CosmeticGrantService.Current != null ? CosmeticGrantService.Current.GetOwnedCosmetics() : System.Array.Empty<CosmeticId>();

        /// <summary>Sprint 11: every currently-active temporary (Daily Mission / Login Reward) cosmetic grant, with its expiry — brief "TEMPORARY COSMETICS: Countdown timer displayed."</summary>
        public IReadOnlyList<TemporaryCosmeticOwnership> GetTemporaryCosmetics() =>
            CosmeticGrantService.Current != null ? CosmeticGrantService.Current.GetTemporaryCosmetics() : System.Array.Empty<TemporaryCosmeticOwnership>();

        public IReadOnlyList<OwnedStoreItem> GetOwnedStoreItems() => StoreBackendService.Current.GetOwnedStoreItems();

        public int TotalOwnedItemCount => GetOwnedCosmetics().Count + GetOwnedStoreItems().Count + totalLaunchCharacterCount;
    }
}

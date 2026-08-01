using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// The seam <c>Features.Store</c> and (Sprint 11) <c>Features.Progression</c>
    /// grant/query cosmetic ownership through instead of ever referencing
    /// <c>Features.Character</c> directly — the same "implement the Core
    /// interface, don't reference the Feature" shape <see cref="ILocalLoadoutProvider"/>
    /// already established for read-only loadout data in Sprint 9. Only the
    /// three <see cref="CosmeticSlot"/> values with real Sprint 8 content
    /// today (Outfit/Emote/VictoryPose) round-trip through here — Visual
    /// Effects and Profile Frames have no slot yet, so the Store keeps
    /// ownership of those in its own ledger (see <c>Core.Backend.IStoreBackendService</c>).
    /// <para>
    /// Sprint 11 addition: temporary (expiring) ownership, for Daily
    /// Mission / Login Reward grants (brief "TEMPORARY COSMETICS"). A
    /// temporary grant never overrides an existing permanent one
    /// (<see cref="GrantTemporaryCosmetic"/> returns false — the caller is
    /// expected to check <see cref="OwnsCosmeticPermanently"/> first and
    /// grant an alternative reward instead, per brief "Never reward
    /// temporary duplicate"), and a later permanent grant always upgrades a
    /// temporary one (see <see cref="Domain.CosmeticInventory.Grant"/>).
    /// </para>
    /// </summary>
    public interface ICosmeticGrantService
    {
        /// <summary>True whether ownership is permanent or a still-active temporary grant.</summary>
        bool OwnsCosmetic(CosmeticId id);

        /// <summary>True only for a permanent grant — the check the Store uses to decide whether a purchase is "Already Owned" (a temporary owner may still buy the permanent version).</summary>
        bool OwnsCosmeticPermanently(CosmeticId id);

        /// <summary>Directly grants permanent ownership — no currency is spent here (the caller already charged Gems/Coins/real money before calling this). Idempotent. Upgrades an existing temporary grant to permanent. Returns false only if <paramref name="id"/> is <see cref="CosmeticId.None"/>.</summary>
        bool GrantCosmetic(CosmeticId id);

        /// <summary>Grants temporary ownership until <paramref name="expiresAtSeconds"/> (real-world/Unix epoch seconds). Returns false for <see cref="CosmeticId.None"/> or an id already permanently owned.</summary>
        bool GrantTemporaryCosmetic(CosmeticId id, double expiresAtSeconds);

        /// <summary>Every cosmetic the local player currently owns (permanent + still-active temporary), across every slot — used by the Store's Inventory screen.</summary>
        IReadOnlyList<CosmeticId> GetOwnedCosmetics();

        /// <summary>Every currently-active temporary grant, with its expiry — used by the Store's "Remaining Time / Unlock Permanently" upsell (brief "PERMANENT PURCHASE").</summary>
        IReadOnlyList<TemporaryCosmeticOwnership> GetTemporaryCosmetics();
    }
}

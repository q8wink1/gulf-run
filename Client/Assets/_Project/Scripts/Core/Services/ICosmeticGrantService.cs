using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// The seam <c>Features.Store</c> grants/queries cosmetic ownership
    /// through instead of ever referencing <c>Features.Character</c>
    /// directly — the same "implement the Core interface, don't reference
    /// the Feature" shape <see cref="ILocalLoadoutProvider"/> already
    /// established for read-only loadout data in Sprint 9. Only the three
    /// <see cref="CosmeticSlot"/> values with real Sprint 8 content today
    /// (Outfit/Emote/VictoryPose) round-trip through here — Visual Effects
    /// and Profile Frames have no slot yet, so the Store keeps ownership of
    /// those in its own ledger (see <c>Core.Backend.IStoreBackendService</c>).
    /// </summary>
    public interface ICosmeticGrantService
    {
        bool OwnsCosmetic(CosmeticId id);

        /// <summary>Directly grants permanent ownership — no currency is spent here (the caller, e.g. <c>Features.Store.StoreManager</c>, already charged Gems/Coins/real money before calling this). Idempotent. Returns false only if <paramref name="id"/> is <see cref="CosmeticId.None"/>.</summary>
        bool GrantCosmetic(CosmeticId id);

        /// <summary>Every cosmetic the local player owns, across every slot — used by the Store's Inventory screen.</summary>
        IReadOnlyList<CosmeticId> GetOwnedCosmetics();
    }
}

using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Read-only view of the local player's current Character/Country/
    /// Outfit selection — the same cross-feature-seam shape as
    /// <see cref="IRunSpeedProvider"/>/<see cref="IGameStateProvider"/>.
    /// Implemented by <c>Features.Character.Loadout.PlayerLoadoutManager</c>
    /// (registered via <see cref="LocalLoadoutProviderService"/>) so
    /// <c>Features.Online.Profile.ProfileManager</c> can show "Current
    /// Character"/"Current Outfit" on the Player Profile screen without
    /// Features.Online ever referencing Features.Character.
    /// </summary>
    public interface ILocalLoadoutProvider
    {
        CharacterId CurrentCharacterId { get; }
        string CurrentCharacterDisplayName { get; }
        CosmeticId CurrentOutfitId { get; }
        string CurrentOutfitDisplayName { get; }
        GulfCountry Country { get; }
    }
}

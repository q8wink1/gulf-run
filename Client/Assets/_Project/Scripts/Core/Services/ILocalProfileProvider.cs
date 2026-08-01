using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Read-only view of the local player's full <see cref="PlayerProfileSummary"/>
    /// (Name/Level/League/Ranks/Coins/Gems/Character/Outfit/Country/Status)
    /// — the same "implement a Core interface, never reference the owning
    /// Feature" shape as <see cref="ILocalLoadoutProvider"/>/
    /// <see cref="IMapContextProvider"/>. Implemented by
    /// <c>Features.Online.Profile.ProfileManager</c> so Sprint 13's Main
    /// Menu Top Bar/Player Preview can show it without
    /// Features.MainMenu ever referencing Features.Online.
    /// </summary>
    public interface ILocalProfileProvider
    {
        /// <summary>False until an account exists and the first refresh has completed.</summary>
        bool HasProfile { get; }

        /// <summary>Only meaningful once <see cref="HasProfile"/> is true.</summary>
        PlayerProfileSummary LocalProfile { get; }
    }
}

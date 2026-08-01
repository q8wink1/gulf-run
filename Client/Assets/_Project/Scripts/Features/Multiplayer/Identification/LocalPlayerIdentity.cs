using System;
using GulfRun.Domain;

namespace GulfRun.Features.Multiplayer.Identification
{
    /// <summary>
    /// Generates the local client's <see cref="PlayerIdentity"/>. PlayerId is
    /// a locally-generated GUID for now — <see cref="PlayerIdentity.ProfileId"/>
    /// stays empty until a real account/profile system exists (P041
    /// Authentication System), and ConnectionId is assigned by whichever
    /// <see cref="Core.Networking.IMatchTransport"/> accepts the join. Same
    /// "real thing later, honest placeholder now" approach already used by
    /// <see cref="Core.Save.IProgressRepository"/>/SaveManager.
    /// </summary>
    public static class LocalPlayerIdentity
    {
        public static PlayerIdentity CreateLocal(string displayName)
        {
            string safeDisplayName = string.IsNullOrWhiteSpace(displayName) ? "Player" : displayName;

            return new PlayerIdentity(
                playerId: Guid.NewGuid().ToString("N"),
                displayName: safeDisplayName,
                connectionId: -1,
                profileId: string.Empty);
        }
    }
}

using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Read-only + action seam onto the Match Flow (Sprint 13 PLAY button +
    /// bottom bar Room Code/Matchmaking ETA + "SOCIAL: Room Code"/"Invite
    /// button"). Implemented by
    /// <c>Features.Multiplayer.Session.SessionManager</c> so
    /// Features.MainMenu can drive Create/Leave Match without ever
    /// referencing Features.Multiplayer — the same shape as
    /// <see cref="ILocalLoadoutProvider"/>.
    /// </summary>
    public interface IMatchLobbySummaryProvider
    {
        bool IsInMatch { get; }
        bool IsMatchmaking { get; }
        bool IsHost { get; }

        /// <see cref="RoomCode.None"/> when not currently hosting.
        RoomCode LocalRoomCode { get; }

        int LobbyPlayerCount { get; }
        int RequiredPlayerCount { get; }

        /// <summary>Starts hosting a brand-new Quick Match (the PLAY button's action).</summary>
        void StartQuickMatch(string localDisplayName);

        /// <summary>Cancels pending matchmaking, or leaves an already-joined match — whichever applies.</summary>
        void CancelOrLeaveMatch();
    }
}

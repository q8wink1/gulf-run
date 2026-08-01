using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Read-only + action seam onto the local player's Friends list (Sprint
    /// 13 "SOCIAL: Friends Online" counts; Sprint 15 adds the small lookup/
    /// invite surface the Pre-Race Lobby's "Invite Friends" panel needs).
    /// Implemented by <c>Features.Online.Friends.FriendManager</c> so
    /// Features.MainMenu/Features.Matchmaking can show/use it without ever
    /// referencing Features.Online.
    /// </summary>
    public interface IFriendsSummaryProvider
    {
        int TotalFriendsCount { get; }

        int OnlineFriendsCount { get; }

        /// <summary>Sprint 15 (Pre-Race Lobby "Invite Friends" list). Every friend whose last published status is not Offline.</summary>
        IReadOnlyList<PlayerId> GetOnlineFriends();

        /// <summary>Sprint 15. The friend's nickname, or a safe fallback if their profile cannot be resolved.</summary>
        string GetFriendDisplayName(PlayerId friend);

        /// <summary>Sprint 15 (Pre-Race Lobby "Invite Friends"). Best-effort only — see <c>FriendManager.InviteFriend</c> remarks.</summary>
        void InviteFriend(PlayerId friend);
    }
}

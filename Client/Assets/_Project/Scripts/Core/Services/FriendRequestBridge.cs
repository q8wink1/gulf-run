using System;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Lets any feature raise "Add Friend" for a player it can already
    /// identify (a race result, a lobby participant, ...) without
    /// referencing Features.Online.Friends.FriendManager directly — the
    /// same event-bridge shape as <see cref="PlayerStatEventService"/>.
    /// <c>FriendManager</c> is the sole subscriber and performs the actual
    /// <c>Core.Backend.IOnlineBackendService.SendFriendRequest</c> call.
    /// </summary>
    public static class FriendRequestBridge
    {
        public static event Action<PlayerId, string> AddFriendRequested;

        public static void RaiseAddFriendRequested(PlayerId target, string targetNickname) =>
            AddFriendRequested?.Invoke(target, targetNickname);
    }
}

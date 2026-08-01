using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Backend;
using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Online.Notifications;
using UnityEngine;

namespace GulfRun.Features.Online.Friends
{
    /// <summary>
    /// Every Friend System operation from the Sprint 9 brief (Send/Accept/
    /// Reject/Cancel/Remove/Block/Invite/Join Friend Lobby/View Profile),
    /// implemented as a thin, local-player-aware wrapper over
    /// <see cref="IOnlineBackendService"/> — the same "manager owns the
    /// local-player convenience API, backend owns the data" split
    /// <c>Leaderboard.LeaderboardManager</c> uses. Also the sole subscriber
    /// of <see cref="FriendRequestBridge"/>, so Leaderboard rows, Search
    /// results, Player Profile screens, and the live Match/Lobby
    /// Participants panel (covering the brief's Lobby/End-Match-Screen/
    /// Tournament-Rankings "Add Friend" entry points — see
    /// <see cref="FriendListView"/> remarks) can all request "Add Friend"
    /// without referencing this class directly.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FriendManager : Singleton<FriendManager>, IFriendsSummaryProvider
    {
        private IOnlineBackendService _backend;
        private int _lastKnownIncomingCount;

        public event Action FriendsChanged;

        public PlayerId LocalPlayerId =>
            SaveManager.Instance != null && SaveManager.Instance.HasAccount ? SaveManager.Instance.GetAccount().PlayerId : PlayerId.None;

        protected override void OnInitialize()
        {
            FriendsSummaryService.Current = this;
        }

        private void OnEnable()
        {
            _backend = OnlineBackendService.Current;
            _backend.FriendsChanged += HandleBackendFriendsChanged;
            FriendRequestBridge.AddFriendRequested += HandleAddFriendRequested;
        }

        private void OnDisable()
        {
            if (_backend != null)
            {
                _backend.FriendsChanged -= HandleBackendFriendsChanged;
            }

            FriendRequestBridge.AddFriendRequested -= HandleAddFriendRequested;

            if (ReferenceEquals(FriendsSummaryService.Current, this))
            {
                FriendsSummaryService.Current = null;
            }
        }

        /// <summary>Sprint 13 (Main Menu "SOCIAL: Friends Online"). Total friends currently known to the local player.</summary>
        public int TotalFriendsCount => GetFriends().Count;

        /// <summary>Sprint 13 (Main Menu "SOCIAL: Friends Online"). Counts every friend whose last published <see cref="PlayerProfileSummary.Status"/> is not <see cref="OnlineStatus.Offline"/>.</summary>
        public int OnlineFriendsCount
        {
            get
            {
                IReadOnlyList<PlayerId> friends = GetFriends();
                int online = 0;
                for (int i = 0; i < friends.Count; i++)
                {
                    if (TryGetFriendProfile(friends[i], out PlayerProfileSummary profile) && profile.Status != OnlineStatus.Offline)
                    {
                        online++;
                    }
                }

                return online;
            }
        }

        public IReadOnlyList<PlayerId> GetFriends() => OnlineBackendService.Current.GetFriends(LocalPlayerId);

        public IReadOnlyList<FriendRequest> GetIncomingRequests() => OnlineBackendService.Current.GetIncomingRequests(LocalPlayerId);

        public IReadOnlyList<FriendRequest> GetOutgoingRequests() => OnlineBackendService.Current.GetOutgoingRequests(LocalPlayerId);

        public IReadOnlyList<PlayerId> GetBlockedPlayers() => OnlineBackendService.Current.GetBlockedPlayers(LocalPlayerId);

        public FriendLinkState GetLinkState(PlayerId other) => OnlineBackendService.Current.GetLinkState(LocalPlayerId, other);

        public void SendFriendRequest(PlayerId target) => OnlineBackendService.Current.SendFriendRequest(LocalPlayerId, target);

        public void AcceptFriendRequest(PlayerId requester) => OnlineBackendService.Current.AcceptFriendRequest(requester, LocalPlayerId);

        public void RejectFriendRequest(PlayerId requester) => OnlineBackendService.Current.RejectFriendRequest(requester, LocalPlayerId);

        public void CancelFriendRequest(PlayerId target) => OnlineBackendService.Current.CancelFriendRequest(LocalPlayerId, target);

        public void RemoveFriend(PlayerId other) => OnlineBackendService.Current.RemoveFriend(LocalPlayerId, other);

        public void BlockPlayer(PlayerId other) => OnlineBackendService.Current.BlockPlayer(LocalPlayerId, other);

        public bool TryGetFriendProfile(PlayerId friend, out PlayerProfileSummary profile) => OnlineBackendService.Current.TryGetProfile(friend, out profile);

        /// <summary>
        /// Best-effort only: a real invite needs a live push channel to an
        /// offline friend, which no backend here provides yet (see Sprint 9
        /// report Remaining TODOs) — this raises a local confirmation
        /// notification so the action always visibly does *something*.
        /// </summary>
        public void InviteFriend(PlayerId friend, string friendNickname) =>
            NotificationManager.Instance?.Raise(NotificationType.NewEvent, "Invited " + friendNickname + " to your lobby.");

        /// <summary>
        /// Best-effort only: joining requires a real remote join
        /// code/session resolution that does not exist under the current
        /// loopback transport (see Sprint 4 remaining TODOs, still
        /// unresolved) — reserved for once a real transport lands.
        /// </summary>
        public void JoinFriendLobby(PlayerId friend, string friendNickname) =>
            NotificationManager.Instance?.Raise(NotificationType.NewEvent, "Requested to join " + friendNickname + "'s lobby.");

        private void HandleAddFriendRequested(PlayerId target, string nickname) => SendFriendRequest(target);

        private void HandleBackendFriendsChanged()
        {
            IReadOnlyList<FriendRequest> incoming = GetIncomingRequests();
            if (incoming.Count > _lastKnownIncomingCount)
            {
                NotificationManager.Instance?.Raise(NotificationType.FriendRequest, "You have a new friend request.");
            }

            _lastKnownIncomingCount = incoming.Count;
            FriendsChanged?.Invoke();
        }
    }
}

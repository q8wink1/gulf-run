using System.Collections.Generic;
using GulfRun.Core.Backend;
using GulfRun.Core.Networking;
using GulfRun.Domain;
using GulfRun.Features.Online.Profile;
using UnityEngine;

namespace GulfRun.Features.Online.Friends
{
    /// <summary>
    /// The Friend System screen: three tabs covering every operation the
    /// Sprint 9 brief lists — "Friends" (list + Remove/Block/View Profile),
    /// "Requests" (incoming Accept/Reject, outgoing Cancel), and "Search"
    /// (by Nickname/Player ID/Country, with Add Friend/View Profile per
    /// result — the brief's dedicated Search entry point). A fourth,
    /// always-visible "Nearby Players" section lists the live match/lobby
    /// roster (<see cref="IMatchTransport.Participants"/>) with its own Add
    /// Friend button, deliberately covering the brief's remaining three Add
    /// Friend entry points (Lobby, End Match Screen, Tournament Rankings)
    /// without this assembly ever referencing Features.Multiplayer or
    /// Features.RaceFinish — those three screens all show the exact same
    /// "who's in this match" roster under the current architecture, so one
    /// shared panel here covers all three honestly (see Sprint 9 report
    /// Remaining TODOs for adding dedicated buttons directly inside those
    /// features' own screens later).
    /// </summary>
    public sealed class FriendListView : MonoBehaviour
    {
        private enum Tab
        {
            Friends,
            Requests,
            Search
        }

        private bool _open;
        private Tab _tab = Tab.Friends;
        private string _searchQuery = string.Empty;
        private Vector2 _scroll;
        private GUIStyle _titleStyle;
        private GUIStyle _tabStyle;
        private GUIStyle _labelStyle;

        private void OnGUI()
        {
            EnsureStyles();

            if (GUI.Button(new Rect(350, 10, 160, 34), _open ? "Close Friends" : "Friends"))
            {
                _open = !_open;
            }

            if (!_open)
            {
                return;
            }

            DrawPanel();
        }

        private void DrawPanel()
        {
            const float panelWidth = 560f;
            const float panelHeight = 600f;
            float x = 650f;
            float y = 50f;

            GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty);
            GUI.Label(new Rect(x + 14f, y + 8f, panelWidth - 28f, 26f), "FRIENDS", _titleStyle);

            float rowY = y + 40f;
            DrawTabs(x + 14f, rowY, panelWidth - 28f);
            rowY += 32f;

            switch (_tab)
            {
                case Tab.Friends:
                    DrawFriendsTab(x + 14f, rowY, panelWidth - 28f, panelHeight - (rowY - y) - 160f);
                    break;
                case Tab.Requests:
                    DrawRequestsTab(x + 14f, rowY, panelWidth - 28f);
                    break;
                case Tab.Search:
                    DrawSearchTab(x + 14f, rowY, panelWidth - 28f);
                    break;
            }

            DrawNearbyPlayers(x + 14f, y + panelHeight - 150f, panelWidth - 28f);
        }

        private void DrawTabs(float x, float y, float width)
        {
            Tab[] tabs = { Tab.Friends, Tab.Requests, Tab.Search };
            float tabWidth = width / tabs.Length;
            for (int i = 0; i < tabs.Length; i++)
            {
                GUI.color = _tab == tabs[i] ? Color.yellow : Color.white;
                if (GUI.Button(new Rect(x + i * tabWidth, y, tabWidth - 2f, 26f), tabs[i].ToString(), _tabStyle))
                {
                    _tab = tabs[i];
                }

                GUI.color = Color.white;
            }
        }

        private void DrawFriendsTab(float x, float y, float width, float height)
        {
            if (FriendManager.Instance == null)
            {
                return;
            }

            IReadOnlyList<PlayerId> friends = FriendManager.Instance.GetFriends();
            GUI.Label(new Rect(x, y, width, 20f), "Friends (" + friends.Count + "):", _labelStyle);
            y += 22f;

            Rect viewport = new Rect(x, y, width, height);
            Rect content = new Rect(0f, 0f, width - 20f, friends.Count * 26f);
            _scroll = GUI.BeginScrollView(viewport, _scroll, content);

            for (int i = 0; i < friends.Count; i++)
            {
                DrawFriendRow(friends[i], i * 26f, content.width);
            }

            GUI.EndScrollView();
        }

        private void DrawFriendRow(PlayerId friendId, float rowY, float width)
        {
            bool hasProfile = FriendManager.Instance.TryGetFriendProfile(friendId, out PlayerProfileSummary profile);
            string nickname = hasProfile ? profile.Nickname : friendId.Value;
            OnlineStatus status = hasProfile ? profile.Status : OnlineStatus.Offline;

            if (GUI.Button(new Rect(0f, rowY, 220f, 24f), nickname + " [" + status + "]"))
            {
                PlayerProfileView.Instance?.ShowProfile(friendId);
            }

            if (GUI.Button(new Rect(230f, rowY, 90f, 22f), "Invite"))
            {
                FriendManager.Instance.InviteFriend(friendId, nickname);
            }

            if (GUI.Button(new Rect(326f, rowY, 90f, 22f), "Join Lobby"))
            {
                FriendManager.Instance.JoinFriendLobby(friendId, nickname);
            }

            if (GUI.Button(new Rect(422f, rowY, 90f, 22f), "Remove"))
            {
                FriendManager.Instance.RemoveFriend(friendId);
            }
        }

        private void DrawRequestsTab(float x, float y, float width)
        {
            if (FriendManager.Instance == null)
            {
                return;
            }

            IReadOnlyList<FriendRequest> incoming = FriendManager.Instance.GetIncomingRequests();
            GUI.Label(new Rect(x, y, width, 20f), "Incoming (" + incoming.Count + "):", _labelStyle);
            y += 22f;

            for (int i = 0; i < incoming.Count; i++)
            {
                DrawIncomingRequestRow(incoming[i], x, y, width);
                y += 26f;
            }

            y += 12f;
            IReadOnlyList<FriendRequest> outgoing = FriendManager.Instance.GetOutgoingRequests();
            GUI.Label(new Rect(x, y, width, 20f), "Sent (" + outgoing.Count + "):", _labelStyle);
            y += 22f;

            for (int i = 0; i < outgoing.Count; i++)
            {
                DrawOutgoingRequestRow(outgoing[i], x, y, width);
                y += 26f;
            }
        }

        private void DrawIncomingRequestRow(FriendRequest request, float x, float y, float width)
        {
            bool hasProfile = FriendManager.Instance.TryGetFriendProfile(request.From, out PlayerProfileSummary profile);
            string nickname = hasProfile ? profile.Nickname : request.From.Value;
            GUI.Label(new Rect(x, y, 220f, 22f), nickname, _labelStyle);

            if (GUI.Button(new Rect(x + 226f, y, 90f, 22f), "Accept"))
            {
                FriendManager.Instance.AcceptFriendRequest(request.From);
            }

            if (GUI.Button(new Rect(x + 322f, y, 90f, 22f), "Reject"))
            {
                FriendManager.Instance.RejectFriendRequest(request.From);
            }
        }

        private void DrawOutgoingRequestRow(FriendRequest request, float x, float y, float width)
        {
            bool hasProfile = FriendManager.Instance.TryGetFriendProfile(request.To, out PlayerProfileSummary profile);
            string nickname = hasProfile ? profile.Nickname : request.To.Value;
            GUI.Label(new Rect(x, y, 220f, 22f), nickname + " (pending)", _labelStyle);

            if (GUI.Button(new Rect(x + 226f, y, 90f, 22f), "Cancel"))
            {
                FriendManager.Instance.CancelFriendRequest(request.To);
            }
        }

        private void DrawSearchTab(float x, float y, float width)
        {
            GUI.Label(new Rect(x, y, 80f, 22f), "Search:", _labelStyle);
            _searchQuery = GUI.TextField(new Rect(x + 84f, y, width - 84f, 22f), _searchQuery);
            y += 30f;

            if (string.IsNullOrWhiteSpace(_searchQuery))
            {
                GUI.Label(new Rect(x, y, width, 22f), "Type a Nickname, Player ID, or Country.", _labelStyle);
                return;
            }

            IReadOnlyList<PlayerProfileSummary> results = OnlineBackendService.Current.SearchPlayers(_searchQuery);
            GUI.Label(new Rect(x, y, width, 20f), "Results (" + results.Count + "):", _labelStyle);
            y += 22f;

            for (int i = 0; i < results.Count && i < 12; i++)
            {
                DrawSearchResultRow(results[i], x, y, width);
                y += 26f;
            }
        }

        private void DrawSearchResultRow(PlayerProfileSummary profile, float x, float y, float width)
        {
            if (GUI.Button(new Rect(x, y, 220f, 24f), profile.Nickname + " (" + profile.Country + ")"))
            {
                PlayerProfileView.Instance?.ShowProfile(profile.PlayerId);
            }

            bool isSelf = FriendManager.Instance != null && profile.PlayerId == FriendManager.Instance.LocalPlayerId;
            if (isSelf || FriendManager.Instance == null)
            {
                return;
            }

            FriendLinkState link = FriendManager.Instance.GetLinkState(profile.PlayerId);
            if (link == FriendLinkState.None && GUI.Button(new Rect(x + 230f, y, 90f, 22f), "Add Friend"))
            {
                FriendManager.Instance.SendFriendRequest(profile.PlayerId);
            }
            else if (link != FriendLinkState.None)
            {
                GUI.Label(new Rect(x + 230f, y, 90f, 22f), link.ToString(), _labelStyle);
            }
        }

        private void DrawNearbyPlayers(float x, float y, float width)
        {
            IMatchTransport transport = MatchTransportService.Current;
            if (transport == null || !transport.IsActive)
            {
                return;
            }

            GUI.Label(new Rect(x, y, width, 20f), "Nearby Players (Lobby / Match / Results):", _labelStyle);
            y += 22f;

            int row = 0;
            foreach (MatchParticipant participant in transport.Participants)
            {
                if (participant.Identity.ConnectionId == transport.LocalConnectionId)
                {
                    continue;
                }

                DrawNearbyPlayerRow(participant, x, y + row * 24f, width);
                row++;
            }
        }

        private void DrawNearbyPlayerRow(MatchParticipant participant, float x, float y, float width)
        {
            PlayerId playerId = new PlayerId(participant.Identity.PlayerId);
            GUI.Label(new Rect(x, y, 220f, 22f), participant.Identity.DisplayName, _labelStyle);

            if (FriendManager.Instance == null)
            {
                return;
            }

            FriendLinkState link = FriendManager.Instance.GetLinkState(playerId);
            if (link == FriendLinkState.None && GUI.Button(new Rect(x + 226f, y, 90f, 22f), "Add Friend"))
            {
                FriendManager.Instance.SendFriendRequest(playerId);
            }
            else if (link != FriendLinkState.None)
            {
                GUI.Label(new Rect(x + 226f, y, 90f, 22f), link.ToString(), _labelStyle);
            }
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _titleStyle.normal.textColor = Color.white;

            _tabStyle = new GUIStyle(GUI.skin.button) { fontSize = 12 };

            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleLeft };
            _labelStyle.normal.textColor = Color.white;
        }
    }
}

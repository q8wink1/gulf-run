using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Matchmaking.UI;
using UnityEngine;

namespace GulfRun.Features.Matchmaking.Lobby
{
    /// <summary>
    /// Sprint 14 Matchmaking owner chrome: Room Code Copy/Share, Invite
    /// Friends, Bot Fill toggle (Private Room only), Start Match.
    /// </summary>
    public sealed class OwnerControlsView : MonoBehaviour
    {
        private LobbyButtonPressAnimator _copyAnim;
        private LobbyButtonPressAnimator _inviteAnim;
        private LobbyButtonPressAnimator _botAnim;
        private LobbyButtonPressAnimator _startAnim;
        private string _toast = string.Empty;
        private double _toastUntil;

        private void OnGUI()
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby == null || !lobby.IsInMatch)
            {
                return;
            }

            float width = Mathf.Min(520f, Screen.width - 24f);
            float height = lobby.IsHost ? 110f : 56f;
            float x = (Screen.width - width) * 0.5f;
            float y = 40f;

            PreRaceLobbyTheme.DrawPanel(new Rect(x, y, width, height));
            string title = lobby.IsPrivateRoom ? "Private Room" : "Quick Play Lobby";
            GUI.Label(new Rect(x + 12f, y + 8f, width - 24f, 22f), title + "  •  Code " + lobby.LocalRoomCode, PreRaceLobbyTheme.Header);

            if (!lobby.IsHost)
            {
                GUI.Label(new Rect(x + 12f, y + 32f, width - 24f, 18f), "Waiting for Room Owner…", PreRaceLobbyTheme.Muted);
                return;
            }

            Rect copy = _copyAnim.Apply(new Rect(x + 12f, y + 36f, 100f, 28f), 2f);
            if (GUI.Button(copy, "Copy", PreRaceLobbyTheme.PanelButton))
            {
                _copyAnim.NotifyPressed();
                GUIUtility.systemCopyBuffer = lobby.LocalRoomCode.Value;
                ShowToast("Room code copied");
            }

            if (GUI.Button(new Rect(x + 120f, y + 36f, 100f, 28f), "Share", PreRaceLobbyTheme.PanelButton))
            {
                GUIUtility.systemCopyBuffer = "Join my GulfRun room: " + lobby.LocalRoomCode.Value;
                ShowToast("Share text copied");
            }

            Rect invite = _inviteAnim.Apply(new Rect(x + 228f, y + 36f, 120f, 28f), 2f);
            if (GUI.Button(invite, "Invite", PreRaceLobbyTheme.PanelButton))
            {
                _inviteAnim.NotifyPressed();
                InviteFirstOnlineFriend();
            }

            if (lobby.IsPrivateRoom)
            {
                Rect bot = _botAnim.Apply(new Rect(x + 356f, y + 36f, 150f, 28f), 2f);
                string botLabel = lobby.BotFillEnabled ? "Bots: ON" : "Bots: OFF";
                if (GUI.Button(bot, botLabel, PreRaceLobbyTheme.PanelButton))
                {
                    _botAnim.NotifyPressed();
                    lobby.SetBotFillEnabled(!lobby.BotFillEnabled);
                }
            }

            bool canStart = lobby.AllPlayersReady;
            Rect start = _startAnim.Apply(new Rect(x + 12f, y + 72f, 160f, 28f), 2f);
            Color previous = GUI.color;
            GUI.color = canStart ? PreRaceLobbyTheme.Gold : PreRaceLobbyTheme.SandDark;
            if (GUI.Button(start, "Start Match", PreRaceLobbyTheme.GoldButton) && canStart)
            {
                _startAnim.NotifyPressed();
                lobby.RequestHostStart();
            }

            GUI.color = previous;
            GUI.Label(new Rect(x + 184f, y + 76f, width - 200f, 20f),
                lobby.LobbyPlayerCount + "/" + lobby.RequiredPlayerCount + " players  •  min " + lobby.MinimumPlayerCount +
                (canStart ? "  •  All Ready" : "  •  Waiting for Ready"),
                PreRaceLobbyTheme.Muted);

            if (!string.IsNullOrEmpty(_toast) && Time.timeAsDouble < _toastUntil)
            {
                GUI.Label(new Rect(x + 12f, y + height + 4f, width, 18f), _toast, PreRaceLobbyTheme.Muted);
            }
        }

        private void InviteFirstOnlineFriend()
        {
            IFriendsSummaryProvider friends = FriendsSummaryService.Current;
            if (friends == null)
            {
                ShowToast("Friends unavailable");
                return;
            }

            var online = friends.GetOnlineFriends();
            if (online == null || online.Count == 0)
            {
                ShowToast("No online friends to invite");
                return;
            }

            friends.InviteFriend(online[0]);
            ShowToast("Invited " + friends.GetFriendDisplayName(online[0]));
        }

        private void ShowToast(string message)
        {
            _toast = message;
            _toastUntil = Time.timeAsDouble + 2.2d;
        }
    }
}

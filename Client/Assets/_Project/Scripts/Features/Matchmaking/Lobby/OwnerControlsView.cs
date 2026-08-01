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

            // Reference layout: 1920×1080; keep host chrome inside the safe margins
            // at 1080p / 1440p / 4K via width clamp + wrapped action row.
            float width = Mathf.Clamp(Screen.width - 48f, 320f, 520f);
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

            float btnY = y + 36f;
            float cursorX = x + 12f;
            float gap = 8f;
            float maxX = x + width - 12f;

            Rect copy = _copyAnim.Apply(new Rect(cursorX, btnY, 100f, 28f), 2f);
            if (GUI.Button(copy, "Copy", PreRaceLobbyTheme.PanelButton))
            {
                _copyAnim.NotifyPressed();
                GUIUtility.systemCopyBuffer = lobby.LocalRoomCode.Value;
                ShowToast("Room code copied");
            }

            cursorX += 100f + gap;
            if (GUI.Button(new Rect(cursorX, btnY, 100f, 28f), "Share", PreRaceLobbyTheme.PanelButton))
            {
                GUIUtility.systemCopyBuffer = "Join my GulfRun room: " + lobby.LocalRoomCode.Value;
                ShowToast("Share text copied");
            }

            cursorX += 100f + gap;
            float inviteW = Mathf.Min(120f, maxX - cursorX);
            Rect invite = _inviteAnim.Apply(new Rect(cursorX, btnY, inviteW, 28f), 2f);
            if (GUI.Button(invite, "Invite", PreRaceLobbyTheme.PanelButton))
            {
                _inviteAnim.NotifyPressed();
                InviteFirstOnlineFriend();
            }

            cursorX += inviteW + gap;
            if (lobby.IsPrivateRoom && cursorX + 90f <= maxX)
            {
                float botW = Mathf.Min(150f, maxX - cursorX);
                Rect bot = _botAnim.Apply(new Rect(cursorX, btnY, botW, 28f), 2f);
                string botLabel = lobby.BotFillEnabled ? "Bots: ON" : "Bots: OFF";
                if (GUI.Button(bot, botLabel, PreRaceLobbyTheme.PanelButton))
                {
                    _botAnim.NotifyPressed();
                    lobby.SetBotFillEnabled(!lobby.BotFillEnabled);
                }
            }
            else if (lobby.IsPrivateRoom)
            {
                // Second row when the top action strip is too narrow for Bots.
                Rect bot = _botAnim.Apply(new Rect(x + 12f + 168f, y + 72f, 140f, 28f), 2f);
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
            float statusX = x + 184f;
            float statusW = width - 200f;
            if (lobby.IsPrivateRoom && cursorX + 90f > maxX)
            {
                statusX = x + 320f;
                statusW = Mathf.Max(120f, width - 332f);
            }

            GUI.Label(new Rect(statusX, y + 76f, statusW, 20f),
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

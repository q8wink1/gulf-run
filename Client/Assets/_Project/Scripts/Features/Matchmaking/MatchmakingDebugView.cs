using GulfRun.Core.Services;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Matchmaking
{
    /// <summary>
    /// Sprint 14 Matchmaking debug: Room ID, Match ID, Player/Bot Count, Ping,
    /// Connection State. <c>panelX: 4960</c> is the next free +450 slot after
    /// RaceHudDebugView at 4510.
    /// </summary>
    public sealed class MatchmakingDebugView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private bool showOnScreenDebug = true;
        [SerializeField] private int panelX = 4960;

        private void OnGUI()
        {
            // Keep production Lobby free of the Matchmaking debug strip
            // (panelX 4960 was off-canvas at 1080p but still an active drawer).
            if (!showOnScreenDebug || PersistentUiScope.IsLobbyActive)
            {
                return;
            }

            int y = 10;
            const int lineHeight = 18;
            const int width = 440;

            void Line(string text)
            {
                GUI.Label(new Rect(panelX, y, width, lineHeight), text);
                y += lineHeight;
            }

            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            Line("[Matchmaking / Pre-Race Lobby]");
            if (lobby == null)
            {
                Line("Lobby provider: n/a");
                return;
            }

            Line($"InMatch:{lobby.IsInMatch}  Matchmaking:{lobby.IsMatchmaking}  Host:{lobby.IsHost}  Private:{lobby.IsPrivateRoom}");
            Line($"Room:{lobby.LocalRoomCode}  MatchId:{lobby.MatchId}");
            Line($"Players:{lobby.LobbyPlayerCount}/{lobby.RequiredPlayerCount}  Phase:{lobby.LobbyPhase}  Countdown:{lobby.AutoStartCountdownSecondsRemaining}");

            int bots = 0;
            foreach (MatchParticipant p in lobby.Participants)
            {
                if (lobby.IsBot(p.Identity.ConnectionId))
                {
                    bots++;
                }

                float ping = lobby.GetPingMilliseconds(p.Identity.ConnectionId);
                ConnectionQuality q = lobby.GetConnectionQuality(p.Identity.ConnectionId);
                Line($"  [{p.Identity.ConnectionId}] {p.Identity.DisplayName} Ready:{p.Ready} Host:{p.IsHost} Q:{q} Ping:{ping:F0}ms Bot:{lobby.IsBot(p.Identity.ConnectionId)}");
            }

            Line($"Bot Count: {bots}  BotFill:{lobby.BotFillEnabled}");
        }
#endif
    }
}

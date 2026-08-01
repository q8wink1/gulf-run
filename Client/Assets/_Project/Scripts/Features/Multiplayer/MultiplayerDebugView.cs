using GulfRun.Core.Networking;
using GulfRun.Domain;
using GulfRun.Features.Multiplayer.Bots;
using GulfRun.Features.Multiplayer.Connection;
using GulfRun.Features.Multiplayer.Identification;
using GulfRun.Features.Multiplayer.Lobby;
using GulfRun.Features.Multiplayer.Match;
using GulfRun.Features.Multiplayer.Session;
using UnityEngine;

namespace GulfRun.Features.Multiplayer
{
    /// <summary>
    /// Debug overlay: Connected Players, Ping, Connection State/Quality,
    /// Player IDs, Match State — plus a handful of Editor/dev-build-only
    /// buttons (Create/Ready/Simulate Remote Join/Leave, and Sprint 15's
    /// Create Private Room/Toggle Bot Fill/Simulate Host Leave) so the whole
    /// Match Flow + Matchmaking/Room/Bot Fill/Host Migration surface can be
    /// exercised end-to-end with no menu UI needed (same OnGUI-placeholder
    /// approach as CountdownView/RunnerDebugView).
    /// </summary>
    public sealed class MultiplayerDebugView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private bool showOnScreenDebug = true;
        [SerializeField] private int panelX = 460;

        private static readonly GulfCountry[] _debugBotCountries = (GulfCountry[])System.Enum.GetValues(typeof(GulfCountry));
        private static int _simulatedBotCount;

        private void OnGUI()
        {
            if (!showOnScreenDebug)
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

            SessionManager session = SessionManager.Instance;
            LobbyManager lobby = LobbyManager.Instance;
            MatchManager match = MatchManager.Instance;
            ConnectionManager connection = ConnectionManager.Instance;
            BotFillController bots = BotFillController.Instance;

            Line($"[Multiplayer] In Match: {session != null && session.IsInMatch}  Host: {session != null && session.IsHost}");
            Line($"Mode: {(session != null ? session.CreationMode.ToString() : "n/a")}  Room Code: {(session != null ? session.LocalRoomCode.ToString() : "—")}  Match Id: {(session != null ? session.MatchId : string.Empty)}");
            Line($"Match State: {(match != null ? match.State.ToString() : "n/a")}  Bot Fill: {(bots != null && bots.Enabled)}  Bots: {(bots != null ? bots.BotCount : 0)}");

            if (match != null && match.State == MatchState.Countdown)
            {
                Line($"Countdown: {match.CountdownSecondsRemaining}");
            }

            if (lobby != null)
            {
                Line($"Player Count: {lobby.PlayerCount}");
                foreach (MatchParticipant participant in lobby.Participants)
                {
                    int connectionId = participant.Identity.ConnectionId;
                    float pingMs = connection != null ? connection.PingSecondsFor(connectionId) * 1000f : 0f;
                    ConnectionQuality quality = connection != null ? connection.GetQuality(connectionId) : ConnectionQuality.Disconnected;
                    bool isBot = bots != null && bots.IsBot(connectionId);
                    Line($"  [{connectionId}]{(isBot ? " (BOT)" : string.Empty)} {participant.Identity.DisplayName} " +
                         $"Host:{participant.IsHost} Ready:{participant.Ready} Conn:{participant.Connection} Quality:{quality} Ping:{pingMs:F0}ms");
                }
            }

            y += 6;
            DrawControls(session, bots, ref y, width);
        }

        private void DrawControls(SessionManager session, BotFillController bots, ref int y, int width)
        {
            if (session == null)
            {
                return;
            }

            const int buttonHeight = 24;
            const int buttonWidth = 190;

            if (!session.IsInMatch)
            {
                if (GUI.Button(new Rect(panelX, y, buttonWidth, buttonHeight), "Create Match (Host)"))
                {
                    session.CreateMatch("Host");
                }

                if (GUI.Button(new Rect(panelX + buttonWidth + 8, y, buttonWidth, buttonHeight), "Create Private Room"))
                {
                    session.CreatePrivateRoom("Host");
                }

                y += buttonHeight + 4;
                return;
            }

            if (GUI.Button(new Rect(panelX, y, buttonWidth, buttonHeight), "Ready Up"))
            {
                session.SetLocalReady(PlayerReadyState.Ready);
            }

            if (GUI.Button(new Rect(panelX + buttonWidth + 8, y, buttonWidth, buttonHeight), "Simulate Remote Join"))
            {
                SimulateRemoteJoinAndReady();
            }

            y += buttonHeight + 4;

            if (session.IsPrivateRoom && GUI.Button(new Rect(panelX, y, buttonWidth, buttonHeight), (bots != null && bots.Enabled ? "Bot Fill: ON" : "Bot Fill: OFF")))
            {
                session.SetBotFillEnabled(!(bots != null && bots.Enabled));
            }

            if (GUI.Button(new Rect(panelX + buttonWidth + 8, y, buttonWidth, buttonHeight), "Simulate Host Leave"))
            {
                SimulateHostLeave(session);
            }

            y += buttonHeight + 4;

            if (GUI.Button(new Rect(panelX, y, buttonWidth, buttonHeight), "Leave Match"))
            {
                session.LeaveMatch();
            }

            y += buttonHeight + 4;
            _ = width;
        }

        private static void SimulateRemoteJoinAndReady()
        {
            if (MatchTransportService.Current is LocalLoopbackTransport loopback)
            {
                // Cycle through every GulfCountry so repeated test joins exercise
                // the Victory Ceremony's national-flag-per-player display (Sprint
                // 7 addendum) with visibly distinct flags, not four identical ones.
                GulfCountry botCountry = _debugBotCountries[_simulatedBotCount % _debugBotCountries.Length];
                _simulatedBotCount++;

                MatchParticipant bot = loopback.SimulateRemoteJoin(LocalPlayerIdentity.CreateLocal("Bot", botCountry));
                loopback.SimulateRemoteReady(bot.Identity.ConnectionId, PlayerReadyState.Ready);
            }
        }

        /// <summary>Sprint 15 (Network "Host migration ready" — see <see cref="Connection.HostMigrationController"/> remarks). Removes whichever roster entry currently holds Host exactly the way a real remote host disconnect would, so the promotion algorithm can be observed end-to-end today.</summary>
        private static void SimulateHostLeave(SessionManager session)
        {
            if (!(MatchTransportService.Current is LocalLoopbackTransport loopback) || LobbyManager.Instance == null)
            {
                return;
            }

            foreach (MatchParticipant participant in LobbyManager.Instance.Participants)
            {
                if (participant.IsHost)
                {
                    loopback.SimulateRemoteLeave(participant.Identity.ConnectionId, DisconnectReason.HostLeft);
                    break;
                }
            }

            _ = session;
        }
#endif
    }
}

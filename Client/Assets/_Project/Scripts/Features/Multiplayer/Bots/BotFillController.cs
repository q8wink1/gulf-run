using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Domain;
using GulfRun.Features.Multiplayer.Configuration;
using GulfRun.Features.Multiplayer.Identification;
using GulfRun.Features.Multiplayer.Lobby;
using UnityEngine;

namespace GulfRun.Features.Multiplayer.Bots
{
    /// <summary>
    /// Sprint 15 "BOT SETTINGS": Room Owner ON/OFF toggle for filling empty
    /// Private Room slots with bots. Deliberately Private-Room-only by
    /// convention (see <c>SessionManager.SetBotFillEnabled</c>/
    /// <see cref="Domain.MatchCreationMode"/>) — P017 §12 explicitly lists
    /// "Bot Filling" under public matchmaking's "Explicitly Not Defined"
    /// items, so Quick Play never surfaces this toggle.
    ///
    /// Bots only exist under the honest <see cref="LocalLoopbackTransport"/>
    /// today (there is no AI opponent controller anywhere in this project —
    /// a bot is simply a <see cref="MatchParticipant"/> row that is always
    /// Ready and never sends real input/snapshots, the same "fills the
    /// roster, does nothing else yet" honesty as every other placeholder
    /// system in this project). Reuses the exact
    /// <see cref="LocalLoopbackTransport.SimulateRemoteJoin"/>/
    /// <see cref="LocalLoopbackTransport.SimulateRemoteReady"/> test hooks
    /// <c>MultiplayerDebugView</c> already uses — the only difference is
    /// *why* they are being called (an owner's deliberate choice instead of
    /// a developer's manual test click).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BotFillController : Singleton<BotFillController>
    {
        [SerializeField] private NetworkSyncConfig config;

        private static readonly GulfCountry[] BotCountries = (GulfCountry[])System.Enum.GetValues(typeof(GulfCountry));

        private readonly HashSet<int> _botConnectionIds = new HashSet<int>();
        private int _nextBotNumber = 1;

        public bool Enabled { get; private set; }

        public int BotCount => _botConnectionIds.Count;

        protected override void OnInitialize()
        {
        }

        private void OnEnable()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.LobbyChanged += HandleLobbyChanged;
            }
        }

        private void OnDisable()
        {
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.LobbyChanged -= HandleLobbyChanged;
            }
        }

        public bool IsBot(int connectionId) => _botConnectionIds.Contains(connectionId);

        /// <summary>Sprint 15 (Bot Settings ON/OFF toggle). Turning ON immediately fills any currently-empty slots; turning OFF removes every bot this controller added, leaving only real players (brief: "If OFF: Only real players join").</summary>
        public void SetEnabled(bool enabled)
        {
            if (Enabled == enabled)
            {
                return;
            }

            Enabled = enabled;

            if (Enabled)
            {
                FillEmptySlots();
            }
            else
            {
                RemoveAllBots();
            }
        }

        /// <summary>Called by <c>SessionManager.KickPlayer</c> so a manually-kicked bot is not immediately re-added by a still-enabled Bot Fill on the next roster change.</summary>
        public void ForgetBot(int connectionId) => _botConnectionIds.Remove(connectionId);

        /// <summary>Clears all tracked bots and turns Bot Fill off — called by <c>SessionManager.LeaveMatch</c> so a fresh match never inherits a stale bot roster/toggle state.</summary>
        public void Reset()
        {
            Enabled = false;
            _botConnectionIds.Clear();
            _nextBotNumber = 1;
        }

        private void HandleLobbyChanged()
        {
            if (!Enabled)
            {
                return;
            }

            // A tracked bot leaving (e.g. kicked, or a future real-bot
            // disconnect) frees its slot back up — keep the room full for
            // as long as the toggle stays on, same as a real matchmaking
            // queue would keep searching for the missing seat.
            FillEmptySlots();
        }

        private void FillEmptySlots()
        {
            if (!(MatchTransportService.Current is LocalLoopbackTransport loopback) || !loopback.IsHost)
            {
                return;
            }

            LobbyManager lobby = LobbyManager.Instance;
            if (lobby == null)
            {
                return;
            }

            int maxPlayers = config != null ? config.MaxPlayers : 4;
            while (lobby.PlayerCount < maxPlayers)
            {
                GulfCountry botCountry = BotCountries[_nextBotNumber % BotCountries.Length];
                string botName = "Bot " + _nextBotNumber;
                _nextBotNumber++;

                MatchParticipant bot = loopback.SimulateRemoteJoin(LocalPlayerIdentity.CreateLocal(botName, botCountry));
                loopback.SimulateRemoteReady(bot.Identity.ConnectionId, PlayerReadyState.Ready);
                _botConnectionIds.Add(bot.Identity.ConnectionId);
            }
        }

        private void RemoveAllBots()
        {
            if (!(MatchTransportService.Current is LocalLoopbackTransport loopback))
            {
                return;
            }

            var ids = new List<int>(_botConnectionIds);
            _botConnectionIds.Clear();

            for (int i = 0; i < ids.Count; i++)
            {
                loopback.KickParticipant(ids[i], DisconnectReason.PlayerLeft);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Managers;
using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Multiplayer.Bots;
using GulfRun.Features.Multiplayer.Configuration;
using GulfRun.Features.Multiplayer.Connection;
using GulfRun.Features.Multiplayer.Identification;
using GulfRun.Features.Multiplayer.Lobby;
using GulfRun.Features.Multiplayer.Match;
using UnityEngine;

namespace GulfRun.Features.Multiplayer.Session
{
    /// <summary>
    /// Top-level entry point for the Match Flow: Create/Join Match, Leave
    /// Match, Cancel Matchmaking (Sprint 4), plus Sprint 15's full
    /// Matchmaking/Private Room/Pre-Race Lobby surface — Bot Fill, Kick
    /// Player, host-authoritative Start Match, Quick Chat, and connection
    /// quality readouts. Everything else (Lobby roster, Ready System,
    /// shared countdown, connection health, Bot Fill, Host Migration)
    /// reacts to the <see cref="IMatchTransport"/> events these calls
    /// trigger — this class is the composition root for the Multiplayer
    /// feature, the same role <c>GameLoopController</c> plays for
    /// single-player. Implements <see cref="IMatchLobbySummaryProvider"/> so
    /// the Main Menu's PLAY/Private Room entry points AND the Pre-Race
    /// Lobby scene (Features.Matchmaking) can drive/observe it without
    /// either ever referencing Features.Multiplayer.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SessionManager : Singleton<SessionManager>, IMatchLobbySummaryProvider
    {
        [SerializeField] private NetworkSyncConfig config;

        [Tooltip("Fallback nationality used only if a match is created/joined before Account Creation has run (should not normally happen — Account Creation gates entry into any match). Sprint 8 supersedes the old freely-settable per-session country field: the real value always comes from the permanently-locked SaveManager account once one exists.")]
        [SerializeField] private GulfCountry fallbackPlayerCountry = GulfCountry.SaudiArabia;

        private LobbyManager _lobby;
        private MatchManager _match;
        private BotFillController _bots;
        private readonly System.Random _roomCodeRandom = new System.Random();
        private static readonly MatchParticipant[] EmptyParticipants = Array.Empty<MatchParticipant>();

        /// <summary>Sprint 14 Matchmaking: simulated public-queue search duration under the offline loopback (no real matchmaking service exists yet — see <see cref="MatchmakingEtaEstimator"/>).</summary>
        private const float QuickPlaySearchSeconds = 2.2f;

        private bool _quickPlaySearchPending;
        private float _quickPlaySearchElapsed;
        private string _pendingQuickPlayDisplayName = string.Empty;

        public bool IsInMatch { get; private set; }
        public bool IsMatchmaking { get; private set; }
        public PlayerIdentity LocalIdentity { get; private set; }

        /// <summary>Sprint 15: which entry point created the current match (Quick Play vs Private Room). Resets to <see cref="MatchCreationMode.QuickPlay"/> on <see cref="LeaveMatch"/>.</summary>
        public MatchCreationMode CreationMode { get; private set; } = MatchCreationMode.QuickPlay;

        public bool IsPrivateRoom => IsInMatch && CreationMode == MatchCreationMode.PrivateRoom;

        /// <summary>Sprint 15: computed from the live roster rather than a manually-toggled flag, so a Host Migration promotion (see <c>LobbyManager.PromoteToHost</c>) is reflected here automatically with no extra wiring.</summary>
        public bool IsHost =>
            IsInMatch && _lobby != null && _lobby.TryGetParticipant(LocalIdentity.ConnectionId, out MatchParticipant me) && me.IsHost;

        /// <summary>Sprint 13 (Main Menu "SOCIAL: Room Code"). Rolled fresh every time <see cref="CreateMatchInternal"/> starts hosting; <see cref="RoomCode.None"/> otherwise.</summary>
        public RoomCode LocalRoomCode { get; private set; } = RoomCode.None;

        /// <summary>Sprint 15 (Debug "Match ID"). A locally-generated identifier for the current match — see remarks on <see cref="LocalPlayerIdentity"/> re: "real thing later, honest placeholder now" until a real backend assigns one.</summary>
        public string MatchId { get; private set; } = string.Empty;

        /// <summary>Sprint 13 (Main Menu bottom bar). Live participant count of the current lobby, 0 if not in a match.</summary>
        public int LobbyPlayerCount => IsInMatch && _lobby != null ? _lobby.PlayerCount : 0;

        /// <summary>Sprint 13 (Main Menu bottom bar / <see cref="MatchmakingEtaEstimator"/>). The configured full-lobby size.</summary>
        public int RequiredPlayerCount => config != null ? config.MaxPlayers : 4;

        /// <summary>Sprint 15 (Bot Settings "Minimum players required: 2").</summary>
        public int MinimumPlayerCount => config != null ? config.MinimumPlayersToStart : 2;

        public int LocalConnectionId => IsInMatch ? LocalIdentity.ConnectionId : -1;

        public PlayerReadyState LocalReadyState =>
            IsInMatch && _lobby != null && _lobby.TryGetParticipant(LocalIdentity.ConnectionId, out MatchParticipant me) ? me.Ready : PlayerReadyState.NotReady;

        public bool BotFillEnabled => _bots != null && _bots.Enabled;

        public MatchState LobbyPhase => _match != null ? _match.State : MatchState.Waiting;

        public int AutoStartCountdownSecondsRemaining => _match != null ? _match.CountdownSecondsRemaining : 0;

        public bool AllPlayersReady => IsInMatch && _lobby != null && _lobby.AllRequiredPlayersReady();

        public IReadOnlyCollection<MatchParticipant> Participants => IsInMatch && _lobby != null ? _lobby.Participants : EmptyParticipants;

        public event Action LobbyStateChanged;
        public event Action<int, QuickChatMessage> QuickChatReceived;

        /// <summary>
        /// The local player's permanent nationality (Sprint 8: "Country
        /// becomes permanently linked to the account... cannot be changed
        /// later"). Sourced from the one-time-created <see cref="SaveManager"/>
        /// account; falls back to <see cref="fallbackPlayerCountry"/> only if
        /// no account exists yet, which should never happen once Account
        /// Creation (<c>Features.Character.Account.AccountCreationView</c>)
        /// gates the flow.
        /// </summary>
        public GulfCountry LocalPlayerCountry =>
            SaveManager.Instance != null && SaveManager.Instance.HasAccount
                ? SaveManager.Instance.GetAccount().Country
                : fallbackPlayerCountry;

        protected override void OnInitialize()
        {
            _lobby = GetComponent<LobbyManager>();
            _match = GetComponent<MatchManager>();
            _bots = GetComponent<BotFillController>();
            MatchLobbySummaryService.Current = this;
        }

        private void OnEnable()
        {
            if (_lobby != null)
            {
                _lobby.LobbyChanged += HandleLobbyOrMatchChanged;
            }

            if (_match != null)
            {
                _match.StateChanged += HandleMatchStateChanged;
                _match.CountdownSecondsChanged += HandleCountdownSecondsChanged;
            }

            MatchTransportService.Current.QuickChatReceived += HandleQuickChatReceived;
        }

        private void OnDisable()
        {
            if (_lobby != null)
            {
                _lobby.LobbyChanged -= HandleLobbyOrMatchChanged;
            }

            if (_match != null)
            {
                _match.StateChanged -= HandleMatchStateChanged;
                _match.CountdownSecondsChanged -= HandleCountdownSecondsChanged;
            }

            if (MatchTransportService.Current != null)
            {
                MatchTransportService.Current.QuickChatReceived -= HandleQuickChatReceived;
            }

            if (ReferenceEquals(MatchLobbySummaryService.Current, this))
            {
                MatchLobbySummaryService.Current = null;
            }
        }

        private void Update()
        {
            if (!_quickPlaySearchPending)
            {
                return;
            }

            _quickPlaySearchElapsed += Time.deltaTime;
            if (_quickPlaySearchElapsed < QuickPlaySearchSeconds)
            {
                return;
            }

            _quickPlaySearchPending = false;
            IsMatchmaking = false;
            CreateMatchInternal(_pendingQuickPlayDisplayName, MatchCreationMode.QuickPlay);
            FillQuickPlayOpponents();
            LobbyStateChanged?.Invoke();
        }

        /// <summary>Sprint 13 / Sprint 14 Matchmaking (Main Menu PLAY): begins a short Searching animation, then hosts a Quick Match and auto-fills remaining seats with simulated opponents under loopback.</summary>
        public void StartQuickMatch(string localDisplayName)
        {
            if (IsInMatch || IsMatchmaking)
            {
                return;
            }

            IsMatchmaking = true;
            _quickPlaySearchPending = true;
            _quickPlaySearchElapsed = 0f;
            _pendingQuickPlayDisplayName = string.IsNullOrWhiteSpace(localDisplayName) ? "Player" : localDisplayName;
            LobbyStateChanged?.Invoke();
        }

        /// <summary>Sprint 15 (Private Room "Create Room"): hosts a brand-new invitation-only Private Room, generating the same shared <see cref="RoomCode"/>/<see cref="LobbyManager"/>/<see cref="MatchManager"/> machinery as Quick Play — only <see cref="CreationMode"/> differs, which is what gates Bot Fill visibility (P017 explicitly excludes Bot Filling from public matchmaking).</summary>
        public void CreatePrivateRoom(string localDisplayName) => CreateMatchInternal(localDisplayName, MatchCreationMode.PrivateRoom);

        /// <summary>Creates a brand-new match and becomes its host.</summary>
        public void CreateMatch(string displayName) => CreateMatchInternal(displayName, MatchCreationMode.QuickPlay);

        private void CreateMatchInternal(string displayName, MatchCreationMode mode)
        {
            if (IsInMatch || IsMatchmaking)
            {
                return;
            }

            IsMatchmaking = true;
            LocalIdentity = LocalPlayerIdentity.CreateLocal(displayName, LocalPlayerCountry);

            int maxPlayers = config != null ? config.MaxPlayers : 4;
            MatchTransportService.Current.StartHost(LocalIdentity, maxPlayers);

            LocalRoomCode = RoomCodeGenerator.Generate(_roomCodeRandom);
            MatchId = Guid.NewGuid().ToString("N");
            CreationMode = mode;
            IsInMatch = true;
            IsMatchmaking = false;
        }

        /// <summary>
        /// Joins an existing match as a client (Sprint 15: always a Private
        /// Room join by Room Code — P018 §4 "Room Code / Friend Invitation"
        /// is the only join path defined anywhere in this project's briefs;
        /// public Quick Play never joins by code). Under the default
        /// <see cref="LocalLoopbackTransport"/> there is no real remote host
        /// to reach (see its JoinAsClient doc comment) — this call is wired
        /// end-to-end and ready for a real transport, but will not complete
        /// a join until one is registered via <see cref="MatchTransportService"/>.
        /// </summary>
        public void JoinMatch(string joinCode, string displayName)
        {
            if (IsInMatch || IsMatchmaking)
            {
                return;
            }

            IsMatchmaking = true;
            LocalIdentity = LocalPlayerIdentity.CreateLocal(displayName, LocalPlayerCountry);
            CreationMode = MatchCreationMode.PrivateRoom;
            MatchId = Guid.NewGuid().ToString("N");

            MatchTransportService.Current.JoinAsClient(LocalIdentity, joinCode);

            IsMatchmaking = false;
        }

        /// <summary>Sprint 15 (Private Room "Join Room" by Room Code). Thin naming wrapper over <see cref="JoinMatch"/> for <see cref="IMatchLobbySummaryProvider"/>.</summary>
        public void JoinPrivateRoom(string roomCode, string localDisplayName) => JoinMatch(roomCode, localDisplayName);

        /// <summary>Sprint 13 (Main Menu PLAY button, while matchmaking/in a match): cancels pending matchmaking, or leaves the current match — whichever applies.</summary>
        public void CancelOrLeaveMatch()
        {
            if (IsMatchmaking)
            {
                CancelMatchmaking();
                return;
            }

            LeaveMatch();
        }

        /// <summary>Cancels a pending Create/Join before a connection has been established.</summary>
        public void CancelMatchmaking()
        {
            _quickPlaySearchPending = false;
            _quickPlaySearchElapsed = 0f;
            _pendingQuickPlayDisplayName = string.Empty;
            IsMatchmaking = false;
            LobbyStateChanged?.Invoke();
        }

        /// <summary>Under loopback, Quick Play fills empty seats with always-Ready simulated opponents so the Pre-Race Lobby can demonstrate Match Found → Ready → Auto Start end-to-end without a real matchmaking queue (P017 Bot Filling remains Private-Room-only via <see cref="BotFillController"/>).</summary>
        private void FillQuickPlayOpponents()
        {
            if (!(MatchTransportService.Current is LocalLoopbackTransport loopback) || config == null)
            {
                return;
            }

            int seats = config.MaxPlayers - (_lobby != null ? _lobby.PlayerCount : 1);
            GulfCountry[] countries = (GulfCountry[])Enum.GetValues(typeof(GulfCountry));
            for (int i = 0; i < seats; i++)
            {
                GulfCountry country = countries[(i + 1) % countries.Length];
                MatchParticipant opponent = loopback.SimulateRemoteJoin(LocalPlayerIdentity.CreateLocal("Racer " + (i + 2), country));
                loopback.SimulateRemoteReady(opponent.Identity.ConnectionId, PlayerReadyState.Ready);
            }
        }

        /// <summary>Leaves the current match. Works identically whether the local player is the host or not.</summary>
        public void LeaveMatch()
        {
            if (!IsInMatch)
            {
                return;
            }

            DisconnectReason reason = IsHost ? DisconnectReason.HostLeft : DisconnectReason.PlayerLeft;
            MatchTransportService.Current.Disconnect(reason);

            _bots?.Reset();
            _lobby?.Clear();
            _match?.ResetMatch();

            IsInMatch = false;
            LocalRoomCode = RoomCode.None;
            MatchId = string.Empty;
            CreationMode = MatchCreationMode.QuickPlay;
        }

        /// <summary>Sets the local participant's Ready System state; the host's MatchManager decides when to start the countdown.</summary>
        public void SetLocalReady(PlayerReadyState state)
        {
            if (!IsInMatch)
            {
                return;
            }

            MatchTransportService.Current.SetLocalReadyState(state);
        }

        /// <summary>Sprint 15 (Bot Settings ON/OFF toggle). Host-only; no-op for a non-host caller.</summary>
        public void SetBotFillEnabled(bool enabled)
        {
            if (!IsHost || _bots == null)
            {
                return;
            }

            _bots.SetEnabled(enabled);
        }

        /// <summary>Sprint 15 (Owner Feature "Kick Player"). Host-only; no-op for a non-host caller or when kicking the local player's own connection.</summary>
        public void KickPlayer(int connectionId)
        {
            if (!IsHost || !IsInMatch || connectionId == LocalIdentity.ConnectionId)
            {
                return;
            }

            MatchTransportService.Current.KickParticipant(connectionId, DisconnectReason.Kicked);
            _bots?.ForgetBot(connectionId);
        }

        /// <summary>Sprint 15 (Owner Feature "Start Match"). Host-only manual nudge — a no-op unless <see cref="AllPlayersReady"/> is already true.</summary>
        public void RequestHostStart()
        {
            if (!IsHost)
            {
                return;
            }

            _match?.TryStartCountdown();
        }

        /// <summary>Sprint 15 (Player Cards "BOT" tag).</summary>
        public bool IsBot(int connectionId) => _bots != null && _bots.IsBot(connectionId);

        /// <summary>Sprint 15 (Network "Latency indicator").</summary>
        public float GetPingMilliseconds(int connectionId) =>
            ConnectionManager.Instance != null ? ConnectionManager.Instance.PingSecondsFor(connectionId) * 1000f : 0f;

        /// <summary>Sprint 15 (Player Cards "Connection Quality").</summary>
        public ConnectionQuality GetConnectionQuality(int connectionId) =>
            ConnectionManager.Instance != null ? ConnectionManager.Instance.GetQuality(connectionId) : ConnectionQuality.Disconnected;

        /// <summary>Sprint 15 (Chat "Quick Chat").</summary>
        public void SendQuickChat(QuickChatMessage message)
        {
            if (IsInMatch)
            {
                MatchTransportService.Current.SendQuickChat(message);
            }
        }

        private void HandleLobbyOrMatchChanged() => LobbyStateChanged?.Invoke();

        private void HandleMatchStateChanged(MatchState state) => LobbyStateChanged?.Invoke();

        private void HandleCountdownSecondsChanged(int seconds) => LobbyStateChanged?.Invoke();

        private void HandleQuickChatReceived(int connectionId, QuickChatMessage message) => QuickChatReceived?.Invoke(connectionId, message);
    }
}

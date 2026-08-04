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
using GulfRun.Features.Multiplayer.Matchmaking;
using UnityEngine;

namespace GulfRun.Features.Multiplayer.Session
{
    /// <summary>
    /// Top-level entry point for the Match Flow. Quick Play uses a mock public-room
    /// directory (join fuller rooms first: 3/4 → 2/4 → 1/4) and Host Play opens
    /// Map Voting instead of auto-starting the race countdown.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SessionManager : Singleton<SessionManager>, IMatchLobbySummaryProvider
    {
        [SerializeField] private NetworkSyncConfig config;

        [Tooltip("Fallback nationality used only if a match is created/joined before Account Creation has run.")]
        [SerializeField] private GulfCountry fallbackPlayerCountry = GulfCountry.SaudiArabia;

        private LobbyManager _lobby;
        private MatchManager _match;
        private BotFillController _bots;
        private readonly System.Random _roomCodeRandom = new System.Random();
        private static readonly MatchParticipant[] EmptyParticipants = Array.Empty<MatchParticipant>();
        private readonly MockPublicRoomDirectory _publicRooms = new MockPublicRoomDirectory();

        private const float QuickPlaySearchSeconds = 2.2f;
        private const float RemoteReadyStaggerSeconds = 1.1f;
        private const float GradualFillSeconds = 1.4f;
        private const float KickRefillSeconds = 1.6f;
        private const float MapVoteDurationSeconds = 12f;

        private bool _quickPlaySearchPending;
        private float _quickPlaySearchElapsed;
        private string _pendingQuickPlayDisplayName = string.Empty;
        private string _matchmakingStatusMessage = string.Empty;
        private string _currentPublicRoomId = string.Empty;
        private bool _joinedExistingPublicRoom;

        private float _gradualFillTimer = -1f;
        private float _kickRefillTimer = -1f;
        private float _remoteReadyTimer = -1f;
        private readonly List<int> _pendingRemoteReadyIds = new List<int>();

        public bool IsInMatch { get; private set; }
        public bool IsMatchmaking { get; private set; }
        public PlayerIdentity LocalIdentity { get; private set; }

        public MatchCreationMode CreationMode { get; private set; } = MatchCreationMode.QuickPlay;

        public bool IsPrivateRoom => IsInMatch && CreationMode == MatchCreationMode.PrivateRoom;

        public bool IsHost =>
            IsInMatch && _lobby != null && _lobby.TryGetParticipant(LocalIdentity.ConnectionId, out MatchParticipant me) && me.IsHost;

        public RoomCode LocalRoomCode { get; private set; } = RoomCode.None;

        public string MatchId { get; private set; } = string.Empty;

        public int LobbyPlayerCount => IsInMatch && _lobby != null ? _lobby.PlayerCount : 0;

        public int RequiredPlayerCount => config != null ? config.MaxPlayers : 4;

        public int MinimumPlayerCount => config != null ? config.MinimumPlayersToStart : 2;

        public int LocalConnectionId => IsInMatch ? LocalIdentity.ConnectionId : -1;

        public PlayerReadyState LocalReadyState =>
            IsInMatch && _lobby != null && _lobby.TryGetParticipant(LocalIdentity.ConnectionId, out MatchParticipant me)
                ? me.Ready
                : PlayerReadyState.NotReady;

        public bool BotFillEnabled => _bots != null && _bots.Enabled;

        public MatchState LobbyPhase => _match != null ? _match.State : MatchState.Waiting;

        public int AutoStartCountdownSecondsRemaining => _match != null ? _match.CountdownSecondsRemaining : 0;

        public bool AllPlayersReady =>
            IsInMatch && _lobby != null &&
            (IsPrivateRoom ? _lobby.AllRequiredPlayersReady() : _lobby.FullLobbyReady());

        public string MatchmakingStatusMessage => _matchmakingStatusMessage;

        public bool JoinedExistingPublicRoom => _joinedExistingPublicRoom;

        public IReadOnlyCollection<MatchParticipant> Participants =>
            IsInMatch && _lobby != null ? _lobby.Participants : EmptyParticipants;

        public event Action LobbyStateChanged;
        public event Action<int, QuickChatMessage> QuickChatReceived;

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
            _publicRooms.SeedDemoRooms(config != null ? config.MaxPlayers : 4);
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
            TickQuickPlaySearch();
            TickGradualFill();
            TickKickRefill();
            TickRemoteReady();
        }

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
            _joinedExistingPublicRoom = false;
            _matchmakingStatusMessage = "Searching for available players...";
            LobbyStateChanged?.Invoke();
        }

        /// <summary>
        /// Sprint 23.13 — offline Quick Play stub: one local player, no search,
        /// no public-room join, no gradual opponent fill. Lobby/matchmaking
        /// code paths remain intact for Invite / Private Room.
        /// </summary>
        public void CreateLocalOfflinePrototype(string localDisplayName)
        {
            if (IsMatchmaking)
            {
                CancelMatchmaking();
            }

            if (IsInMatch)
            {
                LeaveMatch();
            }

            // Arm AFTER Leave/Cancel — those paths call OfflineRaceEntryService.Clear().
            OfflineRaceEntryService.BeginPendingEntry();

            string displayName = string.IsNullOrWhiteSpace(localDisplayName) ? "Player" : localDisplayName;
            if (MatchTransportService.Current == null)
            {
                Debug.LogWarning("[SessionManager] CreateLocalOfflinePrototype: MatchTransportService.Current is null — stub match skipped; offline flag remains set.");
                _matchmakingStatusMessage = "Preparing offline race...";
                LobbyStateChanged?.Invoke();
                return;
            }

            CreateMatchInternal(displayName, MatchCreationMode.QuickPlay);
            if (!IsInMatch)
            {
                Debug.LogWarning("[SessionManager] CreateLocalOfflinePrototype: CreateMatchInternal did not enter match — offline flag remains set.");
                return;
            }

            _gradualFillTimer = -1f;
            _kickRefillTimer = -1f;
            _remoteReadyTimer = -1f;
            _pendingRemoteReadyIds.Clear();
            _currentPublicRoomId = string.Empty;
            _joinedExistingPublicRoom = false;
            _matchmakingStatusMessage = "Preparing offline race...";
            SetLocalReady(PlayerReadyState.Ready);
            LobbyStateChanged?.Invoke();
        }

        /// <summary>
        /// Sprint 23.13 — broadcast MatchState.Running so RaceFinish progress
        /// / finish-line systems activate for the local offline prototype.
        /// </summary>
        public void MarkOfflineRaceRunning()
        {
            if (!IsInMatch || !OfflineRaceEntryService.IsActive)
            {
                return;
            }

            SetLocalReady(PlayerReadyState.Ready);
            _match?.RequestMatchState(MatchState.Running);
        }

        public void CreatePrivateRoom(string localDisplayName) =>
            CreateMatchInternal(localDisplayName, MatchCreationMode.PrivateRoom);

        public void CreateMatch(string displayName) =>
            CreateMatchInternal(displayName, MatchCreationMode.QuickPlay);

        private void CreateMatchInternal(string displayName, MatchCreationMode mode)
        {
            if (IsInMatch)
            {
                return;
            }

            LocalIdentity = LocalPlayerIdentity.CreateLocal(displayName, LocalPlayerCountry);

            int maxPlayers = config != null ? config.MaxPlayers : 4;
            MatchTransportService.Current.StartHost(LocalIdentity, maxPlayers);

            LocalRoomCode = RoomCodeGenerator.Generate(_roomCodeRandom);
            MatchId = Guid.NewGuid().ToString("N");
            CreationMode = mode;
            IsInMatch = true;
            IsMatchmaking = false;
        }

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

        public void JoinPrivateRoom(string roomCode, string localDisplayName) =>
            JoinMatch(roomCode, localDisplayName);

        public void CancelOrLeaveMatch()
        {
            if (IsMatchmaking)
            {
                CancelMatchmaking();
                return;
            }

            LeaveMatch();
        }

        public void CancelMatchmaking()
        {
            _quickPlaySearchPending = false;
            _quickPlaySearchElapsed = 0f;
            _pendingQuickPlayDisplayName = string.Empty;
            IsMatchmaking = false;
            _matchmakingStatusMessage = string.Empty;
            OfflineRaceEntryService.Clear();
            LobbyStateChanged?.Invoke();
        }

        private void TickQuickPlaySearch()
        {
            if (!_quickPlaySearchPending)
            {
                return;
            }

            _quickPlaySearchElapsed += Time.deltaTime;
            if (_quickPlaySearchElapsed < QuickPlaySearchSeconds * 0.45f)
            {
                _matchmakingStatusMessage = "Searching for available players...";
            }
            else if (_quickPlaySearchElapsed < QuickPlaySearchSeconds * 0.75f)
            {
                _matchmakingStatusMessage = "Players Found";
            }

            if (_quickPlaySearchElapsed < QuickPlaySearchSeconds)
            {
                LobbyStateChanged?.Invoke();
                return;
            }

            _quickPlaySearchPending = false;
            CompleteQuickPlaySearch();
        }

        private void CompleteQuickPlaySearch()
        {
            int maxPlayers = config != null ? config.MaxPlayers : 4;
            string displayName = _pendingQuickPlayDisplayName;
            _pendingQuickPlayDisplayName = string.Empty;

            if (_publicRooms.TryFindBestJoinableRoom(maxPlayers, out MockPublicRoomDirectory.PublicRoomOffer offer))
            {
                _joinedExistingPublicRoom = true;
                _matchmakingStatusMessage = "Joining Room... (" + offer.OccupiedSeats + "/" + maxPlayers + ")";
                LobbyStateChanged?.Invoke();

                CreateMatchInternal(displayName, MatchCreationMode.QuickPlay);
                _currentPublicRoomId = offer.RoomId;
                SeedExistingPublicPlayers(offer.OccupiedSeats);
                _publicRooms.NotifyOccupancy(_currentPublicRoomId, LobbyPlayerCount, maxPlayers);
                QueueRemoteReadyStagger();
                ScheduleGradualFillIfNeeded();
                _matchmakingStatusMessage = "Joined room — Waiting For Players...";
                LobbyStateChanged?.Invoke();
                return;
            }

            _joinedExistingPublicRoom = false;
            _matchmakingStatusMessage = "Creating Room...";
            LobbyStateChanged?.Invoke();

            CreateMatchInternal(displayName, MatchCreationMode.QuickPlay);
            var created = _publicRooms.RegisterNewRoom(1, maxPlayers);
            _currentPublicRoomId = created.RoomId;
            ScheduleGradualFillIfNeeded();
            _matchmakingStatusMessage = "Waiting For Players...";
            LobbyStateChanged?.Invoke();
        }

        private void SeedExistingPublicPlayers(int existingCount)
        {
            if (!(MatchTransportService.Current is LocalLoopbackTransport loopback))
            {
                return;
            }

            GulfCountry[] countries = (GulfCountry[])Enum.GetValues(typeof(GulfCountry));
            for (int i = 0; i < existingCount; i++)
            {
                GulfCountry country = countries[(i + 1) % countries.Length];
                MatchParticipant opponent = loopback.SimulateRemoteJoin(
                    LocalPlayerIdentity.CreateLocal("Racer " + (i + 2), country));
                _pendingRemoteReadyIds.Add(opponent.Identity.ConnectionId);
            }
        }

        private void ScheduleGradualFillIfNeeded()
        {
            int maxPlayers = config != null ? config.MaxPlayers : 4;
            _gradualFillTimer = LobbyPlayerCount >= maxPlayers ? -1f : GradualFillSeconds;
        }

        private void TickGradualFill()
        {
            if (_gradualFillTimer < 0f || !IsInMatch || CreationMode != MatchCreationMode.QuickPlay)
            {
                return;
            }

            _gradualFillTimer -= Time.deltaTime;
            if (_gradualFillTimer > 0f)
            {
                return;
            }

            if (!(MatchTransportService.Current is LocalLoopbackTransport loopback) || config == null)
            {
                _gradualFillTimer = -1f;
                return;
            }

            int maxPlayers = config.MaxPlayers;
            if (LobbyPlayerCount >= maxPlayers)
            {
                _gradualFillTimer = -1f;
                _publicRooms.NotifyOccupancy(_currentPublicRoomId, LobbyPlayerCount, maxPlayers);
                return;
            }

            GulfCountry[] countries = (GulfCountry[])Enum.GetValues(typeof(GulfCountry));
            int index = LobbyPlayerCount;
            GulfCountry country = countries[(index + 1) % countries.Length];
            MatchParticipant joiner = loopback.SimulateRemoteJoin(
                LocalPlayerIdentity.CreateLocal("Racer " + (index + 1), country));
            _pendingRemoteReadyIds.Add(joiner.Identity.ConnectionId);
            _remoteReadyTimer = RemoteReadyStaggerSeconds;
            _publicRooms.NotifyOccupancy(_currentPublicRoomId, LobbyPlayerCount, maxPlayers);
            _matchmakingStatusMessage = "Players joining... " + LobbyPlayerCount + "/" + maxPlayers;

            if (LobbyPlayerCount < maxPlayers)
            {
                _gradualFillTimer = GradualFillSeconds;
            }
            else
            {
                _gradualFillTimer = -1f;
                _matchmakingStatusMessage = "Lobby full — set Ready to continue";
            }

            LobbyStateChanged?.Invoke();
        }

        private void TickKickRefill()
        {
            if (_kickRefillTimer < 0f || !IsInMatch || CreationMode != MatchCreationMode.QuickPlay)
            {
                return;
            }

            _kickRefillTimer -= Time.deltaTime;
            if (_kickRefillTimer > 0f)
            {
                return;
            }

            _kickRefillTimer = -1f;
            ScheduleGradualFillIfNeeded();
            _matchmakingStatusMessage = "Searching for replacement player...";
            LobbyStateChanged?.Invoke();
        }

        private void TickRemoteReady()
        {
            if (_pendingRemoteReadyIds.Count == 0 ||
                !(MatchTransportService.Current is LocalLoopbackTransport loopback))
            {
                return;
            }

            if (_remoteReadyTimer < 0f)
            {
                _remoteReadyTimer = RemoteReadyStaggerSeconds;
            }

            _remoteReadyTimer -= Time.deltaTime;
            if (_remoteReadyTimer > 0f)
            {
                return;
            }

            int connectionId = _pendingRemoteReadyIds[0];
            _pendingRemoteReadyIds.RemoveAt(0);
            loopback.SimulateRemoteReady(connectionId, PlayerReadyState.Ready);
            _remoteReadyTimer = _pendingRemoteReadyIds.Count > 0 ? RemoteReadyStaggerSeconds : -1f;
            LobbyStateChanged?.Invoke();
        }

        private void QueueRemoteReadyStagger() => _remoteReadyTimer = RemoteReadyStaggerSeconds;

        public void LeaveMatch()
        {
            if (!IsInMatch)
            {
                return;
            }

            int maxPlayers = config != null ? config.MaxPlayers : 4;
            if (!string.IsNullOrEmpty(_currentPublicRoomId))
            {
                _publicRooms.NotifyOccupancy(_currentPublicRoomId, 0, maxPlayers);
            }

            DisconnectReason reason = IsHost ? DisconnectReason.HostLeft : DisconnectReason.PlayerLeft;
            MatchTransportService.Current.Disconnect(reason);

            _bots?.Reset();
            _lobby?.Clear();
            _match?.ResetMatch();
            MapVotingService.Current?.Clear();

            IsInMatch = false;
            LocalRoomCode = RoomCode.None;
            MatchId = string.Empty;
            CreationMode = MatchCreationMode.QuickPlay;
            _currentPublicRoomId = string.Empty;
            _joinedExistingPublicRoom = false;
            _matchmakingStatusMessage = string.Empty;
            _gradualFillTimer = -1f;
            _kickRefillTimer = -1f;
            _remoteReadyTimer = -1f;
            _pendingRemoteReadyIds.Clear();
            OfflineRaceEntryService.Clear();
        }

        public void SetLocalReady(PlayerReadyState state)
        {
            if (!IsInMatch)
            {
                return;
            }

            MatchTransportService.Current.SetLocalReadyState(state);
        }

        public void SetBotFillEnabled(bool enabled)
        {
            if (!IsHost || _bots == null)
            {
                return;
            }

            _bots.SetEnabled(enabled);
        }

        public void KickPlayer(int connectionId)
        {
            if (!IsHost || !IsInMatch || connectionId == LocalIdentity.ConnectionId)
            {
                return;
            }

            MatchTransportService.Current.KickParticipant(connectionId, DisconnectReason.Kicked);
            _bots?.ForgetBot(connectionId);
            _pendingRemoteReadyIds.Remove(connectionId);

            if (CreationMode == MatchCreationMode.QuickPlay)
            {
                _kickRefillTimer = KickRefillSeconds;
                _matchmakingStatusMessage = "Player kicked — searching for fill...";
            }

            int maxPlayers = config != null ? config.MaxPlayers : 4;
            _publicRooms.NotifyOccupancy(_currentPublicRoomId, LobbyPlayerCount, maxPlayers);
            LobbyStateChanged?.Invoke();
        }

        public void RequestHostStart()
        {
            if (!IsHost || !AllPlayersReady)
            {
                return;
            }

            IMapContextProvider maps = MapContextService.Current;
            IMapVotingProvider voting = MapVotingService.Current;
            if (maps == null || voting == null)
            {
                _match?.TryStartCountdown();
                return;
            }

            IReadOnlyList<MapId> candidates = maps.PickRandomMaps(3);
            voting.BeginVoting(candidates, MapVoteDurationSeconds);
            SceneManager.Instance?.LoadMapVoting();
        }

        public bool IsBot(int connectionId) => _bots != null && _bots.IsBot(connectionId);

        public float GetPingMilliseconds(int connectionId) =>
            ConnectionManager.Instance != null
                ? ConnectionManager.Instance.PingSecondsFor(connectionId) * 1000f
                : 0f;

        public ConnectionQuality GetConnectionQuality(int connectionId) =>
            ConnectionManager.Instance != null
                ? ConnectionManager.Instance.GetQuality(connectionId)
                : ConnectionQuality.Disconnected;

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

        private void HandleQuickChatReceived(int connectionId, QuickChatMessage message) =>
            QuickChatReceived?.Invoke(connectionId, message);
    }
}

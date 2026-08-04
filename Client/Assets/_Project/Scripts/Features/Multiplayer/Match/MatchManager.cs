using System;
using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Domain;
using GulfRun.Features.Multiplayer.Configuration;
using GulfRun.Features.Multiplayer.Lobby;
using UnityEngine;

namespace GulfRun.Features.Multiplayer.Match
{
    /// <summary>
    /// Match-level state machine: Waiting -> Countdown -> Running -> Finished
    /// (with Disconnected as a local-only interrupt state), distinct from the
    /// single-player <see cref="GameLoopState"/>. Only the host ticks the
    /// shared countdown timer and broadcasts <see cref="MatchState"/>/seconds
    /// changes via <see cref="IMatchTransport"/> — every client (including
    /// the host's own UI) reacts purely to those broadcasts, which is what
    /// guarantees every connected player receives the identical countdown
    /// and starts simultaneously. Automatically starts the countdown the
    /// moment <see cref="LobbyManager.AllRequiredPlayersReady"/> becomes true
    /// (Ready System requirement) with no button required, mirroring the
    /// single-player race-start addendum's auto-countdown design.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MatchManager : Singleton<MatchManager>
    {
        [SerializeField] private NetworkSyncConfig config;

        private IMatchTransport _transport;
        private double _countdownElapsedSeconds;
        private int _lastBroadcastSeconds = -1;

        public MatchState State { get; private set; } = MatchState.Waiting;
        public int CountdownSecondsRemaining { get; private set; }

        public event Action<MatchState> StateChanged;
        public event Action<int> CountdownSecondsChanged;

        protected override void OnInitialize()
        {
        }

        private void OnEnable()
        {
            _transport = MatchTransportService.Current;
            _transport.MatchStateChanged += HandleRemoteMatchStateChanged;
            _transport.CountdownSecondsChanged += HandleRemoteCountdownSecondsChanged;

            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.LobbyChanged += HandleLobbyChanged;
            }
        }

        private void OnDisable()
        {
            if (_transport != null)
            {
                _transport.MatchStateChanged -= HandleRemoteMatchStateChanged;
                _transport.CountdownSecondsChanged -= HandleRemoteCountdownSecondsChanged;
            }

            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.LobbyChanged -= HandleLobbyChanged;
            }
        }

        private void Update()
        {
            // Only the host advances time/broadcasts; a non-host client (or,
            // today, the offline loopback's only role) purely reacts to
            // HandleRemoteCountdownSecondsChanged/HandleRemoteMatchStateChanged.
            if (_transport == null || !_transport.IsHost || State != MatchState.Countdown)
            {
                return;
            }

            _countdownElapsedSeconds += Time.deltaTime;
            float duration = config != null ? config.CountdownDurationSeconds : 3f;
            int wholeSeconds = CountdownMath.WholeSecondsRemaining(_countdownElapsedSeconds, duration);

            if (wholeSeconds != _lastBroadcastSeconds)
            {
                _lastBroadcastSeconds = wholeSeconds;
                _transport.BroadcastCountdownSeconds(wholeSeconds);
            }

            if (_countdownElapsedSeconds >= duration)
            {
                RequestMatchState(MatchState.Running);
            }
        }

        /// <summary>Host-only: Waiting -> Countdown, once every required player is Ready. No button required.</summary>
        public void TryStartCountdown()
        {
            if (_transport == null || !_transport.IsHost || State != MatchState.Waiting)
            {
                return;
            }

            LobbyManager lobby = LobbyManager.Instance;
            if (lobby == null || !lobby.AllRequiredPlayersReady())
            {
                return;
            }

            _countdownElapsedSeconds = 0d;
            _lastBroadcastSeconds = -1;
            RequestMatchState(MatchState.Countdown);
        }

        /// <summary>Host-only authoritative state change, broadcast to every connected participant.</summary>
        public void RequestMatchState(MatchState newState)
        {
            if (_transport == null || !_transport.IsHost)
            {
                return;
            }

            _transport.BroadcastMatchState(newState);
        }

        /// <summary>Resets local match state (e.g. after leaving); does not affect other participants.</summary>
        public void ResetMatch()
        {
            _countdownElapsedSeconds = 0d;
            _lastBroadcastSeconds = -1;
            CountdownSecondsRemaining = 0;
            SetState(MatchState.Waiting);
        }

        private void HandleLobbyChanged()
        {
            // Host must press Play → Map Voting. Do not auto-start countdown
            // when the lobby becomes Ready (Quick Play Lobby flow).
        }

        private void HandleRemoteMatchStateChanged(MatchState newState) => SetState(newState);

        private void HandleRemoteCountdownSecondsChanged(int seconds)
        {
            CountdownSecondsRemaining = seconds;
            CountdownSecondsChanged?.Invoke(seconds);
        }

        private void SetState(MatchState newState)
        {
            if (newState == State)
            {
                return;
            }

            State = newState;
            StateChanged?.Invoke(newState);
        }
    }
}

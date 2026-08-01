using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Domain;
using GulfRun.Features.Multiplayer.Configuration;
using UnityEngine;

namespace GulfRun.Features.Multiplayer.Connection
{
    /// <summary>
    /// Tracks per-participant link health (Connecting/Connected/TimedOut/
    /// Reconnecting/Disconnected), independent of Lobby/roster or Ready
    /// System concerns. Detects "Temporary Disconnect" via a per-connection
    /// heartbeat timeout (any inbound event — join or snapshot — counts as a
    /// heartbeat) and reports "Reconnect" the moment data starts flowing
    /// again, without ever removing the participant from the match itself
    /// (that remains the Lobby's decision, driven by an explicit
    /// ParticipantLeft/host-leave event). Ping is currently always 0 under
    /// <see cref="LocalLoopbackTransport"/> (in-process, no real round trip);
    /// a real transport populates <see cref="PingSecondsFor"/> from measured RTT.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ConnectionManager : Singleton<ConnectionManager>
    {
        [SerializeField] private NetworkSyncConfig config;

        private readonly Dictionary<int, ConnectionState> _states = new Dictionary<int, ConnectionState>();
        private readonly Dictionary<int, float> _lastSeenTime = new Dictionary<int, float>();
        private readonly List<int> _scratchIds = new List<int>();

        public event Action<int, ConnectionState> ConnectionStateChanged;
        public event Action<int> PlayerTimedOut;
        public event Action<int> PlayerReconnected;

        protected override void OnInitialize()
        {
        }

        private void OnEnable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            transport.ParticipantJoined += HandleParticipantJoined;
            transport.ParticipantLeft += HandleParticipantLeft;
            transport.SnapshotReceived += HandleSnapshotReceived;
        }

        private void OnDisable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            transport.ParticipantJoined -= HandleParticipantJoined;
            transport.ParticipantLeft -= HandleParticipantLeft;
            transport.SnapshotReceived -= HandleSnapshotReceived;
        }

        private void Update()
        {
            if (config == null || _lastSeenTime.Count == 0)
            {
                return;
            }

            float now = Time.time;
            float timeout = config.ConnectionTimeoutSeconds;

            _scratchIds.Clear();
            _scratchIds.AddRange(_lastSeenTime.Keys);

            for (int i = 0; i < _scratchIds.Count; i++)
            {
                int id = _scratchIds[i];
                if (GetState(id) == ConnectionState.TimedOut)
                {
                    continue;
                }

                if (now - _lastSeenTime[id] > timeout)
                {
                    SetState(id, ConnectionState.TimedOut);
                    PlayerTimedOut?.Invoke(id);
                }
            }
        }

        public ConnectionState GetState(int connectionId) =>
            _states.TryGetValue(connectionId, out ConnectionState state) ? state : ConnectionState.Disconnected;

        /// <summary>Round-trip time in seconds. Always 0 under the offline loopback transport; a real transport supplies measured RTT.</summary>
        public float PingSecondsFor(int connectionId) => 0f;

        /// <summary>Sprint 15 (Player Cards "Connection Quality" / Network "Latency indicator"). Pure bucketing of live state + ping via <see cref="ConnectionQualityResolver"/> — never disconnected/timed-out participants are never reported as merely "Poor".</summary>
        public ConnectionQuality GetQuality(int connectionId) =>
            ConnectionQualityResolver.Resolve(GetState(connectionId), PingSecondsFor(connectionId) * 1000f);

        private void HandleParticipantJoined(MatchParticipant participant)
        {
            int id = participant.Identity.ConnectionId;
            _lastSeenTime[id] = Time.time;
            SetState(id, ConnectionState.Connected);
        }

        private void HandleParticipantLeft(int connectionId, DisconnectReason reason)
        {
            _lastSeenTime.Remove(connectionId);
            SetState(connectionId, ConnectionState.Disconnected);
        }

        private void HandleSnapshotReceived(NetworkPlayerSnapshot snapshot)
        {
            _lastSeenTime[snapshot.ConnectionId] = Time.time;

            ConnectionState current = GetState(snapshot.ConnectionId);
            if (current == ConnectionState.TimedOut || current == ConnectionState.Reconnecting)
            {
                SetState(snapshot.ConnectionId, ConnectionState.Connected);
                PlayerReconnected?.Invoke(snapshot.ConnectionId);
            }
        }

        private void SetState(int connectionId, ConnectionState state)
        {
            if (_states.TryGetValue(connectionId, out ConnectionState existing) && existing == state)
            {
                return;
            }

            _states[connectionId] = state;
            ConnectionStateChanged?.Invoke(connectionId, state);
        }
    }
}

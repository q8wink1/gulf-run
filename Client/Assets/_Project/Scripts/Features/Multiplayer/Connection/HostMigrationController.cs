using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Domain;
using GulfRun.Features.Multiplayer.Lobby;
using UnityEngine;

namespace GulfRun.Features.Multiplayer.Connection
{
    /// <summary>
    /// Sprint 15 "NETWORK: Host migration ready". Pure, transport-agnostic
    /// promotion rule: the moment the participant who currently holds Host
    /// leaves for any reason, the earliest-still-connected remaining
    /// participant (by join order) becomes the new Host via
    /// <see cref="LobbyManager.PromoteToHost"/>. Every connected client runs
    /// this exact same deterministic algorithm over the exact same ordered
    /// join history, so — once a real multi-machine transport exists — every
    /// machine converges on the identical new Host with no central arbiter
    /// call needed, mirroring this project's existing "client predicts,
    /// deterministic rule decides" posture (see <see cref="MatchManager"/>'s
    /// countdown).
    ///
    /// Deliberately maintains its own independent join-order/was-host
    /// tracking rather than reading <see cref="LobbyManager"/> post-removal
    /// state, so this never depends on component subscription order against
    /// <see cref="LobbyManager"/>'s own <c>ParticipantLeft</c> handler.
    ///
    /// Honesty note (see Sprint report Remaining TODOs): under the current
    /// single-process <see cref="LocalLoopbackTransport"/> the only
    /// participant that can ever hold Host is the local machine itself (see
    /// <c>SessionManager.CreateMatchInternal</c>) — a real "the host's
    /// machine disconnected while I stayed connected" scenario cannot occur
    /// until a real multi-client transport lands. The algorithm is fully
    /// exercised today via <c>MultiplayerDebugView</c>'s "Simulate Host
    /// Leave" button, which removes the host's roster entry exactly the way
    /// a real remote disconnect would and lets every other connected
    /// participant (including bots) observe the same promotion.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class HostMigrationController : Singleton<HostMigrationController>
    {
        private readonly List<int> _joinOrder = new List<int>();
        private readonly Dictionary<int, bool> _wasHostByConnection = new Dictionary<int, bool>();

        public event Action<int> HostMigrated;

        protected override void OnInitialize()
        {
        }

        private void OnEnable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            transport.ParticipantJoined += HandleParticipantJoined;
            transport.ParticipantLeft += HandleParticipantLeft;
        }

        private void OnDisable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            if (transport != null)
            {
                transport.ParticipantJoined -= HandleParticipantJoined;
                transport.ParticipantLeft -= HandleParticipantLeft;
            }
        }

        private void HandleParticipantJoined(MatchParticipant participant)
        {
            int id = participant.Identity.ConnectionId;
            if (!_wasHostByConnection.ContainsKey(id))
            {
                _joinOrder.Add(id);
            }

            _wasHostByConnection[id] = participant.IsHost;
        }

        private void HandleParticipantLeft(int connectionId, DisconnectReason reason)
        {
            bool wasHost = _wasHostByConnection.TryGetValue(connectionId, out bool value) && value;

            _wasHostByConnection.Remove(connectionId);
            _joinOrder.Remove(connectionId);

            if (!wasHost || _joinOrder.Count == 0)
            {
                return;
            }

            int newHostConnectionId = _joinOrder[0];
            _wasHostByConnection[newHostConnectionId] = true;

            LobbyManager.Instance?.PromoteToHost(newHostConnectionId);
            HostMigrated?.Invoke(newHostConnectionId);
        }
    }
}

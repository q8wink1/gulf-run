using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Domain;
using GulfRun.Features.Multiplayer.Configuration;
using UnityEngine;

namespace GulfRun.Features.Multiplayer.Lobby
{
    /// <summary>
    /// The Lobby/Waiting Room roster: every joined <see cref="MatchParticipant"/>
    /// with their Ready/Connection/Host status and the current player count,
    /// kept in sync purely by listening to <see cref="IMatchTransport"/>
    /// events. <see cref="LobbyChanged"/> fires after the roster dictionary
    /// has been updated, so any listener (e.g. MatchManager's ready check, or
    /// a debug/UI view) always observes a fully up-to-date roster.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LobbyManager : Singleton<LobbyManager>
    {
        [SerializeField] private NetworkSyncConfig config;

        private readonly Dictionary<int, MatchParticipant> _participants = new Dictionary<int, MatchParticipant>();

        public IReadOnlyCollection<MatchParticipant> Participants => _participants.Values;
        public int PlayerCount => _participants.Count;

        /// <summary>Raised after any join/leave/ready/connection change has been applied to the roster.</summary>
        public event Action LobbyChanged;

        protected override void OnInitialize()
        {
        }

        private void OnEnable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            transport.ParticipantJoined += HandleParticipantJoined;
            transport.ParticipantLeft += HandleParticipantLeft;
            transport.ReadyStateChanged += HandleReadyStateChanged;
        }

        private void OnDisable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            transport.ParticipantJoined -= HandleParticipantJoined;
            transport.ParticipantLeft -= HandleParticipantLeft;
            transport.ReadyStateChanged -= HandleReadyStateChanged;
        }

        public bool TryGetParticipant(int connectionId, out MatchParticipant participant) =>
            _participants.TryGetValue(connectionId, out participant);

        /// <summary>True once at least the configured minimum number of players have all joined and readied up.</summary>
        public bool AllRequiredPlayersReady()
        {
            int minimum = config != null ? config.MinimumPlayersToStart : 2;
            if (_participants.Count < minimum)
            {
                return false;
            }

            foreach (MatchParticipant participant in _participants.Values)
            {
                if (participant.Ready != PlayerReadyState.Ready)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Sprint 15 (Network "Host migration ready"). Marks <paramref name="connectionId"/> as the new Room Owner; a no-op if it is not currently a roster member. Called only by <see cref="Connection.HostMigrationController"/> once the previous host has left.</summary>
        public void PromoteToHost(int connectionId)
        {
            if (!_participants.TryGetValue(connectionId, out MatchParticipant participant) || participant.IsHost)
            {
                return;
            }

            _participants[connectionId] = participant.WithHost(true);
            LobbyChanged?.Invoke();
        }

        /// <summary>Clears the roster, e.g. after leaving a match.</summary>
        public void Clear()
        {
            if (_participants.Count == 0)
            {
                return;
            }

            _participants.Clear();
            LobbyChanged?.Invoke();
        }

        private void HandleParticipantJoined(MatchParticipant participant)
        {
            _participants[participant.Identity.ConnectionId] = participant;
            LobbyChanged?.Invoke();
        }

        private void HandleParticipantLeft(int connectionId, DisconnectReason reason)
        {
            if (_participants.Remove(connectionId))
            {
                LobbyChanged?.Invoke();
            }
        }

        private void HandleReadyStateChanged(int connectionId, PlayerReadyState state)
        {
            if (_participants.TryGetValue(connectionId, out MatchParticipant participant))
            {
                _participants[connectionId] = participant.WithReady(state);
                LobbyChanged?.Invoke();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Networking
{
    /// <summary>
    /// In-process, single-machine <see cref="IMatchTransport"/> implementation.
    /// There are no sockets and no real remote peers — every "remote"
    /// participant only exists because <see cref="SimulateRemoteJoin"/> (or
    /// one of its siblings, below) was called explicitly, e.g. from
    /// <c>MultiplayerDebugView</c> for local testing. This is the default
    /// transport registered by <see cref="MatchTransportService"/> until a
    /// ratified Netcode ADR supplies a real one (Unity Netcode for
    /// GameObjects, a custom authoritative protocol, or a third party — see
    /// docs/adr/0001-multiplayer-transport-abstraction.md); every manager in
    /// Features.Multiplayer only ever talks to <see cref="IMatchTransport"/>,
    /// so replacing this class is the entire migration.
    ///
    /// Deliberately has zero UnityEngine dependency — this exact class could
    /// run unmodified inside a future plain-.NET dedicated server process,
    /// which is one concrete way this sprint satisfies "future dedicated
    /// server compatibility" rather than just asserting it in prose.
    /// </summary>
    public sealed class LocalLoopbackTransport : IMatchTransport
    {
        private readonly Dictionary<int, MatchParticipant> _participants = new Dictionary<int, MatchParticipant>();
        private int _nextConnectionId;

        public bool IsActive { get; private set; }
        public bool IsHost { get; private set; }
        public int LocalConnectionId { get; private set; } = -1;

        public IReadOnlyCollection<MatchParticipant> Participants => _participants.Values;

        public event Action<MatchParticipant> ParticipantJoined;
        public event Action<int, DisconnectReason> ParticipantLeft;
        public event Action<int, PlayerReadyState> ReadyStateChanged;
        public event Action<MatchState> MatchStateChanged;
        public event Action<int> CountdownSecondsChanged;
        public event Action<NetworkPlayerSnapshot> SnapshotReceived;

        public event Action<WeaponPickupRequest> WeaponPickupRequested;
        public event Action<WeaponPickupEvent> WeaponPickupConfirmed;
        public event Action<WeaponUseRequest> WeaponUseRequested;
        public event Action<WeaponUseRequest> WeaponUseConfirmed;
        public event Action<WeaponHitEvent> WeaponHitReported;
        public event Action<WeaponHitEvent> WeaponHitConfirmed;

        public event Action<TrapSpawnEvent> TrapSpawned;
        public event Action<int> TrapExpired;
        public event Action<TrapTriggerEvent> TrapTriggerReported;
        public event Action<TrapTriggerEvent> TrapTriggerConfirmed;

        public void StartHost(PlayerIdentity hostIdentity, int maxParticipants)
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;
            IsHost = true;
            LocalConnectionId = AllocateConnectionId();

            var participant = new MatchParticipant(
                hostIdentity.WithConnectionId(LocalConnectionId),
                isHost: true,
                ready: PlayerReadyState.NotReady,
                connection: ConnectionState.Connected);

            _participants[LocalConnectionId] = participant;
            ParticipantJoined?.Invoke(participant);
        }

        public void JoinAsClient(PlayerIdentity clientIdentity, string joinCode)
        {
            // No real remote host is reachable in-process. Left as a no-op
            // stub so calling code (SessionManager.JoinMatch, and eventually
            // a menu UI) can already wire up the "Join Match" action without
            // depending on the future real transport implementation.
        }

        public void Disconnect(DisconnectReason reason)
        {
            if (!IsActive)
            {
                return;
            }

            var connectionIds = new List<int>(_participants.Keys);
            _participants.Clear();

            for (int i = 0; i < connectionIds.Count; i++)
            {
                ParticipantLeft?.Invoke(connectionIds[i], reason);
            }

            IsActive = false;
            IsHost = false;
            LocalConnectionId = -1;
        }

        public void SetLocalReadyState(PlayerReadyState state) => SetReadyState(LocalConnectionId, state);

        public void BroadcastMatchState(MatchState state) => MatchStateChanged?.Invoke(state);

        public void BroadcastCountdownSeconds(int secondsRemaining) => CountdownSecondsChanged?.Invoke(secondsRemaining);

        public void SendSnapshot(NetworkPlayerSnapshot snapshot) => SnapshotReceived?.Invoke(snapshot);

        // --- Sprint 5: Weapon System. This transport only relays — it has no
        // opinion on inventory rules, spawn odds, or targeting; that
        // authority-side logic lives entirely in
        // Features.Weapons.Authority.WeaponAuthority, which is the only
        // listener expected to call the Confirm* methods (mirroring how
        // BroadcastMatchState is only ever called host-side by MatchManager). ---

        public void RequestWeaponPickup(WeaponPickupRequest request) => WeaponPickupRequested?.Invoke(request);

        public void ConfirmWeaponPickup(WeaponPickupEvent confirmed) => WeaponPickupConfirmed?.Invoke(confirmed);

        public void RequestWeaponUse(WeaponUseRequest request) => WeaponUseRequested?.Invoke(request);

        public void ConfirmWeaponUse(WeaponUseRequest confirmed) => WeaponUseConfirmed?.Invoke(confirmed);

        public void ReportWeaponHit(WeaponHitEvent hit) => WeaponHitReported?.Invoke(hit);

        public void ConfirmWeaponHit(WeaponHitEvent confirmed) => WeaponHitConfirmed?.Invoke(confirmed);

        // --- Sprint 6: Dynamic Trap System. Same "this transport only
        // relays" role as the Weapon System block above — all spawn-rule,
        // lifetime, and hit-validation logic lives in
        // Features.Traps.Authority.TrapAuthority. ---

        public void BroadcastTrapSpawned(TrapSpawnEvent spawned) => TrapSpawned?.Invoke(spawned);

        public void BroadcastTrapExpired(int trapInstanceId) => TrapExpired?.Invoke(trapInstanceId);

        public void ReportTrapTrigger(TrapTriggerEvent trigger) => TrapTriggerReported?.Invoke(trigger);

        public void ConfirmTrapTrigger(TrapTriggerEvent confirmed) => TrapTriggerConfirmed?.Invoke(confirmed);

        // --- Local-only test/demo hooks; not part of IMatchTransport ---

        public MatchParticipant SimulateRemoteJoin(PlayerIdentity identity)
        {
            int connectionId = AllocateConnectionId();
            var participant = new MatchParticipant(
                identity.WithConnectionId(connectionId),
                isHost: false,
                ready: PlayerReadyState.NotReady,
                connection: ConnectionState.Connected);

            _participants[connectionId] = participant;
            ParticipantJoined?.Invoke(participant);
            return participant;
        }

        public void SimulateRemoteLeave(int connectionId, DisconnectReason reason)
        {
            if (_participants.Remove(connectionId))
            {
                ParticipantLeft?.Invoke(connectionId, reason);
            }
        }

        public void SimulateRemoteReady(int connectionId, PlayerReadyState state) => SetReadyState(connectionId, state);

        public void SimulateRemoteSnapshot(NetworkPlayerSnapshot snapshot) => SnapshotReceived?.Invoke(snapshot);

        private void SetReadyState(int connectionId, PlayerReadyState state)
        {
            if (!_participants.TryGetValue(connectionId, out MatchParticipant participant))
            {
                return;
            }

            _participants[connectionId] = participant.WithReady(state);
            ReadyStateChanged?.Invoke(connectionId, state);
        }

        private int AllocateConnectionId()
        {
            int id = _nextConnectionId;
            _nextConnectionId++;
            return id;
        }
    }
}

using System;
using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Networking
{
    /// <summary>
    /// The single seam every multiplayer manager (Connection/Lobby/Match/
    /// Session/Spawn — see Features.Multiplayer) talks to instead of any
    /// concrete transport. This is intentionally transport-agnostic: the
    /// Netcode approach (custom protocol, Unity Netcode for GameObjects, or
    /// a third party) is still an open decision pending an ADR (see
    /// docs/02-architecture/MULTIPLAYER_ARCHITECTURE.md §9 and
    /// docs/adr/0001-multiplayer-transport-abstraction.md). Swapping the
    /// registered <see cref="MatchTransportService.Current"/> implementation
    /// is the only change required once that ADR is ratified — no
    /// gameplay-facing manager code depends on the wire format.
    /// </summary>
    public interface IMatchTransport
    {
        bool IsActive { get; }
        bool IsHost { get; }
        int LocalConnectionId { get; }

        /// <summary>Live roster of every currently connected participant (Sprint 5: lets a feature resolve "who else is in this match" without depending on Features.Multiplayer.Lobby directly).</summary>
        IReadOnlyCollection<MatchParticipant> Participants { get; }

        event Action<MatchParticipant> ParticipantJoined;
        event Action<int, DisconnectReason> ParticipantLeft;
        event Action<int, PlayerReadyState> ReadyStateChanged;
        event Action<MatchState> MatchStateChanged;
        event Action<int> CountdownSecondsChanged;
        event Action<NetworkPlayerSnapshot> SnapshotReceived;

        // --- Sprint 5: Weapon System sync. Same client-asks / authority-confirms
        // duality as the Ready System above: a client calls Request*, the
        // host-authoritative side (see Features.Weapons.Authority.WeaponAuthority)
        // validates and calls Confirm*, and every connected client (including
        // the host's own UI) reacts only to the Confirmed events — exactly
        // what "the server validates pickup/usage/hit detection/removal"
        // requires, using the same seam every other multiplayer manager uses. ---

        event Action<WeaponPickupRequest> WeaponPickupRequested;
        event Action<WeaponPickupEvent> WeaponPickupConfirmed;
        event Action<WeaponUseRequest> WeaponUseRequested;
        event Action<WeaponUseRequest> WeaponUseConfirmed;
        event Action<WeaponHitEvent> WeaponHitReported;
        event Action<WeaponHitEvent> WeaponHitConfirmed;

        /// <summary>Client: asks the authority to resolve an Item Box touch. Never grants a weapon itself.</summary>
        void RequestWeaponPickup(WeaponPickupRequest request);

        /// <summary>Authority-only: broadcasts the validated pickup outcome (possibly ungranted — "the Item Box is lost").</summary>
        void ConfirmWeaponPickup(WeaponPickupEvent confirmed);

        /// <summary>Client: asks the authority to activate a weapon it believes it is carrying.</summary>
        void RequestWeaponUse(WeaponUseRequest request);

        /// <summary>Authority-only: broadcasts a validated weapon activation. Inventories update from this event alone.</summary>
        void ConfirmWeaponUse(WeaponUseRequest confirmed);

        /// <summary>Client: reports a candidate hit it observed locally (seam for a future collision/prediction system).</summary>
        void ReportWeaponHit(WeaponHitEvent hit);

        /// <summary>Authority-only: broadcasts a validated hit. Status effects are applied from this event alone.</summary>
        void ConfirmWeaponHit(WeaponHitEvent confirmed);

        /// <summary>Creates a new match and becomes its authoritative host.</summary>
        void StartHost(PlayerIdentity hostIdentity, int maxParticipants);

        /// <summary>Joins an existing match as a non-authoritative client.</summary>
        void JoinAsClient(PlayerIdentity clientIdentity, string joinCode);

        /// <summary>Leaves the current match, if any.</summary>
        void Disconnect(DisconnectReason reason);

        /// <summary>Sets the local participant's Ready System state.</summary>
        void SetLocalReadyState(PlayerReadyState state);

        /// <summary>Host-authoritative: broadcasts a new <see cref="MatchState"/> to every participant.</summary>
        void BroadcastMatchState(MatchState state);

        /// <summary>Host-authoritative: broadcasts the shared countdown value so every client sees the same 3-2-1-GO.</summary>
        void BroadcastCountdownSeconds(int secondsRemaining);

        /// <summary>Sends this frame's local player snapshot for network sync/interpolation.</summary>
        void SendSnapshot(NetworkPlayerSnapshot snapshot);
    }
}

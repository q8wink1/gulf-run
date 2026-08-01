using System;
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

        event Action<MatchParticipant> ParticipantJoined;
        event Action<int, DisconnectReason> ParticipantLeft;
        event Action<int, PlayerReadyState> ReadyStateChanged;
        event Action<MatchState> MatchStateChanged;
        event Action<int> CountdownSecondsChanged;
        event Action<NetworkPlayerSnapshot> SnapshotReceived;

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

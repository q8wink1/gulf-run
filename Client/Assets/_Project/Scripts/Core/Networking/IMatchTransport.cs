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

        // --- Sprint 6: Dynamic Trap System sync. Traps belong to the map, not
        // any player, so unlike weapons there is no client "request" for
        // spawning — only Features.Traps.Authority.TrapAuthority (host-only)
        // ever decides to spawn/expire one, and every client (including the
        // host's own scene) reacts only to the broadcasts below. Triggering
        // still follows the same client-reports / authority-confirms shape as
        // WeaponHit, since only a client's own physics can observe the
        // collision in the first place. ---

        event Action<TrapSpawnEvent> TrapSpawned;
        event Action<int> TrapExpired;
        event Action<TrapTriggerEvent> TrapTriggerReported;
        event Action<TrapTriggerEvent> TrapTriggerConfirmed;

        /// <summary>Authority-only: broadcasts that a new trap instance now exists in the world, at a fully randomized position (see TrapPositionRoll) — never a fixed layout.</summary>
        void BroadcastTrapSpawned(TrapSpawnEvent spawned);

        /// <summary>Authority-only: broadcasts that a trap instance's ~15s lifetime has elapsed; every client returns its local instance to the pool.</summary>
        void BroadcastTrapExpired(int trapInstanceId);

        /// <summary>Client: reports a candidate "I touched this active trap" observed locally. Never applies the effect itself.</summary>
        void ReportTrapTrigger(TrapTriggerEvent trigger);

        /// <summary>Authority-only: broadcasts a validated trigger (the trap instance was still active). Status effects are applied from this event alone.</summary>
        void ConfirmTrapTrigger(TrapTriggerEvent confirmed);

        // --- Sprint 7: Race Finish, Ranking & Victory Ceremony sync. The
        // race does not end at first place, so every participant's progress
        // must be tracked until they either finish or are eliminated; only
        // Features.RaceFinish.Authority.RaceFinishAuthority (host-only) ever
        // decides a finish/elimination/final-ranking/ceremony-phase outcome,
        // mirroring the "client reports, host confirms" shape Weapons/Traps
        // already established. Returning to the lobby reuses the existing
        // MatchState.Waiting broadcast below — no new message is needed,
        // and since LobbyManager only clears its roster on an explicit
        // leave/disconnect, every connected player simply stays put. ---

        event Action<RaceProgressReport> RaceProgressReported;
        event Action<PlayerRaceResult> PlayerRaceResultReported;
        event Action<EliminationStatusEvent> EliminationStatusChanged;
        event Action<PlayerRaceResult[]> RaceResultsFinalized;
        event Action<RaceRewardBreakdown> RaceRewardCalculated;
        event Action<RaceEndPhase> RaceEndPhaseChanged;
        event Action<int> SkipRaceEndPhaseRequested;

        /// <summary>Client: periodically reports local distance/coins so the host can detect a finish-line crossing or an elimination gap.</summary>
        void ReportRaceProgress(RaceProgressReport report);

        /// <summary>Authority-only: broadcasts one player's live resolution (finished or eliminated) the instant it happens. <see cref="PlayerRaceResult.FinishPosition"/> is -1 until the whole race ends.</summary>
        void BroadcastPlayerRaceResult(PlayerRaceResult result);

        /// <summary>Authority-only: broadcasts a player's live elimination warning/countdown/clear state.</summary>
        void BroadcastEliminationStatus(EliminationStatusEvent status);

        /// <summary>Authority-only: broadcasts the complete, final 1..N ranking once every participant has resolved. Fired exactly once per race.</summary>
        void BroadcastRaceResultsFinalized(PlayerRaceResult[] results);

        /// <summary>Authority-only: broadcasts one player's computed reward. Every client receives every player's breakdown; only the Reward Screen's local-connection filter enforces privacy (see <see cref="RaceRewardBreakdown"/>).</summary>
        void BroadcastRaceReward(RaceRewardBreakdown reward);

        /// <summary>Authority-only: broadcasts the current post-race presentation phase (Podium/Reward/None) so every client's ceremony stays in lockstep.</summary>
        void BroadcastRaceEndPhase(RaceEndPhase phase);

        /// <summary>Client: asks the host to advance the current Podium/Reward phase immediately (the Skip button). Any single participant's request advances the phase for everyone.</summary>
        void RequestSkipRaceEndPhase();

        // --- Sprint 8: Characters, Countries & Customization sync. Unlike
        // Weapons/Traps/Race Finish, a loadout is never host-computed or
        // arbitrated — it is entirely the owning player's own choice (which
        // character, which owned cosmetic per slot). This mirrors the Ready
        // System's shape (SetLocalReadyState / ReadyStateChanged) rather
        // than the heavier Request/Confirm authority pattern: every client
        // just broadcasts its own current loadout whenever it changes (and
        // once on joining a match), and every other client's UI reacts only
        // to the broadcast event. ---

        event Action<PlayerLoadout> LoadoutChanged;

        /// <summary>Broadcasts the local player's current Character/Country/equipped-cosmetics/Victory-Pose loadout to every other participant. Callers must pass a private copy (see <see cref="PlayerLoadout.Clone"/>) — never a reference still being mutated locally.</summary>
        void SetLocalLoadout(PlayerLoadout loadout);

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

        // --- Sprint 15: Matchmaking, Room & Pre-Race Lobby. "Kick Player" is
        // host-only, same authority posture as every Broadcast* call above;
        // Quick Chat has no authority step at all (mirrors LoadoutChanged —
        // every client just broadcasts its own action and every other
        // client's UI reacts to the event, no server validation needed for
        // four fixed, harmless preset messages). ---

        /// <summary>Host-authoritative: forcibly removes a participant from the match (Owner "Kick Player", or Bot Fill removing a bot it added).</summary>
        void KickParticipant(int connectionId, DisconnectReason reason);

        event Action<int, QuickChatMessage> QuickChatReceived;

        /// <summary>Broadcasts one of the fixed Quick Chat presets from the local player to every other participant.</summary>
        void SendQuickChat(QuickChatMessage message);

        // --- Sprint 15: Race HUD in-race emotes. Same fire-and-forget shape
        // as Quick Chat — fixed presets, no authority validation. ---

        event Action<int, RaceEmoteId> RaceEmoteReceived;

        /// <summary>Broadcasts one fixed in-race emote from the local player.</summary>
        void SendRaceEmote(RaceEmoteId emote);
    }
}

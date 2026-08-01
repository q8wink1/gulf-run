using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Traps.Avoidance
{
    /// <summary>
    /// Sprint 11 "Daily Missions: Avoid 10 Traps" hook. This project has no
    /// spatial "was the player nearby but didn't touch it" concept — traps
    /// are always spawned ahead of the local player's own path (see
    /// <c>TrapAuthority.TrySpawnTrap</c>, which rolls position from
    /// <c>LocalPlayerStateService.Current.Position</c>), and only the local
    /// player has a physically simulated collider today (no networked
    /// remote avatar exists yet). So "avoided" is honestly and simply
    /// defined here as: an active trap instance's lifetime expired
    /// (<see cref="IMatchTransport.TrapExpired"/>) without it ever
    /// confirming a hit against the local player. A separate,
    /// single-responsibility listener from <see cref="Effects.TrapEffectApplicator"/>
    /// (which only cares about applying a confirmed hit's effect), kept in
    /// its own <see cref="SceneSingleton{T}"/> — same "one small class per
    /// concern" split the rest of this project already uses.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TrapAvoidanceTracker : SceneSingleton<TrapAvoidanceTracker>
    {
        private readonly HashSet<int> _liveInstanceIds = new HashSet<int>();
        private readonly HashSet<int> _hitLocalPlayerInstanceIds = new HashSet<int>();

        private IMatchTransport _transport;

        private void OnEnable()
        {
            _transport = MatchTransportService.Current;
            _transport.TrapSpawned += HandleTrapSpawned;
            _transport.TrapExpired += HandleTrapExpired;
            _transport.TrapTriggerConfirmed += HandleTrapTriggerConfirmed;
        }

        private void OnDisable()
        {
            if (_transport == null)
            {
                return;
            }

            _transport.TrapSpawned -= HandleTrapSpawned;
            _transport.TrapExpired -= HandleTrapExpired;
            _transport.TrapTriggerConfirmed -= HandleTrapTriggerConfirmed;
        }

        private void HandleTrapSpawned(TrapSpawnEvent spawned)
        {
            _liveInstanceIds.Add(spawned.TrapInstanceId);
        }

        private void HandleTrapTriggerConfirmed(TrapTriggerEvent hit)
        {
            if (_transport != null && hit.TargetConnectionId == _transport.LocalConnectionId)
            {
                _hitLocalPlayerInstanceIds.Add(hit.TrapInstanceId);
            }
        }

        private void HandleTrapExpired(int trapInstanceId)
        {
            bool wasLive = _liveInstanceIds.Remove(trapInstanceId);
            bool hitLocalPlayer = _hitLocalPlayerInstanceIds.Remove(trapInstanceId);

            if (wasLive && !hitLocalPlayer)
            {
                PlayerStatEventService.RaiseLocalTrapAvoided();
            }
        }
    }
}

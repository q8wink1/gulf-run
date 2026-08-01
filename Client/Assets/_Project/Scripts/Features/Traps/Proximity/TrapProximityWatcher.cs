using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Traps.Proximity
{
    /// <summary>
    /// Tracks live trap positions vs the local player and publishes a
    /// proximity warning for the Race HUD (indicator only — never auto-avoids).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TrapProximityWatcher : SceneSingleton<TrapProximityWatcher>, ITrapProximityHudProvider
    {
        [SerializeField] private float warningRadiusMeters = 8f;

        private readonly Dictionary<int, TrapSpawnEvent> _live = new Dictionary<int, TrapSpawnEvent>();
        private IMatchTransport _transport;
        private TrapId? _nearbyTrapId;
        private float _proximity01;

        public bool IsTrapNearby => _nearbyTrapId.HasValue;
        public TrapId? NearbyTrapId => _nearbyTrapId;
        public float Proximity01 => _proximity01;

        private void OnEnable()
        {
            TrapProximityHudService.Current = this;
            _transport = MatchTransportService.Current;
            _transport.TrapSpawned += HandleTrapSpawned;
            _transport.TrapExpired += HandleTrapExpired;
            _transport.TrapTriggerConfirmed += HandleTrapTriggerConfirmed;
        }

        private void OnDisable()
        {
            if (ReferenceEquals(TrapProximityHudService.Current, this))
            {
                TrapProximityHudService.Current = null;
            }

            if (_transport == null)
            {
                return;
            }

            _transport.TrapSpawned -= HandleTrapSpawned;
            _transport.TrapExpired -= HandleTrapExpired;
            _transport.TrapTriggerConfirmed -= HandleTrapTriggerConfirmed;
        }

        private void Update()
        {
            _nearbyTrapId = null;
            _proximity01 = 0f;

            ILocalPlayerStateProvider local = LocalPlayerStateService.Current;
            if (local == null || _live.Count == 0)
            {
                return;
            }

            Vector2 player = local.Position;
            float best = warningRadiusMeters;
            foreach (KeyValuePair<int, TrapSpawnEvent> pair in _live)
            {
                float dx = pair.Value.Position.X - player.x;
                float dy = pair.Value.Position.Y - player.y;
                float distance = (float)System.Math.Sqrt(dx * dx + dy * dy);
                if (distance < best)
                {
                    best = distance;
                    _nearbyTrapId = pair.Value.Trap;
                    _proximity01 = 1f - Mathf.Clamp01(distance / warningRadiusMeters);
                }
            }
        }

        private void HandleTrapSpawned(TrapSpawnEvent spawned) => _live[spawned.TrapInstanceId] = spawned;

        private void HandleTrapExpired(int trapInstanceId) => _live.Remove(trapInstanceId);

        private void HandleTrapTriggerConfirmed(TrapTriggerEvent hit) => _live.Remove(hit.TrapInstanceId);
    }
}

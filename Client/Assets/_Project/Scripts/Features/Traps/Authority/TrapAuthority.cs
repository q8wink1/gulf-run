using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Traps.Configuration;
using UnityEngine;

namespace GulfRun.Features.Traps.Authority
{
    /// <summary>
    /// Host-authoritative decision-maker for every Dynamic Trap System
    /// network message: on a configurable timer, decides whether/where/which
    /// trap to spawn (weighted pick + randomized position, scaled by the
    /// shared difficulty ramp), tracks each active instance's expiry, and
    /// validates every client-reported trigger before confirming it. Exactly
    /// the same role <c>Features.Weapons.Authority.WeaponAuthority</c> plays
    /// for weapons — every gameplay-facing system (spawn controller, effect
    /// applicator, debug UI) reacts only to the Confirmed/broadcast events
    /// this class produces, never to a raw client report, so a client can
    /// never spawn itself a trap or fake a trigger.
    ///
    /// Persistent (match-spanning) — placed alongside the Sprint 4/5
    /// Connection/Lobby/Match/Session/Weapon managers in Boot.unity's
    /// TrapSystems GameObject.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TrapAuthority : Singleton<TrapAuthority>
    {
        [SerializeField] private TrapCatalogConfig catalog;

        private readonly Dictionary<int, ActiveTrap> _active = new Dictionary<int, ActiveTrap>();
        private readonly List<int> _expiredScratch = new List<int>();

        private IMatchTransport _transport;
        private IRandomSource _random;
        private int _nextInstanceId = 1;
        private float _spawnTimerSeconds;

        /// <summary>Host-side only: number of trap instances this TrapAuthority currently considers active. Zero on every non-host client (their Update() is a no-op) — debug/UI should read TrapSpawnController.ActiveTraps instead, which every client populates.</summary>
        public int HostActiveTrapCount => _active.Count;

        protected override void OnInitialize()
        {
            _random = SeededRandom.FromTime();
        }

        private void OnEnable()
        {
            _transport = MatchTransportService.Current;
            _transport.TrapTriggerReported += HandleTrapTriggerReported;
        }

        private void OnDisable()
        {
            if (_transport != null)
            {
                _transport.TrapTriggerReported -= HandleTrapTriggerReported;
            }
        }

        private void Update()
        {
            if (_transport == null || !_transport.IsHost || catalog == null)
            {
                return;
            }

            IGameStateProvider gameState = GameStateService.Current;
            if (gameState != null && gameState.CurrentState != GameLoopState.Running)
            {
                return;
            }

            ExpireDueTraps();

            _spawnTimerSeconds -= Time.deltaTime;
            if (_spawnTimerSeconds <= 0f)
            {
                TrySpawnTrap();
                _spawnTimerSeconds = TrapDifficulty.ResolveSpawnIntervalSeconds(catalog.MinSpawnIntervalSeconds, catalog.MaxSpawnIntervalSeconds, CurrentDifficulty01());
            }
        }

        /// <summary>Resets all host-side spawn bookkeeping for a fresh match. Safe to call any time (e.g. on Create/Leave Match) — same documented seam as WeaponAuthority.ResetForNewMatch.</summary>
        public void ResetForNewMatch()
        {
            _active.Clear();
            _spawnTimerSeconds = 0f;
        }

        private void TrySpawnTrap()
        {
            float difficulty01 = CurrentDifficulty01();
            int maxConcurrent = TrapDifficulty.ResolveMaxConcurrent(catalog.MaxConcurrentTraps, catalog.MaxConcurrentTrapsBonusAtFullDifficulty, difficulty01);
            if (_active.Count >= maxConcurrent)
            {
                return;
            }

            if (!WeightedSelector.TrySelect(catalog.GetWeightedOptions(), _random, out TrapId trapId))
            {
                return;
            }

            TrapDefinition definition = catalog.GetDefinition(trapId);
            if (definition == null)
            {
                return;
            }

            Vector2 origin = LocalPlayerStateService.Current != null ? LocalPlayerStateService.Current.Position : Vector2.zero;
            NetVector2 position = TrapPositionRoll.NextPosition(_random, origin.x, catalog.MinSpawnAheadMeters, catalog.MaxSpawnAheadMeters, catalog.GroundY);

            int instanceId = _nextInstanceId++;
            double lifetimeSeconds = definition.LifetimeSeconds;
            double now = Time.timeAsDouble;
            _active[instanceId] = new ActiveTrap(trapId, now + lifetimeSeconds);

            _transport.BroadcastTrapSpawned(new TrapSpawnEvent(instanceId, trapId, position, lifetimeSeconds, now));
        }

        private void ExpireDueTraps()
        {
            double now = Time.timeAsDouble;
            _expiredScratch.Clear();

            foreach (KeyValuePair<int, ActiveTrap> entry in _active)
            {
                if (now >= entry.Value.ExpireAtSeconds)
                {
                    _expiredScratch.Add(entry.Key);
                }
            }

            for (int i = 0; i < _expiredScratch.Count; i++)
            {
                int instanceId = _expiredScratch[i];
                _active.Remove(instanceId);
                _transport.BroadcastTrapExpired(instanceId);
            }
        }

        private void HandleTrapTriggerReported(TrapTriggerEvent trigger)
        {
            if (_transport == null || !_transport.IsHost)
            {
                return;
            }

            if (!_active.ContainsKey(trigger.TrapInstanceId))
            {
                // The trap already expired server-side (or never existed) —
                // ignore a late/invalid client report instead of confirming a
                // hit against a trap that no longer exists. This is "the
                // server validates all trap events" applied to expiration.
                return;
            }

            _transport.ConfirmTrapTrigger(trigger);
        }

        private static float CurrentDifficulty01() => DifficultyService.Current != null ? DifficultyService.Current.Current01 : 0f;

        private readonly struct ActiveTrap
        {
            public readonly TrapId Trap;
            public readonly double ExpireAtSeconds;

            public ActiveTrap(TrapId trap, double expireAtSeconds)
            {
                Trap = trap;
                ExpireAtSeconds = expireAtSeconds;
            }
        }
    }
}

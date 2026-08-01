using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Managers;
using GulfRun.Core.Networking;
using GulfRun.Core.Pooling;
using GulfRun.Domain;
using GulfRun.Features.Traps.Configuration;
using GulfRun.Features.Traps.Hazards;
using UnityEngine;

namespace GulfRun.Features.Traps.Spawning
{
    /// <summary>Debug-visible snapshot of one active trap instance on this client — see <see cref="TrapSpawnController.ActiveTraps"/>.</summary>
    public readonly struct ActiveTrapView
    {
        public readonly TrapId Trap;
        public readonly GameObject Instance;
        public readonly double ExpireAtSeconds;

        public ActiveTrapView(TrapId trap, GameObject instance, double expireAtSeconds)
        {
            Trap = trap;
            Instance = instance;
            ExpireAtSeconds = expireAtSeconds;
        }
    }

    /// <summary>
    /// Client-side materializer for every Dynamic Trap System network event:
    /// reacts to <c>TrapSpawned</c> by pulling a pooled instance from
    /// <see cref="ObjectPoolManager"/> and configuring it, and to
    /// <c>TrapExpired</c> by releasing it back — never Instantiates/Destroys.
    /// Runs identically on every connected client (including the host's own
    /// scene, since <c>TrapAuthority</c> broadcasts through the same
    /// transport it listens on), so every player sees the exact same trap
    /// layout for a given match. Scene-scoped like
    /// <c>Features.Weapons.Effects.WeaponEffectApplicator</c> since it only
    /// matters during actual gameplay.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TrapSpawnController : SceneSingleton<TrapSpawnController>
    {
        [SerializeField] private TrapCatalogConfig catalog;

        private readonly Dictionary<int, ActiveTrapView> _active = new Dictionary<int, ActiveTrapView>();
        private readonly HashSet<GameObject> _preloadedPrefabs = new HashSet<GameObject>();
        private IMatchTransport _transport;

        /// <summary>Every trap instance currently active on this client, keyed by the host-minted TrapInstanceId — the source of truth for TrapsDebugView's count/positions/lifetime timers.</summary>
        public IReadOnlyDictionary<int, ActiveTrapView> ActiveTraps => _active;

        private void Start()
        {
            if (catalog == null || ObjectPoolManager.Instance == null)
            {
                return;
            }

            IReadOnlyList<TrapDefinition> traps = catalog.Traps;
            for (int i = 0; i < traps.Count; i++)
            {
                GameObject prefab = traps[i] != null ? traps[i].Prefab : null;
                if (prefab != null && _preloadedPrefabs.Add(prefab))
                {
                    ObjectPoolManager.Instance.Preload(prefab, catalog.PreloadCountPerPrefab, transform);
                }
            }
        }

        private void OnEnable()
        {
            _transport = MatchTransportService.Current;
            _transport.TrapSpawned += HandleTrapSpawned;
            _transport.TrapExpired += HandleTrapExpired;
        }

        private void OnDisable()
        {
            if (_transport == null)
            {
                return;
            }

            _transport.TrapSpawned -= HandleTrapSpawned;
            _transport.TrapExpired -= HandleTrapExpired;
        }

        private void HandleTrapSpawned(TrapSpawnEvent spawned)
        {
            TrapDefinition definition = catalog != null ? catalog.GetDefinition(spawned.Trap) : null;
            if (definition == null || definition.Prefab == null || ObjectPoolManager.Instance == null)
            {
                return;
            }

            Vector3 position = new Vector3(spawned.Position.X, spawned.Position.Y, 0f);
            GameObject instance = ObjectPoolManager.Instance.Get(definition.Prefab, position, Quaternion.identity, transform);
            if (instance == null)
            {
                return;
            }

            instance.GetComponent<Trap>()?.Configure(spawned.TrapInstanceId, spawned.Trap, definition);
            _active[spawned.TrapInstanceId] = new ActiveTrapView(spawned.Trap, instance, spawned.TimestampSeconds + spawned.LifetimeSeconds);
            AudioManager.Instance?.PlayOneShot(definition.AppearSound);
        }

        private void HandleTrapExpired(int trapInstanceId)
        {
            if (!_active.TryGetValue(trapInstanceId, out ActiveTrapView view))
            {
                return;
            }

            _active.Remove(trapInstanceId);
            if (view.Instance != null)
            {
                ObjectPoolManager.Instance?.Release(view.Instance);
            }
        }
    }
}

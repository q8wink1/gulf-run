using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Traps.Configuration
{
    /// <summary>
    /// The single source of truth for "which 15 traps exist, how likely is
    /// each, and what are the map's spawn rules" — referenced by
    /// <c>TrapAuthority</c> (the only thing that decides to spawn/expire a
    /// trap) and by <c>TrapSpawnController</c>/<c>TrapEffectApplicator</c>
    /// for trap-id -&gt; definition lookups. Mirrors the role
    /// <c>WeaponCatalogConfig</c> plays for Sprint 5 — one data asset, no
    /// tuning values hardcoded in code, so every "must be configurable"
    /// Randomization requirement is satisfied by editing this asset.
    /// </summary>
    [CreateAssetMenu(fileName = "TrapCatalogConfig", menuName = "GulfRun/Traps/Trap Catalog Config")]
    public sealed class TrapCatalogConfig : ScriptableObject
    {
        [SerializeField] private List<TrapDefinition> traps = new List<TrapDefinition>();

        [Header("Spawn rules (difficulty01 = 0 at race start, 1 fully ramped — Core.Services.IDifficultyProvider)")]
        [Tooltip("Seconds between spawn attempts once difficulty is fully ramped (traps spawn most often here).")]
        [SerializeField] private float minSpawnIntervalSeconds = 4f;

        [Tooltip("Seconds between spawn attempts at race start (difficulty01 = 0).")]
        [SerializeField] private float maxSpawnIntervalSeconds = 9f;

        [Tooltip("Base cap on simultaneously active trap instances.")]
        [SerializeField] private int maxConcurrentTraps = 2;

        [Tooltip("Extra concurrent traps allowed once difficulty is fully ramped, on top of MaxConcurrentTraps.")]
        [SerializeField] private int maxConcurrentTrapsBonusAtFullDifficulty = 2;

        [Tooltip("Minimum distance ahead of the local player a new trap may spawn.")]
        [SerializeField] private float minSpawnAheadMeters = 8f;

        [Tooltip("Maximum distance ahead of the local player a new trap may spawn.")]
        [SerializeField] private float maxSpawnAheadMeters = 20f;

        [SerializeField] private float groundY;

        [Tooltip("Pooled instances preloaded per distinct trap prefab at scene start (Performance: no Instantiate during gameplay).")]
        [SerializeField] private int preloadCountPerPrefab = 4;

        private readonly Dictionary<TrapId, TrapDefinition> _byId = new Dictionary<TrapId, TrapDefinition>();
        private readonly List<WeightedOption<TrapId>> _optionsScratch = new List<WeightedOption<TrapId>>();
        private bool _indexed;

        public IReadOnlyList<TrapDefinition> Traps => traps;
        public float MinSpawnIntervalSeconds => minSpawnIntervalSeconds;
        public float MaxSpawnIntervalSeconds => maxSpawnIntervalSeconds;
        public int MaxConcurrentTraps => maxConcurrentTraps;
        public int MaxConcurrentTrapsBonusAtFullDifficulty => maxConcurrentTrapsBonusAtFullDifficulty;
        public float MinSpawnAheadMeters => minSpawnAheadMeters;
        public float MaxSpawnAheadMeters => maxSpawnAheadMeters;
        public float GroundY => groundY;
        public int PreloadCountPerPrefab => preloadCountPerPrefab;

        public TrapDefinition GetDefinition(TrapId id)
        {
            EnsureIndexed();
            return _byId.TryGetValue(id, out TrapDefinition definition) ? definition : null;
        }

        /// <summary>Weighted options over every configured trap, for <see cref="WeightedSelector"/>. Cached/reused list — not reentrant, matching WeaponCatalogConfig's documented usage.</summary>
        public IReadOnlyList<WeightedOption<TrapId>> GetWeightedOptions()
        {
            _optionsScratch.Clear();
            for (int i = 0; i < traps.Count; i++)
            {
                TrapDefinition definition = traps[i];
                if (definition != null)
                {
                    _optionsScratch.Add(new WeightedOption<TrapId>(definition.Id, definition.SpawnWeight));
                }
            }

            return _optionsScratch;
        }

        private void EnsureIndexed()
        {
            if (_indexed)
            {
                return;
            }

            _byId.Clear();
            for (int i = 0; i < traps.Count; i++)
            {
                if (traps[i] != null)
                {
                    _byId[traps[i].Id] = traps[i];
                }
            }

            _indexed = true;
        }

#if UNITY_EDITOR
        private void OnValidate() => _indexed = false;
#endif
    }
}

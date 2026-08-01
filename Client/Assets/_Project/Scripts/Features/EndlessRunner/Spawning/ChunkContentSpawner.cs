using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Core.Pooling;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.EndlessRunner.Configuration;
using GulfRun.Features.EndlessRunner.WorldGeneration;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.Spawning
{
    /// <summary>
    /// Populates a chunk's <see cref="SpawnPoint"/>s with pooled content once
    /// it becomes active, and releases that content back to the pool when the
    /// chunk is recycled. One weighted <see cref="SpawnCategoryConfig"/> per
    /// <see cref="SpawnCategory"/> keeps Obstacles/Coins/PowerUps/Decorations
    /// fully data-driven and independently swappable (e.g. for a future
    /// seasonal-event content set).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ChunkContentSpawner : SceneSingleton<ChunkContentSpawner>
    {
        [SerializeField] private SpawnCategoryConfig obstacleConfig;
        [SerializeField] private SpawnCategoryConfig coinConfig;
        [SerializeField] private SpawnCategoryConfig powerUpConfig;
        [SerializeField] private SpawnCategoryConfig decorationConfig;

        [Tooltip("Sprint 5: Mystery Item Boxes. Reuses this same pooled, weighted-random, per-chunk pipeline instead of a parallel spawn system — BaseSpawnChance/entry weights ARE the 'configurable respawn rules'.")]
        [SerializeField] private SpawnCategoryConfig itemBoxConfig;

        [Tooltip("0 = non-deterministic (time-seeded). Any other value reproduces the exact same spawn sequence.")]
        [SerializeField] private int randomSeed;

        private IRandomSource _random;
        private IMatchTransport _transport;
        private readonly Dictionary<SpawnCategory, int> _spawnCounts = new Dictionary<SpawnCategory, int>();

        /// <summary>Lifetime count of items spawned per category this run (debug tool).</summary>
        public IReadOnlyDictionary<SpawnCategory, int> SpawnCounts => _spawnCounts;

        protected override void Awake()
        {
            base.Awake();
            _random = randomSeed != 0 ? new SeededRandom(randomSeed) : SeededRandom.FromTime();

            foreach (SpawnCategory category in Enum.GetValues(typeof(SpawnCategory)))
            {
                _spawnCounts[category] = 0;
            }
        }

        private void Start()
        {
            PreloadCategory(obstacleConfig);
            PreloadCategory(coinConfig);
            PreloadCategory(powerUpConfig);
            PreloadCategory(decorationConfig);
            PreloadCategory(itemBoxConfig);
        }

        private void OnEnable()
        {
            _transport = MatchTransportService.Current;
            if (_transport != null)
            {
                _transport.MatchStateChanged += HandleMatchStateChanged;
            }
        }

        private void OnDisable()
        {
            if (_transport != null)
            {
                _transport.MatchStateChanged -= HandleMatchStateChanged;
            }
        }

        /// <summary>
        /// Sprint 12: re-seeds this chunk's shared content RNG — which
        /// includes <see cref="SpawnCategory.ItemBox"/> placement — from the
        /// match's resolved Item Box Seed the instant the race starts
        /// ("Running" always follows the "Countdown" broadcast that
        /// MapEnvironmentManager reacts to, so the seed is guaranteed to
        /// already be resolved here). A no-op fallback to the existing
        /// config-seeded/time-seeded <see cref="_random"/> when no
        /// environment has been resolved (Features.Maps absent from a given
        /// scene/test).
        /// </summary>
        private void HandleMatchStateChanged(MatchState newState)
        {
            if (newState != MatchState.Running)
            {
                return;
            }

            IMapContextProvider mapContext = MapContextService.Current;
            if (mapContext != null && mapContext.HasResolvedEnvironment)
            {
                _random = new SeededRandom(mapContext.Current.Seeds.ItemBoxSeed);
            }
        }

        public void PopulateChunk(Chunk chunk, float difficulty01)
        {
            if (chunk.SpawnPoints == null)
            {
                return;
            }

            for (int i = 0; i < chunk.SpawnPoints.Count; i++)
            {
                SpawnPoint point = chunk.SpawnPoints[i];
                if (point == null)
                {
                    continue;
                }

                SpawnCategoryConfig config = GetConfig(point.Category);
                if (config == null || _random.NextFloat01() > config.BaseSpawnChance)
                {
                    continue;
                }

                if (!WeightedSelector.TrySelect(config.GetWeightedOptions(difficulty01), _random, out GameObject prefab) || prefab == null)
                {
                    continue;
                }

                Transform pointTransform = point.transform;
                GameObject instance = ObjectPoolManager.Instance.Get(prefab, pointTransform.position, pointTransform.rotation, chunk.transform);
                chunk.SpawnedContent.Add(instance);
                _spawnCounts[point.Category] = _spawnCounts[point.Category] + 1;
            }
        }

        public void ClearChunk(Chunk chunk)
        {
            List<GameObject> content = chunk.SpawnedContent;
            for (int i = 0; i < content.Count; i++)
            {
                if (content[i] != null)
                {
                    ObjectPoolManager.Instance.Release(content[i]);
                }
            }

            content.Clear();
        }

        private void PreloadCategory(SpawnCategoryConfig config)
        {
            if (config == null || ObjectPoolManager.Instance == null)
            {
                return;
            }

            IReadOnlyList<SpawnEntry> entries = config.Entries;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Prefab != null)
                {
                    ObjectPoolManager.Instance.Preload(entries[i].Prefab, config.PreloadCountPerPrefab, transform);
                }
            }
        }

        private SpawnCategoryConfig GetConfig(SpawnCategory category)
        {
            switch (category)
            {
                case SpawnCategory.Obstacle: return obstacleConfig;
                case SpawnCategory.Coin: return coinConfig;
                case SpawnCategory.PowerUp: return powerUpConfig;
                case SpawnCategory.Decoration: return decorationConfig;
                case SpawnCategory.ItemBox: return itemBoxConfig;
                default: return null;
            }
        }
    }
}

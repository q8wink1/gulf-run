using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Pooling;
using GulfRun.Domain;
using GulfRun.Features.EndlessRunner.Configuration;
using GulfRun.Features.EndlessRunner.Spawning;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.WorldGeneration
{
    /// <summary>
    /// Drives infinite level generation: spawns pooled chunks ahead of the
    /// current run distance and recycles chunks that have fallen behind it.
    /// Chunk selection is weighted-random and seedable (future biome
    /// compatibility: adding a biome is a data change to
    /// <see cref="WorldGenerationConfig"/>, never a code change here).
    ///
    /// Distance-driven rather than transform-driven: this generator never
    /// looks at the Player directly (features must not reference each other),
    /// it only needs "how far has the run progressed" from
    /// <see cref="Distance.DistanceTracker"/>, which is itself derived from
    /// the deterministic Game Speed Controller — not raw physics — so world
    /// generation stays reproducible.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WorldGenerator : SceneSingleton<WorldGenerator>
    {
        [SerializeField] private WorldGenerationConfig config;
        [SerializeField] private Transform chunkParent;

        private readonly Queue<Chunk> _activeChunks = new Queue<Chunk>();
        private IRandomSource _random;
        private float _frontierX;

        /// <summary>Number of chunks currently active in the world (debug tool).</summary>
        public int ActiveChunkCount => _activeChunks.Count;

        /// <summary>Most recently spawned chunk (debug tool).</summary>
        public Chunk LatestChunk { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            _random = config.Seed != 0 ? new SeededRandom(config.Seed) : SeededRandom.FromTime();
        }

        private void Start()
        {
            PreloadChunkPools();
            FillInitialBuffer();
        }

        /// <summary>
        /// Advances world generation for the current frame. Must only be
        /// called while the game loop is in the Running state.
        /// </summary>
        public void Tick(double distanceMeters, float difficulty01)
        {
            SpawnAheadOf(distanceMeters, difficulty01);
            CleanupBehind(distanceMeters);
        }

        /// <summary>Releases every active chunk and re-seeds the initial buffer. Called by the game loop on Restart.</summary>
        public void ResetGenerator()
        {
            while (_activeChunks.Count > 0)
            {
                ReleaseChunk(_activeChunks.Dequeue());
            }

            _frontierX = 0f;
            LatestChunk = null;
            FillInitialBuffer();
        }

        private void PreloadChunkPools()
        {
            if (ObjectPoolManager.Instance == null)
            {
                return;
            }

            IReadOnlyList<ChunkEntry> entries = config.ChunkPrefabs;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Prefab != null)
                {
                    ObjectPoolManager.Instance.Preload(entries[i].Prefab, config.PreloadCountPerChunkPrefab, chunkParent);
                }
            }
        }

        private void FillInitialBuffer()
        {
            while (_frontierX < config.SpawnAheadBufferMeters)
            {
                if (!SpawnNextChunk(difficulty01: 0f))
                {
                    break;
                }
            }
        }

        private void SpawnAheadOf(double distanceMeters, float difficulty01)
        {
            while (_frontierX < distanceMeters + config.SpawnAheadBufferMeters)
            {
                if (!SpawnNextChunk(difficulty01))
                {
                    break;
                }
            }
        }

        private void CleanupBehind(double distanceMeters)
        {
            while (_activeChunks.Count > 0)
            {
                Chunk oldest = _activeChunks.Peek();
                float oldestEndX = oldest.transform.position.x + oldest.Length;
                if (oldestEndX < distanceMeters - config.CleanupBehindBufferMeters)
                {
                    _activeChunks.Dequeue();
                    ReleaseChunk(oldest);
                }
                else
                {
                    break;
                }
            }
        }

        private bool SpawnNextChunk(float difficulty01)
        {
            if (ObjectPoolManager.Instance == null)
            {
                return false;
            }

            if (!WeightedSelector.TrySelect(config.GetWeightedChunkPrefabs(), _random, out GameObject prefab) || prefab == null)
            {
                return false;
            }

            Vector3 position = new Vector3(_frontierX, 0f, 0f);
            GameObject instance = ObjectPoolManager.Instance.Get(prefab, position, Quaternion.identity, chunkParent);
            Chunk chunk = instance.GetComponent<Chunk>();
            if (chunk == null)
            {
                Debug.LogError($"Chunk prefab '{prefab.name}' has no Chunk component.", instance);
                ObjectPoolManager.Instance.Release(instance);
                return false;
            }

            _frontierX += chunk.Length;
            _activeChunks.Enqueue(chunk);
            LatestChunk = chunk;

            if (ChunkContentSpawner.Instance != null)
            {
                ChunkContentSpawner.Instance.PopulateChunk(chunk, difficulty01);
            }

            return true;
        }

        private void ReleaseChunk(Chunk chunk)
        {
            if (ChunkContentSpawner.Instance != null)
            {
                ChunkContentSpawner.Instance.ClearChunk(chunk);
            }

            ObjectPoolManager.Instance.Release(chunk.gameObject);
        }
    }
}

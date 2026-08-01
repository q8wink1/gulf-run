using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.Configuration
{
    /// <summary>One selectable chunk prefab variant and its relative spawn weight.</summary>
    [Serializable]
    public sealed class ChunkEntry
    {
        public GameObject Prefab;
        public float Weight = 1f;
    }

    /// <summary>
    /// Tuning values for the endless world generator. Every number that would
    /// otherwise be hardcoded lives here, including the chunk-prefab list
    /// itself — adding a new biome later is purely a data change (new
    /// <see cref="ChunkEntry"/> entries), never a code change.
    /// </summary>
    [CreateAssetMenu(
        fileName = "WorldGenerationConfig",
        menuName = "GulfRun/EndlessRunner/World Generation Config")]
    public sealed class WorldGenerationConfig : ScriptableObject
    {
        [Header("Chunk Prefabs (future biome compatibility: add more weighted entries)")]
        [SerializeField] private List<ChunkEntry> chunkPrefabs = new List<ChunkEntry>();

        [Header("Generation")]
        [Tooltip("How far ahead of the current distance a chunk must be spawned.")]
        [SerializeField] private float spawnAheadBufferMeters = 40f;

        [Tooltip("How far behind the current distance a chunk must fall before it is recycled.")]
        [SerializeField] private float cleanupBehindBufferMeters = 20f;

        [SerializeField] private int preloadCountPerChunkPrefab = 3;

        [Tooltip("0 = non-deterministic (time-seeded). Any other value reproduces the exact same chunk sequence.")]
        [SerializeField] private int seed;

        private readonly List<WeightedOption<GameObject>> _scratch = new List<WeightedOption<GameObject>>();

        public IReadOnlyList<ChunkEntry> ChunkPrefabs => chunkPrefabs;
        public float SpawnAheadBufferMeters => spawnAheadBufferMeters;
        public float CleanupBehindBufferMeters => cleanupBehindBufferMeters;
        public int PreloadCountPerChunkPrefab => preloadCountPerChunkPrefab;
        public int Seed => seed;

        /// <summary>
        /// Builds (and caches, to avoid per-call allocation) the weighted
        /// option list consumed by <see cref="WeightedSelector"/>. Not
        /// thread-safe / not reentrant — only ever called from the main
        /// thread by a single WorldGenerator, which is the intended usage.
        /// </summary>
        public IReadOnlyList<WeightedOption<GameObject>> GetWeightedChunkPrefabs()
        {
            _scratch.Clear();
            for (int i = 0; i < chunkPrefabs.Count; i++)
            {
                if (chunkPrefabs[i].Prefab != null)
                {
                    _scratch.Add(new WeightedOption<GameObject>(chunkPrefabs[i].Prefab, chunkPrefabs[i].Weight));
                }
            }

            return _scratch;
        }
    }
}

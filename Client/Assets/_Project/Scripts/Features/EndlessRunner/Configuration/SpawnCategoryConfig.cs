using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.Configuration
{
    /// <summary>One spawnable prefab option within a category's weighted table.</summary>
    [Serializable]
    public sealed class SpawnEntry
    {
        public GameObject Prefab;
        public float BaseWeight = 1f;

        [Tooltip("1 = weight is unaffected by difficulty. >1 = becomes more common as difficulty rises. <1 = becomes rarer.")]
        [Range(0f, 5f)]
        public float DifficultyWeightMultiplier = 1f;
    }

    /// <summary>
    /// Weighted spawn table for a single <see cref="SpawnCategory"/>
    /// (Obstacle, Coin, PowerUp, or Decoration — one asset instance per
    /// category). Supports difficulty scaling per entry and seasonal-event
    /// swaps by simply assigning a different config asset at runtime.
    /// </summary>
    [CreateAssetMenu(
        fileName = "SpawnCategoryConfig",
        menuName = "GulfRun/EndlessRunner/Spawn Category Config")]
    public sealed class SpawnCategoryConfig : ScriptableObject
    {
        [SerializeField] private List<SpawnEntry> entries = new List<SpawnEntry>();

        [Tooltip("Chance [0..1] that a spawn point of this category spawns anything at all.")]
        [Range(0f, 1f)]
        [SerializeField] private float baseSpawnChance = 1f;

        [SerializeField] private int preloadCountPerPrefab = 4;

        private readonly List<WeightedOption<GameObject>> _scratch = new List<WeightedOption<GameObject>>();

        public IReadOnlyList<SpawnEntry> Entries => entries;
        public float BaseSpawnChance => baseSpawnChance;
        public int PreloadCountPerPrefab => preloadCountPerPrefab;

        /// <summary>
        /// Builds (and caches, to avoid per-call allocation) the weighted
        /// option list for the given normalized difficulty. Not thread-safe /
        /// not reentrant — only ever called from the main thread by a single
        /// ChunkContentSpawner, which is the intended usage.
        /// </summary>
        public IReadOnlyList<WeightedOption<GameObject>> GetWeightedOptions(float difficulty01)
        {
            _scratch.Clear();
            for (int i = 0; i < entries.Count; i++)
            {
                SpawnEntry entry = entries[i];
                if (entry.Prefab == null)
                {
                    continue;
                }

                float effectiveWeight = entry.BaseWeight * Mathf.Lerp(1f, entry.DifficultyWeightMultiplier, difficulty01);
                _scratch.Add(new WeightedOption<GameObject>(entry.Prefab, effectiveWeight));
            }

            return _scratch;
        }
    }
}

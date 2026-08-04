using System;
using System.Collections.Generic;
using GulfRun.Core.Pooling;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.9 / 23.10 — one catalog row: authoring data + prefab + pool preload.
    /// </summary>
    [Serializable]
    public sealed class ObstacleCatalogEntry
    {
        [SerializeField] private ObstacleData data;
        [SerializeField] private GameObject prefab;
        [Tooltip("Instances to Preload when WarmPools runs (0 = skip).")]
        [SerializeField] private int preloadCount = 4;

        public ObstacleData Data => data;
        public GameObject Prefab => prefab;
        public int PreloadCount => preloadCount < 0 ? 0 : preloadCount;
    }

    /// <summary>
    /// Sprint 23.9 / 23.10 — map / session obstacle prefab catalog.
    /// Weighted pick + difficulty filter feed SpawnManager pool execution.
    /// </summary>
    [CreateAssetMenu(
        fileName = "ObstacleCatalog",
        menuName = "GulfRun/Gameplay/Obstacle Catalog")]
    public sealed class ObstacleCatalog : ScriptableObject
    {
        [SerializeField] private List<ObstacleCatalogEntry> entries = new List<ObstacleCatalogEntry>();

        [NonSerialized] private List<WeightedOption<ObstacleCatalogEntry>> _pickScratch;

        public IReadOnlyList<ObstacleCatalogEntry> Entries => entries;

        public bool TryGetPrefab(ObstacleData data, out GameObject prefab)
        {
            prefab = null;
            if (data == null || entries == null)
            {
                return false;
            }

            for (int i = 0; i < entries.Count; i++)
            {
                ObstacleCatalogEntry entry = entries[i];
                if (entry != null && entry.Data == data && entry.Prefab != null)
                {
                    prefab = entry.Prefab;
                    return true;
                }
            }

            return false;
        }

        /// <summary>First prefab matching <paramref name="type"/>, or null.</summary>
        public GameObject GetPrefabForType(ObstacleType type)
        {
            if (entries == null)
            {
                return null;
            }

            for (int i = 0; i < entries.Count; i++)
            {
                ObstacleCatalogEntry entry = entries[i];
                if (entry == null || entry.Prefab == null || entry.Data == null)
                {
                    continue;
                }

                if (entry.Data.ObstacleType == type)
                {
                    return entry.Prefab;
                }
            }

            return null;
        }

        /// <summary>
        /// Weighted pick among entries whose data difficulty fits the session tier.
        /// Balancing of weights / spacing is deferred — filter only.
        /// </summary>
        public bool TryPickEntry(
            IRandomSource random,
            ObstacleDifficultyLevel difficulty,
            out ObstacleCatalogEntry entry)
        {
            entry = null;
            if (entries == null || entries.Count == 0 || random == null)
            {
                return false;
            }

            int maxDifficulty = ObstacleDifficultyLevelRules.MaxObstacleDataDifficulty(difficulty);
            if (_pickScratch == null)
            {
                _pickScratch = new List<WeightedOption<ObstacleCatalogEntry>>(8);
            }

            _pickScratch.Clear();

            for (int i = 0; i < entries.Count; i++)
            {
                ObstacleCatalogEntry candidate = entries[i];
                if (candidate == null || candidate.Prefab == null || candidate.Data == null)
                {
                    continue;
                }

                if (candidate.Data.Difficulty > maxDifficulty)
                {
                    continue;
                }

                float weight = candidate.Data.SpawnWeight;
                if (weight <= 0f)
                {
                    continue;
                }

                _pickScratch.Add(new WeightedOption<ObstacleCatalogEntry>(candidate, weight));
            }

            if (_pickScratch.Count == 0)
            {
                // Fallback: any valid prefab entry (difficulty prep may leave Easy empty).
                for (int i = 0; i < entries.Count; i++)
                {
                    ObstacleCatalogEntry candidate = entries[i];
                    if (candidate == null || candidate.Prefab == null || candidate.Data == null)
                    {
                        continue;
                    }

                    float weight = candidate.Data.SpawnWeight > 0f ? candidate.Data.SpawnWeight : 1f;
                    _pickScratch.Add(new WeightedOption<ObstacleCatalogEntry>(candidate, weight));
                }
            }

            return WeightedSelector.TrySelect(_pickScratch, random, out entry) && entry != null;
        }

        /// <summary>
        /// Preloads pooled instances for each entry. Safe no-op when pool manager missing.
        /// </summary>
        public void WarmPools(ObjectPoolManager pools, Transform parent = null)
        {
            if (pools == null || entries == null)
            {
                return;
            }

            for (int i = 0; i < entries.Count; i++)
            {
                ObstacleCatalogEntry entry = entries[i];
                if (entry == null || entry.Prefab == null || entry.PreloadCount <= 0)
                {
                    continue;
                }

                pools.Preload(entry.Prefab, entry.PreloadCount, parent);
            }
        }
    }
}

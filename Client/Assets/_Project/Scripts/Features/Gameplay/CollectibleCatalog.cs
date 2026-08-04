using System;
using System.Collections.Generic;
using GulfRun.Core.Pooling;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>Sprint 23.12 — one catalog row: type + prefab + pool preload.</summary>
    [Serializable]
    public sealed class CollectibleCatalogEntry
    {
        [SerializeField] private CollectibleType type = CollectibleType.Coin;
        [SerializeField] private GameObject prefab;
        [Tooltip("Instances to Preload when WarmPools runs (0 = skip).")]
        [SerializeField] private int preloadCount = 12;

        public CollectibleType Type => type;
        public GameObject Prefab => prefab;
        public int PreloadCount => preloadCount < 0 ? 0 : preloadCount;
    }

    /// <summary>
    /// Sprint 23.12 — Coin / Gem prefab catalog for SpawnManager pool execution.
    /// </summary>
    [CreateAssetMenu(
        fileName = "CollectibleCatalog",
        menuName = "GulfRun/Gameplay/Collectible Catalog")]
    public sealed class CollectibleCatalog : ScriptableObject
    {
        [SerializeField] private List<CollectibleCatalogEntry> entries = new List<CollectibleCatalogEntry>();

        public IReadOnlyList<CollectibleCatalogEntry> Entries => entries;

        public bool TryGetPrefab(CollectibleType type, out GameObject prefab)
        {
            prefab = null;
            if (entries == null)
            {
                return false;
            }

            for (int i = 0; i < entries.Count; i++)
            {
                CollectibleCatalogEntry entry = entries[i];
                if (entry != null && entry.Type == type && entry.Prefab != null)
                {
                    prefab = entry.Prefab;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetEntry(CollectibleType type, out CollectibleCatalogEntry entry)
        {
            entry = null;
            if (entries == null)
            {
                return false;
            }

            for (int i = 0; i < entries.Count; i++)
            {
                CollectibleCatalogEntry candidate = entries[i];
                if (candidate != null && candidate.Type == type && candidate.Prefab != null)
                {
                    entry = candidate;
                    return true;
                }
            }

            return false;
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
                CollectibleCatalogEntry entry = entries[i];
                if (entry == null || entry.Prefab == null || entry.PreloadCount <= 0)
                {
                    continue;
                }

                pools.Preload(entry.Prefab, entry.PreloadCount, parent);
            }
        }
    }
}

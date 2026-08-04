using System;
using System.Collections.Generic;
using GulfRun.Core.Pooling;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.9 — one catalog row: authoring data + prefab + pool preload count.
    /// Prefabs are hooks for <see cref="SpawnManager.WarmPools"/>; nothing is spawned yet.
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
    /// Sprint 23.9 — map / session obstacle prefab catalog for SpawnManager / RaceManager.
    /// Weighted selection and random spawn are intentionally not implemented.
    /// </summary>
    [CreateAssetMenu(
        fileName = "ObstacleCatalog",
        menuName = "GulfRun/Gameplay/Obstacle Catalog")]
    public sealed class ObstacleCatalog : ScriptableObject
    {
        [SerializeField] private List<ObstacleCatalogEntry> entries = new List<ObstacleCatalogEntry>();

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
        /// Preloads pooled instances for each entry. Safe no-op when pool manager missing.
        /// Does not place obstacles on the track.
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

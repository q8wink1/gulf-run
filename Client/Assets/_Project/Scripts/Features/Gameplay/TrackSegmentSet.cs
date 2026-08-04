using System;
using System.Collections.Generic;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>One selectable track segment prefab (weight reserved for future random maps).</summary>
    [Serializable]
    public sealed class TrackSegmentEntry
    {
        public GameObject Prefab;
        [Min(0f)] public float Weight = 1f;
    }

    /// <summary>
    /// Sprint 23.6 — per-map (or per-biome) list of track segment prefabs.
    /// Swap this asset on <see cref="EndlessTrackGenerator"/> to give each Gulf
    /// map its own track set without changing the generator.
    /// </summary>
    [CreateAssetMenu(
        fileName = "TrackSegmentSet",
        menuName = "GulfRun/Gameplay/Track Segment Set")]
    public sealed class TrackSegmentSet : ScriptableObject
    {
        [Tooltip("Stable id for map catalogs (e.g. DubaiSkyline, DesertDunes).")]
        [SerializeField] private string setId = "Default";

        [SerializeField] private List<TrackSegmentEntry> segments = new List<TrackSegmentEntry>();

        public string SetId => setId;
        public IReadOnlyList<TrackSegmentEntry> Segments => segments;
        public int Count => segments != null ? segments.Count : 0;

        /// <summary>Valid prefab at index, or null.</summary>
        public GameObject GetPrefab(int index)
        {
            if (segments == null || index < 0 || index >= segments.Count)
            {
                return null;
            }

            TrackSegmentEntry entry = segments[index];
            return entry != null ? entry.Prefab : null;
        }

        /// <summary>
        /// Future hook: weighted-random pick. Today unused — generator alternates.
        /// Returns false when the set is empty.
        /// </summary>
        public bool TrySelectWeighted(System.Random rng, out GameObject prefab)
        {
            prefab = null;
            if (segments == null || segments.Count == 0 || rng == null)
            {
                return false;
            }

            float total = 0f;
            for (int i = 0; i < segments.Count; i++)
            {
                if (segments[i] != null && segments[i].Prefab != null && segments[i].Weight > 0f)
                {
                    total += segments[i].Weight;
                }
            }

            if (total <= 0f)
            {
                return false;
            }

            float roll = (float)(rng.NextDouble() * total);
            float cursor = 0f;
            for (int i = 0; i < segments.Count; i++)
            {
                TrackSegmentEntry entry = segments[i];
                if (entry == null || entry.Prefab == null || entry.Weight <= 0f)
                {
                    continue;
                }

                cursor += entry.Weight;
                if (roll <= cursor)
                {
                    prefab = entry.Prefab;
                    return true;
                }
            }

            prefab = segments[segments.Count - 1].Prefab;
            return prefab != null;
        }
    }
}

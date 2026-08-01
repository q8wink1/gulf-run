using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Maps.Configuration
{
    /// <summary>
    /// The single source of truth for the six Sprint 12 launch maps: display
    /// identity, the country it flies (reuses <see cref="GulfCountry"/> —
    /// each launch map is exactly one GCC nation's flagship city, so a
    /// second country catalog would be redundant), its background-only
    /// landmark list, which optional animated background elements it
    /// supports, and its ambient day/night audio. Landmarks and background
    /// elements are explicitly presentation-only (Sprint 12 brief:
    /// "Landmarks are background visuals only. They never affect
    /// gameplay.") — nothing here is ever read by gameplay/collision code,
    /// only by Features.Maps' own audio/background/debug components.
    ///
    /// Every launch map shares the exact same reusable chunk-prefab library
    /// (see <c>Features.EndlessRunner.Configuration.WorldGenerationConfig</c>)
    /// — per-map identity is presentation, not track geometry, which is
    /// what keeps six maps "fair and balanced" (brief) for free and is the
    /// Code Quality section's "reusable level components" applied literally.
    /// </summary>
    [CreateAssetMenu(fileName = "MapCatalogConfig", menuName = "GulfRun/Maps/Map Catalog Config")]
    public sealed class MapCatalogConfig : ScriptableObject
    {
        [Serializable]
        public sealed class LandmarkEntry
        {
            [SerializeField] private string entryName = string.Empty;
            [SerializeField] private Color placeholderColor = Color.white;

            [Tooltip("0 = distant background layer (scrolls slowest), 1 = nearest background layer.")]
            [Range(0f, 1f)]
            [SerializeField] private float parallaxDepth = 0.2f;

            public string Name => entryName;
            public Color PlaceholderColor => placeholderColor;
            public float ParallaxDepth => parallaxDepth;
        }

        [Serializable]
        public sealed class MapEntry
        {
            [SerializeField] private string mapId = string.Empty;
            [SerializeField] private string displayName = string.Empty;
            [SerializeField] private GulfCountry country;

            [Tooltip("Relative chance this map is picked for a given match. Equal weights = a uniform random rotation across all six launch maps.")]
            [SerializeField] private float selectionWeight = 1f;

            [SerializeField] private Color paletteColor = Color.white;

            [SerializeField] private BackgroundElementFlags backgroundElements =
                BackgroundElementFlags.Clouds | BackgroundElementFlags.Birds;

            [SerializeField] private List<LandmarkEntry> landmarks = new List<LandmarkEntry>();

            [SerializeField] private AudioClip dayAmbientClip;
            [SerializeField] private AudioClip nightAmbientClip;

            public MapId MapId => new MapId(mapId);
            public string DisplayName => string.IsNullOrEmpty(displayName) ? mapId : displayName;
            public GulfCountry Country => country;
            public float SelectionWeight => selectionWeight;
            public Color PaletteColor => paletteColor;
            public BackgroundElementFlags BackgroundElements => backgroundElements;
            public IReadOnlyList<LandmarkEntry> Landmarks => landmarks;
            public AudioClip DayAmbientClip => dayAmbientClip;
            public AudioClip NightAmbientClip => nightAmbientClip;
        }

        [SerializeField] private List<MapEntry> maps = new List<MapEntry>();

        private readonly List<WeightedOption<MapEntry>> _optionsScratch = new List<WeightedOption<MapEntry>>();

        public IReadOnlyList<MapEntry> Maps => maps;

        public bool TryGetEntry(MapId mapId, out MapEntry entry)
        {
            for (int i = 0; i < maps.Count; i++)
            {
                if (maps[i].MapId == mapId)
                {
                    entry = maps[i];
                    return true;
                }
            }

            entry = null;
            return false;
        }

        /// <summary>Weighted options over every configured map, for <see cref="WeightedSelector"/>. Cached/reused list — not reentrant, same documented usage as every other catalog's GetWeighted* method.</summary>
        public IReadOnlyList<WeightedOption<MapEntry>> GetWeightedMaps()
        {
            _optionsScratch.Clear();
            for (int i = 0; i < maps.Count; i++)
            {
                _optionsScratch.Add(new WeightedOption<MapEntry>(maps[i], maps[i].SelectionWeight));
            }

            return _optionsScratch;
        }
    }
}

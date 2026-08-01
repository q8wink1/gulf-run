using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Maps.Configuration
{
    /// <summary>
    /// Sprint 12 "WEATHER: Visual only" catalog. Rain/Fog/Sandstorm rows are
    /// authored with <see cref="WeatherEntry.SelectionWeight"/> at 0 so
    /// <c>MapEnvironmentManager</c> never rolls them today, exactly matching
    /// the brief's "Future support" wording — enabling one later is a
    /// weight edit, never a code change.
    /// </summary>
    [CreateAssetMenu(fileName = "WeatherCatalogConfig", menuName = "GulfRun/Maps/Weather Catalog Config")]
    public sealed class WeatherCatalogConfig : ScriptableObject
    {
        [Serializable]
        public sealed class WeatherEntry
        {
            [SerializeField] private WeatherType weather;
            [SerializeField] private float selectionWeight = 1f;
            [SerializeField] private Color tintColor = Color.white;

            [Tooltip("0 = no fog. Drives RenderSettings.fogDensity; kept low/subtle since this is visual-only (Dusty Sky, Light Wind haze).")]
            [Range(0f, 1f)]
            [SerializeField] private float fogDensity01;

            public WeatherType Weather => weather;
            public float SelectionWeight => selectionWeight;
            public Color TintColor => tintColor;
            public float FogDensity01 => fogDensity01;
        }

        [SerializeField] private List<WeatherEntry> weathers = new List<WeatherEntry>();

        private readonly List<WeightedOption<WeatherEntry>> _optionsScratch = new List<WeightedOption<WeatherEntry>>();

        public IReadOnlyList<WeatherEntry> Weathers => weathers;

        public bool TryGetEntry(WeatherType weather, out WeatherEntry entry)
        {
            for (int i = 0; i < weathers.Count; i++)
            {
                if (weathers[i].Weather == weather)
                {
                    entry = weathers[i];
                    return true;
                }
            }

            entry = null;
            return false;
        }

        /// <summary>Weighted options over every configured weather, for <see cref="WeightedSelector"/>. Cached/reused list — not reentrant.</summary>
        public IReadOnlyList<WeightedOption<WeatherEntry>> GetWeightedWeathers()
        {
            _optionsScratch.Clear();
            for (int i = 0; i < weathers.Count; i++)
            {
                _optionsScratch.Add(new WeightedOption<WeatherEntry>(weathers[i], weathers[i].SelectionWeight));
            }

            return _optionsScratch;
        }
    }
}

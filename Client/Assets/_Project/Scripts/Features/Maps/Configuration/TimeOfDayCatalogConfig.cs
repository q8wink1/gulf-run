using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Maps.Configuration
{
    /// <summary>
    /// Sprint 12 "TIME OF DAY: ... Lighting changes only" catalog — each
    /// entry is purely an ambient-lighting recipe applied by
    /// <c>MapEnvironmentManager</c> via <c>RenderSettings.ambientLight</c>
    /// (plus a sky gradient hint for a future skybox pass), never anything
    /// gameplay-affecting.
    /// </summary>
    [CreateAssetMenu(fileName = "TimeOfDayCatalogConfig", menuName = "GulfRun/Maps/Time Of Day Catalog Config")]
    public sealed class TimeOfDayCatalogConfig : ScriptableObject
    {
        [Serializable]
        public sealed class TimeOfDayEntry
        {
            [SerializeField] private TimeOfDay timeOfDay;
            [SerializeField] private float selectionWeight = 1f;
            [SerializeField] private Color ambientLightColor = Color.white;

            [Range(0f, 3f)]
            [SerializeField] private float ambientIntensity = 1f;

            [Tooltip("Future skybox/gradient pass hint — not applied to any real skybox material today.")]
            [SerializeField] private Color skyTopColor = Color.white;

            [SerializeField] private Color skyBottomColor = Color.white;

            public TimeOfDay TimeOfDay => timeOfDay;
            public float SelectionWeight => selectionWeight;
            public Color AmbientLightColor => ambientLightColor;
            public float AmbientIntensity => ambientIntensity;
            public Color SkyTopColor => skyTopColor;
            public Color SkyBottomColor => skyBottomColor;
        }

        [SerializeField] private List<TimeOfDayEntry> timesOfDay = new List<TimeOfDayEntry>();

        private readonly List<WeightedOption<TimeOfDayEntry>> _optionsScratch = new List<WeightedOption<TimeOfDayEntry>>();

        public IReadOnlyList<TimeOfDayEntry> TimesOfDay => timesOfDay;

        public bool TryGetEntry(TimeOfDay timeOfDay, out TimeOfDayEntry entry)
        {
            for (int i = 0; i < timesOfDay.Count; i++)
            {
                if (timesOfDay[i].TimeOfDay == timeOfDay)
                {
                    entry = timesOfDay[i];
                    return true;
                }
            }

            entry = null;
            return false;
        }

        /// <summary>Weighted options over every configured time of day, for <see cref="WeightedSelector"/>. Cached/reused list — not reentrant.</summary>
        public IReadOnlyList<WeightedOption<TimeOfDayEntry>> GetWeightedTimesOfDay()
        {
            _optionsScratch.Clear();
            for (int i = 0; i < timesOfDay.Count; i++)
            {
                _optionsScratch.Add(new WeightedOption<TimeOfDayEntry>(timesOfDay[i], timesOfDay[i].SelectionWeight));
            }

            return _optionsScratch;
        }
    }
}

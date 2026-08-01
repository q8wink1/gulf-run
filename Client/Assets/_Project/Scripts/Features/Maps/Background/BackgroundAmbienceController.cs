using System.Collections.Generic;
using GulfRun.Domain;
using GulfRun.Features.Maps.Configuration;
using UnityEngine;

namespace GulfRun.Features.Maps.Background
{
    /// <summary>
    /// Sprint 12 "BACKGROUND" capability surface: exposes which animated
    /// elements (clouds/birds/palm trees/flags/city lights/traffic/sea
    /// waves) the active map supports, and toggles the night-only ones
    /// (city lights) with the resolved Time of Day. Scene-scoped so it
    /// resets cleanly per Gameplay load. Every actual moving layer is a
    /// <see cref="ParallaxLayer"/> placed under this controller once real
    /// art exists — this controller only owns the data-driven on/off +
    /// query surface, never a hardcoded per-city branch (Code Quality:
    /// "No hardcoded map logic").
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BackgroundAmbienceController : MonoBehaviour
    {
        [Tooltip("Optional override. Defaults to MapEnvironmentManager.Instance.MapCatalog when unset.")]
        [SerializeField] private MapCatalogConfig mapCatalog;

        [Tooltip("City-lights-at-night layers to show only when the active map supports CityLightsAtNight AND the resolved time of day is Night.")]
        [SerializeField] private List<GameObject> cityLightsAtNightLayers = new List<GameObject>();

        public BackgroundElementFlags ActiveElements { get; private set; }
        public TimeOfDay ActiveTimeOfDay { get; private set; }

        private void OnEnable()
        {
            if (MapEnvironmentManager.Instance == null)
            {
                return;
            }

            MapEnvironmentManager.Instance.EnvironmentResolved += HandleEnvironmentResolved;

            if (MapEnvironmentManager.Instance.HasResolvedEnvironment)
            {
                HandleEnvironmentResolved(MapEnvironmentManager.Instance.Current);
            }
        }

        private void OnDisable()
        {
            if (MapEnvironmentManager.Instance != null)
            {
                MapEnvironmentManager.Instance.EnvironmentResolved -= HandleEnvironmentResolved;
            }
        }

        public bool IsElementActive(BackgroundElementFlags element) => (ActiveElements & element) == element;

        private void HandleEnvironmentResolved(MatchEnvironmentSelection selection)
        {
            MapCatalogConfig catalog = mapCatalog != null
                ? mapCatalog
                : (MapEnvironmentManager.Instance != null ? MapEnvironmentManager.Instance.MapCatalog : null);

            ActiveTimeOfDay = selection.TimeOfDay;
            ActiveElements = catalog != null && catalog.TryGetEntry(selection.Map, out MapCatalogConfig.MapEntry entry)
                ? entry.BackgroundElements
                : BackgroundElementFlags.None;

            bool showCityLights = ActiveTimeOfDay == TimeOfDay.Night && IsElementActive(BackgroundElementFlags.CityLightsAtNight);
            for (int i = 0; i < cityLightsAtNightLayers.Count; i++)
            {
                if (cityLightsAtNightLayers[i] != null)
                {
                    cityLightsAtNightLayers[i].SetActive(showCityLights);
                }
            }
        }
    }
}

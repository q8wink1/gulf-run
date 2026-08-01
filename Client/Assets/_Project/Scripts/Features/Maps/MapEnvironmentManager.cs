using System;
using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Maps.Configuration;
using UnityEngine;

namespace GulfRun.Features.Maps
{
    /// <summary>
    /// Composition root for Sprint 12: resolves a brand new
    /// <see cref="MatchEnvironmentSelection"/> (Map / Weather / Time of Day
    /// / Trap+Item Box seeds) the instant a match enters
    /// <see cref="MatchState.Countdown"/> — strictly before
    /// <see cref="MatchState.Running"/>, which is when every other
    /// re-seeding listener (<c>TrapAuthority</c>, <c>ChunkContentSpawner</c>)
    /// reacts, so this resolution is always visible to them with no
    /// same-frame ordering dependency between independent singletons.
    /// Applies the resolved Time of Day/Weather to ambient lighting
    /// immediately (Sprint 12: "Lighting changes only. Gameplay remains
    /// identical.") and implements <see cref="IMapContextProvider"/> so
    /// every other Feature can read the result without ever referencing
    /// Features.Maps. Persistent (Boot-scene, alongside the other
    /// match-spanning authorities).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MapEnvironmentManager : Singleton<MapEnvironmentManager>, IMapContextProvider
    {
        [SerializeField] private MapCatalogConfig mapCatalog;
        [SerializeField] private WeatherCatalogConfig weatherCatalog;
        [SerializeField] private TimeOfDayCatalogConfig timeOfDayCatalog;

        private IMatchTransport _transport;
        private IRandomSource _random;

        public bool HasResolvedEnvironment { get; private set; }
        public MatchEnvironmentSelection Current { get; private set; }

        public event Action<MatchEnvironmentSelection> EnvironmentResolved;

        public MapCatalogConfig MapCatalog => mapCatalog;
        public WeatherCatalogConfig WeatherCatalog => weatherCatalog;
        public TimeOfDayCatalogConfig TimeOfDayCatalog => timeOfDayCatalog;

        protected override void OnInitialize()
        {
            _random = SeededRandom.FromTime();
            MapContextService.Current = this;
        }

        private void OnEnable()
        {
            _transport = MatchTransportService.Current;
            if (_transport != null)
            {
                _transport.MatchStateChanged += HandleMatchStateChanged;
            }
        }

        private void OnDisable()
        {
            if (_transport != null)
            {
                _transport.MatchStateChanged -= HandleMatchStateChanged;
            }

            if (ReferenceEquals(MapContextService.Current, this))
            {
                MapContextService.Current = null;
            }
        }

        private void HandleMatchStateChanged(MatchState newState)
        {
            if (newState == MatchState.Countdown)
            {
                ResolveNewEnvironment();
            }
        }

        /// <summary>
        /// Rolls a brand new weighted Map/Weather/Time of Day and two fresh
        /// random seeds, applies lighting immediately, and notifies every
        /// listener. Public so a future host-only map-select/vote screen can
        /// call this directly instead of only reacting to Countdown.
        /// </summary>
        public void ResolveNewEnvironment()
        {
            if (mapCatalog == null || weatherCatalog == null || timeOfDayCatalog == null)
            {
                return;
            }

            if (!WeightedSelector.TrySelect(mapCatalog.GetWeightedMaps(), _random, out MapCatalogConfig.MapEntry mapEntry) || mapEntry == null)
            {
                return;
            }

            WeatherCatalogConfig.WeatherEntry weatherEntry = null;
            WeatherType weather = WeightedSelector.TrySelect(weatherCatalog.GetWeightedWeathers(), _random, out weatherEntry) && weatherEntry != null
                ? weatherEntry.Weather
                : WeatherType.Sunny;

            TimeOfDayCatalogConfig.TimeOfDayEntry timeEntry = null;
            TimeOfDay timeOfDay = WeightedSelector.TrySelect(timeOfDayCatalog.GetWeightedTimesOfDay(), _random, out timeEntry) && timeEntry != null
                ? timeEntry.TimeOfDay
                : TimeOfDay.Morning;

            RaceEnvironmentSeeds seeds = new RaceEnvironmentSeeds(_random.NextInt(1, int.MaxValue), _random.NextInt(1, int.MaxValue));
            MatchEnvironmentSelection selection = new MatchEnvironmentSelection(mapEntry.MapId, timeOfDay, weather, seeds);

            Current = selection;
            HasResolvedEnvironment = true;

            ApplyLighting(timeEntry, weatherEntry);

            EnvironmentResolved?.Invoke(selection);
        }

        private static void ApplyLighting(TimeOfDayCatalogConfig.TimeOfDayEntry timeEntry, WeatherCatalogConfig.WeatherEntry weatherEntry)
        {
            if (timeEntry != null)
            {
                RenderSettings.ambientLight = Scale(timeEntry.AmbientLightColor, timeEntry.AmbientIntensity);
            }

            if (weatherEntry != null)
            {
                RenderSettings.fog = weatherEntry.FogDensity01 > 0f;
                RenderSettings.fogColor = weatherEntry.TintColor;
                RenderSettings.fogDensity = weatherEntry.FogDensity01 * 0.05f;
            }
        }

        private static Color Scale(Color color, float intensity)
        {
            return new Color(color.r * intensity, color.g * intensity, color.b * intensity, color.a);
        }

        /// <summary>Sprint 13 (Main Menu "Current selected map"): looks up <paramref name="mapId"/>'s display name in <see cref="mapCatalog"/>, falling back to the raw id.</summary>
        public string ResolveMapDisplayName(MapId mapId)
        {
            if (mapCatalog != null && mapCatalog.TryGetEntry(mapId, out MapCatalogConfig.MapEntry entry))
            {
                return entry.DisplayName;
            }

            return mapId.Value;
        }
    }
}

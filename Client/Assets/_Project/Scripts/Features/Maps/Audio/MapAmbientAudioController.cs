using GulfRun.Core.Managers;
using GulfRun.Domain;
using GulfRun.Features.Maps.Configuration;
using UnityEngine;

namespace GulfRun.Features.Maps.Audio
{
    /// <summary>
    /// Sprint 12 "AUDIO: Ambient sounds per city ... Day and night
    /// variations." Reacts to <see cref="MapEnvironmentManager.EnvironmentResolved"/>
    /// and plays the active map's day/night ambient clip through
    /// <see cref="AudioManager"/>'s dedicated ambient channel — never the
    /// Music channel, so a Victory Ceremony track is never cut short by a
    /// city-ambience swap (see <see cref="AudioManager.PlayAmbient"/>).
    /// Scene-scoped (Gameplay.unity), resets cleanly per scene load.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MapAmbientAudioController : MonoBehaviour
    {
        [Tooltip("Optional override. Defaults to MapEnvironmentManager.Instance.MapCatalog when unset.")]
        [SerializeField] private MapCatalogConfig mapCatalog;

        [Range(0f, 1f)]
        [SerializeField] private float ambientVolume = 0.6f;

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

        private void HandleEnvironmentResolved(MatchEnvironmentSelection selection)
        {
            MapCatalogConfig catalog = ResolveCatalog();
            if (catalog == null || AudioManager.Instance == null)
            {
                return;
            }

            if (!catalog.TryGetEntry(selection.Map, out MapCatalogConfig.MapEntry entry))
            {
                return;
            }

            AudioClip clip = selection.TimeOfDay == TimeOfDay.Night ? entry.NightAmbientClip : entry.DayAmbientClip;
            AudioManager.Instance.PlayAmbient(clip, ambientVolume);
        }

        private MapCatalogConfig ResolveCatalog()
        {
            if (mapCatalog != null)
            {
                return mapCatalog;
            }

            return MapEnvironmentManager.Instance != null ? MapEnvironmentManager.Instance.MapCatalog : null;
        }
    }
}

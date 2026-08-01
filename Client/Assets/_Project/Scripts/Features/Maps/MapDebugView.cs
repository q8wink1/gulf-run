using GulfRun.Domain;
using GulfRun.Features.Maps.Configuration;
using UnityEngine;

namespace GulfRun.Features.Maps
{
    /// <summary>
    /// Debug overlay: Current Map, Current Weather, Current Time, Trap
    /// Seed, Item Box Seed — the Sprint 12 brief's exact "DEBUG: Display"
    /// list. Same OnGUI-placeholder approach as every other Sprint's
    /// *DebugView; next free panel slot after Progression's 3160.
    /// </summary>
    public sealed class MapDebugView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private bool showOnScreenDebug = true;
        [SerializeField] private int panelX = 3610;

        private void OnGUI()
        {
            if (!showOnScreenDebug)
            {
                return;
            }

            int y = 10;
            const int lineHeight = 18;
            const int width = 460;

            void Line(string text)
            {
                GUI.Label(new Rect(panelX, y, width, lineHeight), text);
                y += lineHeight;
            }

            Line("[Maps] Sprint 12");

            MapEnvironmentManager manager = MapEnvironmentManager.Instance;
            if (manager == null || !manager.HasResolvedEnvironment)
            {
                Line("Environment not resolved yet (waiting for Match Countdown).");
                return;
            }

            MatchEnvironmentSelection current = manager.Current;
            string mapName = current.Map.Value;
            if (manager.MapCatalog != null && manager.MapCatalog.TryGetEntry(current.Map, out MapCatalogConfig.MapEntry mapEntry))
            {
                mapName = mapEntry.DisplayName + " (" + mapEntry.Country + ")";
            }

            Line($"Current Map: {mapName}");
            Line($"Current Weather: {current.Weather}");
            Line($"Current Time: {current.TimeOfDay}");
            Line($"Trap Seed: {current.Seeds.TrapSeed}");
            Line($"Item Box Seed: {current.Seeds.ItemBoxSeed}");
        }
#endif
    }
}

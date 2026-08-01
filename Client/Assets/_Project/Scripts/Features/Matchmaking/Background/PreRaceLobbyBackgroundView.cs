using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Matchmaking.UI;
using UnityEngine;

namespace GulfRun.Features.Matchmaking.Background
{
    /// <summary>
    /// Sprint 14 Matchmaking "BACKGROUND": animated Gulf lobby sky + palm
    /// silhouettes, driven by the already-resolved <see cref="IMapContextProvider"/>
    /// launch city (random city from Main Menu session, not re-rolled here).
    /// </summary>
    public sealed class PreRaceLobbyBackgroundView : MonoBehaviour
    {
        private void OnGUI()
        {
            TimeOfDay timeOfDay = TimeOfDay.Morning;
            IMapContextProvider map = MapContextService.Current;
            if (map != null && map.HasResolvedEnvironment)
            {
                timeOfDay = map.Current.TimeOfDay;
            }

            DrawSky(timeOfDay);
            DrawGround();
            DrawPalms();
            DrawCityLabel(map);
        }

        private static void DrawSky(TimeOfDay timeOfDay)
        {
            Color top = timeOfDay == TimeOfDay.Night ? new Color(0.04f, 0.06f, 0.16f, 1f)
                : timeOfDay == TimeOfDay.Sunset ? new Color(0.55f, 0.28f, 0.42f, 1f)
                : new Color(0.55f, 0.75f, 0.95f, 1f);
            Color bottom = timeOfDay == TimeOfDay.Night ? new Color(0.16f, 0.14f, 0.28f, 1f)
                : timeOfDay == TimeOfDay.Sunset ? new Color(0.95f, 0.55f, 0.30f, 1f)
                : new Color(0.95f, 0.85f, 0.60f, 1f);

            float half = Screen.height * 0.5f;
            Color previous = GUI.color;
            GUI.color = top;
            GUI.Box(new Rect(0f, 0f, Screen.width, half), string.Empty);
            GUI.color = bottom;
            GUI.Box(new Rect(0f, half, Screen.width, Screen.height - half), string.Empty);
            GUI.color = previous;
        }

        private static void DrawGround()
        {
            float h = Screen.height * 0.18f;
            Color previous = GUI.color;
            GUI.color = PreRaceLobbyTheme.SandDark;
            GUI.Box(new Rect(0f, Screen.height - h, Screen.width, h), string.Empty);
            GUI.color = previous;
        }

        private static void DrawPalms()
        {
            double elapsed = Time.timeAsDouble;
            float groundY = Screen.height * 0.82f;
            Color previous = GUI.color;
            GUI.color = new Color(0.10f, 0.10f, 0.08f, 0.9f);
            for (int i = 0; i < 4; i++)
            {
                float x = Screen.width * (0.05f + i * 0.08f);
                float sway = CelebrationAnimation.EvaluateOffset(elapsed, 8f, 0.25f + i * 0.03f);
                GUI.Box(new Rect(x, groundY - 90f, 10f, 90f), string.Empty);
                GUI.Box(new Rect(x - 24f + sway, groundY - 110f, 58f, 20f), string.Empty);
            }

            GUI.color = previous;
        }

        private static void DrawCityLabel(IMapContextProvider map)
        {
            string city = "Gulf City";
            if (map != null && map.HasResolvedEnvironment)
            {
                city = map.ResolveMapDisplayName(map.Current.Map);
            }

            GUI.Label(new Rect(16f, 12f, 320f, 24f), "Launch City: " + city, PreRaceLobbyTheme.Muted);
        }
    }
}

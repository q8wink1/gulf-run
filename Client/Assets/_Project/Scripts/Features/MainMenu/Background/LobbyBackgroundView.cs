using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.MainMenu.UI;
using UnityEngine;

namespace GulfRun.Features.MainMenu.Background
{
    /// <summary>
    /// Sprint 13 "BACKGROUND": an animated sky (random launch map + random
    /// Morning/Sunset/Night, moving clouds, flying birds, swaying palm
    /// trees) drawn entirely with colored <see cref="GUI.Box"/> shapes —
    /// this project's established "no final art yet" placeholder posture
    /// (see every prior sprint's OnGUI screens). Deliberately reuses
    /// Sprint 12's real <see cref="IMapContextProvider"/> instead of
    /// rolling its own random map: the very first
    /// <see cref="IMapContextProvider.ResolveNewEnvironment"/> call this
    /// Main Menu session makes becomes both "what the lobby looks like"
    /// AND "BOTTOM: Current selected map" (<see cref="Bottom.PlayButtonView"/>)
    /// — one honest source of truth instead of two disconnected random
    /// pickers that could disagree.
    /// </summary>
    public sealed class LobbyBackgroundView : MonoBehaviour
    {
        private const int CloudCount = 4;
        private const int BirdCount = 3;
        private const int PalmTreeCount = 4;

        private bool _hasRolledForThisSession;

        private void Update()
        {
            // Only the very first Main Menu view alive rolls a fresh
            // environment for this session — every other widget (bottom
            // bar map name, etc.) just reads whatever IMapContextProvider
            // already resolved, so re-entering the menu mid-session (e.g.
            // after Leave Match) does not re-roll the background under
            // the player's feet.
            if (_hasRolledForThisSession)
            {
                return;
            }

            _hasRolledForThisSession = true;
            IMapContextProvider mapContext = MapContextService.Current;
            if (mapContext != null && !mapContext.HasResolvedEnvironment)
            {
                mapContext.ResolveNewEnvironment();
            }
        }

        private void OnGUI()
        {
            TimeOfDay timeOfDay = ResolveTimeOfDay();
            DrawSky(timeOfDay);
            DrawSunOrMoon(timeOfDay);
            DrawClouds();
            DrawBirds();
            DrawGroundAndPalmTrees(timeOfDay);
        }

        private static TimeOfDay ResolveTimeOfDay()
        {
            IMapContextProvider mapContext = MapContextService.Current;
            return mapContext != null && mapContext.HasResolvedEnvironment ? mapContext.Current.TimeOfDay : TimeOfDay.Morning;
        }

        private static void DrawSky(TimeOfDay timeOfDay)
        {
            float halfHeight = Screen.height * 0.5f;

            Color previous = GUI.color;
            GUI.color = MainMenuTheme.SkyTop(timeOfDay);
            GUI.Box(new Rect(0f, 0f, Screen.width, halfHeight), string.Empty);

            GUI.color = MainMenuTheme.SkyBottom(timeOfDay);
            GUI.Box(new Rect(0f, halfHeight, Screen.width, Screen.height - halfHeight), string.Empty);
            GUI.color = previous;
        }

        private static void DrawSunOrMoon(TimeOfDay timeOfDay)
        {
            const float size = 90f;
            float x = Screen.width * 0.78f;
            float y = Screen.height * 0.14f;

            Color previous = GUI.color;
            GUI.color = MainMenuTheme.SunOrMoon(timeOfDay);
            GUI.Box(new Rect(x, y, size, size), string.Empty);
            GUI.color = previous;
        }

        private void DrawClouds()
        {
            double elapsed = Time.timeAsDouble;
            Color previous = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.55f);

            for (int i = 0; i < CloudCount; i++)
            {
                float laneY = Screen.height * (0.08f + i * 0.06f);
                float speed = 14f + i * 6f;
                float cloudWidth = 140f + i * 20f;

                float travel = (float)((elapsed * speed) % (Screen.width + cloudWidth));
                float x = travel - cloudWidth;

                GUI.Box(new Rect(x, laneY, cloudWidth, 34f), string.Empty);
            }

            GUI.color = previous;
        }

        private void DrawBirds()
        {
            double elapsed = Time.timeAsDouble;
            Color previous = GUI.color;
            GUI.color = new Color(0.15f, 0.15f, 0.18f, 0.85f);

            for (int i = 0; i < BirdCount; i++)
            {
                float laneY = Screen.height * (0.18f + i * 0.05f);
                float speed = 60f + i * 15f;
                float travel = (float)((elapsed * speed + i * 220f) % (Screen.width + 40f));
                float x = travel - 20f;
                float bob = CelebrationAnimation.EvaluateOffset(elapsed + i, 6f, 0.6f);

                GUI.Box(new Rect(x, laneY + bob, 18f, 6f), string.Empty);
            }

            GUI.color = previous;
        }

        private void DrawGroundAndPalmTrees(TimeOfDay timeOfDay)
        {
            float groundHeight = Screen.height * 0.16f;
            float groundY = Screen.height - groundHeight;

            Color previous = GUI.color;
            GUI.color = timeOfDay == TimeOfDay.Night ? new Color(0.05f, 0.06f, 0.10f, 1f) : MainMenuTheme.SandDark;
            GUI.Box(new Rect(0f, groundY, Screen.width, groundHeight), string.Empty);

            double elapsed = Time.timeAsDouble;
            GUI.color = new Color(0.10f, 0.10f, 0.08f, 0.9f);
            for (int i = 0; i < PalmTreeCount; i++)
            {
                bool leftSide = i % 2 == 0;
                float x = leftSide ? Screen.width * (0.03f + (i / 2) * 0.05f) : Screen.width * (0.97f - (i / 2) * 0.05f);
                float trunkHeight = 90f;
                float trunkY = groundY - trunkHeight + 20f;

                GUI.Box(new Rect(x, trunkY, 10f, trunkHeight), string.Empty);

                // "Palm trees moving" — a gentle sway on the leaf canopy only, trunk stays planted.
                float sway = CelebrationAnimation.EvaluateOffset(elapsed, 8f, 0.25f + i * 0.03f);
                GUI.Box(new Rect(x - 24f + sway, trunkY - 22f, 58f, 20f), string.Empty);
            }

            GUI.color = previous;
        }
    }
}

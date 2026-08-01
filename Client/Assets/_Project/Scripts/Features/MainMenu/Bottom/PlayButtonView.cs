using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.MainMenu.UI;
using UnityEngine;

namespace GulfRun.Features.MainMenu.Bottom
{
    /// <summary>
    /// Sprint 13 "BOTTOM" + "PLAY BUTTON": the large golden PLAY button
    /// (animated glow, small click animation) plus the current selected
    /// map / game mode / estimated matchmaking time readout above it.
    /// Drives <see cref="IMatchLobbySummaryProvider"/> end-to-end (Create
    /// Match → Cancel/Leave → hand off to <see cref="SceneManager.LoadGameplay"/>)
    /// with zero compile-time reference to Features.Multiplayer.
    /// </summary>
    public sealed class PlayButtonView : MonoBehaviour
    {
        private const float ButtonWidth = 260f;
        private const float ButtonHeight = 84f;

        private ButtonPressAnimator _playAnim;
        private ButtonPressAnimator _secondaryAnim;

        private void OnGUI()
        {
            float centerX = Screen.width * 0.5f;
            float bottomY = Screen.height - 40f;

            DrawInfoStrip(centerX, bottomY - ButtonHeight - 54f);
            DrawPlayButton(centerX, bottomY - ButtonHeight);
        }

        private void DrawInfoStrip(float centerX, float y)
        {
            const float width = 420f;
            IMapContextProvider mapContext = MapContextService.Current;
            string mapName = "—";
            if (mapContext != null && mapContext.HasResolvedEnvironment)
            {
                mapName = mapContext.ResolveMapDisplayName(mapContext.Current.Map);
            }

            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            string etaText = ResolveEtaText(lobby);

            string line = "Map: " + mapName + "   •   Mode: " + GameMode.QuickRace + "   •   " + etaText;
            GUI.Label(new Rect(centerX - width * 0.5f, y, width, 24f), line, MainMenuTheme.MutedLabel);
        }

        private static string ResolveEtaText(IMatchLobbySummaryProvider lobby)
        {
            if (lobby == null)
            {
                return "Matchmaking unavailable";
            }

            if (!lobby.IsInMatch && !lobby.IsMatchmaking)
            {
                int eta = MatchmakingEtaEstimator.EstimateSecondsRemaining(0, lobby.RequiredPlayerCount);
                return "Est. wait: ~" + eta + "s";
            }

            if (lobby.IsMatchmaking)
            {
                int eta = MatchmakingEtaEstimator.EstimateSecondsRemaining(lobby.LobbyPlayerCount, lobby.RequiredPlayerCount);
                return "Finding match... ~" + eta + "s";
            }

            return "Lobby: " + lobby.LobbyPlayerCount + "/" + lobby.RequiredPlayerCount + " players";
        }

        private void DrawPlayButton(float centerX, float y)
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            bool inMatch = lobby != null && lobby.IsInMatch;
            bool matchmaking = lobby != null && lobby.IsMatchmaking;

            string label = inMatch ? "START RACE" : matchmaking ? "SEARCHING..." : "PLAY";
            DrawGlow(centerX, y);

            Rect rect = _playAnim.Apply(new Rect(centerX - ButtonWidth * 0.5f, y, ButtonWidth, ButtonHeight), 4f);
            Color previous = GUI.color;
            GUI.color = MainMenuTheme.Gold;
            if (GUI.Button(rect, label, MainMenuTheme.PlayButton))
            {
                _playAnim.NotifyPressed();
                HandlePlayClicked(lobby, inMatch);
            }

            GUI.color = previous;

            if (inMatch || matchmaking)
            {
                DrawSecondaryButton(centerX, y + ButtonHeight + 8f, lobby);
            }
        }

        private static void HandlePlayClicked(IMatchLobbySummaryProvider lobby, bool inMatch)
        {
            if (lobby == null)
            {
                return;
            }

            if (inMatch)
            {
                SceneManager.Instance?.LoadGameplay();
                return;
            }

            string displayName = LocalProfileProviderService.Current != null && LocalProfileProviderService.Current.HasProfile
                ? LocalProfileProviderService.Current.LocalProfile.Nickname
                : "Player";

            lobby.StartQuickMatch(displayName);
        }

        private void DrawSecondaryButton(float centerX, float y, IMatchLobbySummaryProvider lobby)
        {
            const float width = 160f;
            const float height = 30f;

            Rect rect = _secondaryAnim.Apply(new Rect(centerX - width * 0.5f, y, width, height), 2f);
            if (GUI.Button(rect, "Cancel", MainMenuTheme.PanelButton))
            {
                _secondaryAnim.NotifyPressed();
                lobby.CancelOrLeaveMatch();
            }
        }

        private static void DrawGlow(float centerX, float y)
        {
            double elapsed = Time.timeAsDouble;
            // "Animated glow" — a soft pulsing gold halo behind the button, driven by the same sine helper every other breathing/sway effect in this sprint uses.
            float pulse01 = (CelebrationAnimation.EvaluateOffset(elapsed, 1f, 0.5f) + 1f) * 0.5f;
            float glowSize = 12f + pulse01 * 10f;

            Color previous = GUI.color;
            GUI.color = new Color(MainMenuTheme.GoldBright.r, MainMenuTheme.GoldBright.g, MainMenuTheme.GoldBright.b, 0.25f + pulse01 * 0.15f);
            GUI.Box(new Rect(centerX - ButtonWidth * 0.5f - glowSize, y - glowSize, ButtonWidth + glowSize * 2f, ButtonHeight + glowSize * 2f), string.Empty);
            GUI.color = previous;
        }
    }
}

using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Matchmaking.UI;
using UnityEngine;

namespace GulfRun.Features.Matchmaking.Lobby
{
    /// <summary>
    /// Sprint 14 Matchmaking "MATCH FOUND": premium popup shown briefly when
    /// entering the Pre-Race Lobby after Quick Play / Private Room create.
    /// </summary>
    public sealed class MatchFoundPopupView : MonoBehaviour
    {
        [SerializeField, Range(0.5f, 4f)] private float visibleSeconds = 1.8f;

        private double _shownAt = -1d;
        private bool _armed;

        private void OnEnable()
        {
            _shownAt = Time.timeAsDouble;
            _armed = true;
        }

        private void OnGUI()
        {
            if (!_armed || Time.timeAsDouble - _shownAt > visibleSeconds)
            {
                return;
            }

            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            float t = (float)(Time.timeAsDouble - _shownAt);
            float alpha = t < 0.2f ? t / 0.2f : t > visibleSeconds - 0.35f ? (visibleSeconds - t) / 0.35f : 1f;
            float pulse = (CelebrationAnimation.EvaluateOffset(Time.timeAsDouble, 1f, 1.2f) + 1f) * 0.5f;

            float width = Mathf.Min(420f, Screen.width - 40f);
            float height = 120f;
            float x = (Screen.width - width) * 0.5f;
            float y = Screen.height * 0.18f - pulse * 4f;

            Color previous = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, alpha);
            PreRaceLobbyTheme.DrawPanel(new Rect(x, y, width, height));
            GUI.Label(new Rect(x + 16f, y + 18f, width - 32f, 36f), "MATCH FOUND", PreRaceLobbyTheme.Title);
            string detail = lobby != null
                ? (lobby.IsPrivateRoom ? "Private Room ready" : "Quick Play lobby filling") + " — " + lobby.LobbyPlayerCount + "/" + lobby.RequiredPlayerCount
                : "Lobby ready";
            GUI.Label(new Rect(x + 16f, y + 60f, width - 32f, 28f), detail, PreRaceLobbyTheme.Header);
            GUI.color = previous;
        }
    }
}

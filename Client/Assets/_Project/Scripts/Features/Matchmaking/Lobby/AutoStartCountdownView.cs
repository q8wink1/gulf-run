using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Matchmaking.UI;
using UnityEngine;

namespace GulfRun.Features.Matchmaking.Lobby
{
    /// <summary>
    /// Sprint 14 Matchmaking Auto Start: 5-4-3-2-1-GO overlay, countdown SFX,
    /// then smooth fade into Gameplay when <see cref="MatchState.Running"/>.
    /// </summary>
    public sealed class AutoStartCountdownView : MonoBehaviour
    {
        [SerializeField] private AudioClip countdownTickSound;
        [SerializeField] private AudioClip goSound;
        [SerializeField, Range(0.2f, 2f)] private float fadeSeconds = 0.55f;

        private int _lastHeardSeconds = -1;
        private bool _transitionStarted;
        private double _fadeStartedAt = -1d;
        private MatchState _lastPhase = MatchState.Waiting;

        private void Update()
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby == null)
            {
                return;
            }

            MatchState phase = lobby.LobbyPhase;
            if (phase == MatchState.Countdown)
            {
                int seconds = lobby.AutoStartCountdownSecondsRemaining;
                if (seconds != _lastHeardSeconds && seconds > 0)
                {
                    _lastHeardSeconds = seconds;
                    AudioManager.Instance?.PlayOneShot(countdownTickSound);
                }
            }
            else if (_lastPhase == MatchState.Countdown && phase == MatchState.Running && !_transitionStarted)
            {
                AudioManager.Instance?.PlayOneShot(goSound);
                _transitionStarted = true;
                _fadeStartedAt = Time.timeAsDouble;
            }

            if (_transitionStarted && Time.timeAsDouble - _fadeStartedAt >= fadeSeconds)
            {
                SceneManager.Instance?.LoadGameplay();
            }

            _lastPhase = phase;
        }

        private void OnGUI()
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby == null)
            {
                return;
            }

            if (lobby.LobbyPhase == MatchState.Countdown)
            {
                int seconds = lobby.AutoStartCountdownSecondsRemaining;
                string text = seconds > 0 ? seconds.ToString() : "GO";
                float punch = 1f + Mathf.Abs(CelebrationAnimation.EvaluateOffset(Time.timeAsDouble, 0.08f, 6f));
                GUIStyle style = PreRaceLobbyTheme.Countdown;
                style.fontSize = Mathf.CeilToInt(72f * punch);
                GUI.Label(new Rect(0f, Screen.height * 0.12f, Screen.width, 120f), text, style);
            }

            if (_transitionStarted)
            {
                float t = Mathf.Clamp01((float)((Time.timeAsDouble - _fadeStartedAt) / fadeSeconds));
                Color previous = GUI.color;
                GUI.color = new Color(0f, 0f, 0f, t);
                GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), string.Empty);
                GUI.color = previous;
            }
        }
    }
}

using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.RaceHud.Configuration;
using UnityEngine;

namespace GulfRun.Features.RaceHud.UI
{
    /// <summary>
    /// Professional 3-2-1-GO presentation: punch scale per beat, GO slides
    /// away after the race starts, optional tick/GO SFX via AudioManager.
    /// </summary>
    public sealed class CountdownHudView : MonoBehaviour
    {
        [SerializeField] private RaceHudConfig config;

        private int _lastAnnouncedSecond = int.MinValue;
        private bool _goAnnounced;
        private float _goElapsed = -1f;
        private float _beatElapsed;
        private string _display = string.Empty;
        private bool _visible;

        private void OnEnable()
        {
            ICountdownHudProvider countdown = CountdownHudService.Current;
            if (countdown != null)
            {
                countdown.SecondsChanged += HandleSecondsChanged;
                countdown.Finished += HandleFinished;
            }

            IGameStateProvider state = GameStateService.Current;
            if (state != null)
            {
                state.StateChanged += HandleStateChanged;
            }
        }

        private void OnDisable()
        {
            ICountdownHudProvider countdown = CountdownHudService.Current;
            if (countdown != null)
            {
                countdown.SecondsChanged -= HandleSecondsChanged;
                countdown.Finished -= HandleFinished;
            }

            IGameStateProvider state = GameStateService.Current;
            if (state != null)
            {
                state.StateChanged -= HandleStateChanged;
            }
        }

        private void Update()
        {
            if (!_visible)
            {
                return;
            }

            _beatElapsed += Time.unscaledDeltaTime;
            if (_goElapsed >= 0f)
            {
                _goElapsed += Time.unscaledDeltaTime;
                float hold = config != null ? config.GoHoldSeconds : 0.75f;
                if (_goElapsed >= hold)
                {
                    _visible = false;
                }
            }
        }

        private void OnGUI()
        {
            if (!_visible || string.IsNullOrEmpty(_display))
            {
                return;
            }

            float scale = HudLayoutScale.Resolve(Screen.width, Screen.height);
            float punch = _goElapsed >= 0f ? 1f : CountdownHudAnimation.EvaluatePunchScale(_beatElapsed);
            float alpha = 1f;
            float yOffset = 0f;
            if (_goElapsed >= 0f)
            {
                float hold = config != null ? config.GoHoldSeconds : 0.75f;
                CountdownHudAnimation.EvaluateGoExit(_goElapsed, hold, out float offset01, out alpha);
                yOffset = offset01 * Screen.height;
            }

            Color previous = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, alpha);
            GUIStyle style = RaceHudTheme.Huge(scale * punch);
            Rect area = new Rect(0f, Screen.height * 0.42f + yOffset, Screen.width, 120f * scale * punch);
            GUI.Label(area, _display, style);
            GUI.color = previous;
        }

        private void HandleSecondsChanged(int seconds)
        {
            _display = seconds.ToString();
            _visible = true;
            _beatElapsed = 0f;
            _goElapsed = -1f;
            if (seconds != _lastAnnouncedSecond && seconds > 0)
            {
                _lastAnnouncedSecond = seconds;
                Play(config != null ? config.CountdownTickClip : null);
            }
        }

        private void HandleFinished()
        {
            _display = "GO!";
            _visible = true;
            _goElapsed = 0f;
            _beatElapsed = 0f;
            if (!_goAnnounced)
            {
                _goAnnounced = true;
                Play(config != null ? config.CountdownGoClip : null);
            }
        }

        private void HandleStateChanged(GameLoopState state)
        {
            if (state == GameLoopState.Countdown || state == GameLoopState.Ready)
            {
                _goAnnounced = false;
                _lastAnnouncedSecond = int.MinValue;
                ICountdownHudProvider countdown = CountdownHudService.Current;
                if (countdown != null && countdown.IsActive)
                {
                    _display = countdown.DisplayText;
                    _visible = true;
                    _goElapsed = -1f;
                }
            }
        }

        private static void Play(AudioClip clip)
        {
            if (clip != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayOneShot(clip);
            }
        }
    }
}

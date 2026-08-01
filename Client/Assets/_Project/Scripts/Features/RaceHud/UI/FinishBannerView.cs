using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.RaceHud.Configuration;
using UnityEngine;

namespace GulfRun.Features.RaceHud.UI
{
    /// <summary>
    /// Large FINISH banner with fireworks + confetti + crowd/fanfare cues,
    /// shown the moment the local player crosses the finish line.
    /// </summary>
    public sealed class FinishBannerView : MonoBehaviour
    {
        [SerializeField] private RaceHudConfig config;

        private bool _active;
        private float _elapsed;
        private string _placeLabel = "FINISHED!";
        private bool _audioPlayed;
        private bool _wasFinished;

        private void Update()
        {
            IRaceStandingsHudProvider standings = RaceStandingsHudService.Current;
            bool finished = standings != null && standings.LocalHasFinished;
            if (finished && !_wasFinished)
            {
                Begin(standings);
            }

            _wasFinished = finished;

            if (!_active)
            {
                return;
            }

            _elapsed += Time.unscaledDeltaTime;
            float duration = config != null ? config.FinishBannerSeconds : 3.5f;
            if (_elapsed >= duration)
            {
                _active = false;
            }
        }

        private void OnGUI()
        {
            if (!_active)
            {
                return;
            }

            float scale = HudLayoutScale.Resolve(Screen.width, Screen.height);
            DrawFireworks(scale);
            DrawConfetti();

            Rect banner = new Rect(Screen.width * 0.15f, Screen.height * 0.28f, Screen.width * 0.7f, 90f * scale);
            RaceHudTheme.DrawPanel(banner);
            GUI.Label(banner, "FINISH!", RaceHudTheme.Huge(scale * 0.55f));
            GUI.Label(
                new Rect(banner.x, banner.yMax + 8f, banner.width, 36f * scale),
                _placeLabel,
                RaceHudTheme.Title(scale));
        }

        private void Begin(IRaceStandingsHudProvider standings)
        {
            _active = true;
            _elapsed = 0f;
            int place = standings.LocalFinalPlace ?? standings.LocalPlace;
            _placeLabel = RacePositionFormatter.FormatOrdinal(place).ToUpperInvariant();
            if (!_audioPlayed)
            {
                _audioPlayed = true;
                if (AudioManager.Instance != null)
                {
                    if (config != null && config.FinishFanfareClip != null)
                    {
                        AudioManager.Instance.PlayOneShot(config.FinishFanfareClip);
                    }

                    if (config != null && config.FinishCrowdClip != null)
                    {
                        AudioManager.Instance.PlayOneShot(config.FinishCrowdClip);
                    }
                }
            }
        }

        private void DrawFireworks(float scale)
        {
            int count = config != null ? config.FireworkParticleCount : 36;
            float speed = config != null ? config.FireworkBurstSpeed : 0.9f;
            Color previous = GUI.color;
            for (int i = 0; i < count; i++)
            {
                FireworkParticle p = FireworkSimulation.Evaluate(i, _elapsed, speed);
                GUI.color = new Color(RaceHudTheme.GoldBright.r, RaceHudTheme.Sand.g, 0.2f, p.Alpha01);
                float size = 5f * scale;
                GUI.Box(new Rect(p.NormalizedX * Screen.width, p.NormalizedY * Screen.height, size, size), string.Empty);
            }

            GUI.color = previous;
        }

        private void DrawConfetti()
        {
            int count = config != null ? config.FinishConfettiCount : 48;
            float fall = config != null ? config.FinishConfettiFallSpeed : 0.35f;
            Color previous = GUI.color;
            GUI.color = new Color(1f, 0.85f, 0.1f, 0.85f);
            for (int i = 0; i < count; i++)
            {
                ConfettiParticle particle = ConfettiSimulation.Evaluate(i, _elapsed, fall);
                GUI.Box(new Rect(particle.NormalizedX * Screen.width, particle.NormalizedY * Screen.height * 0.7f, 4f, 8f), string.Empty);
            }

            GUI.color = previous;
        }
    }
}

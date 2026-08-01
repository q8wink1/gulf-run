using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.MainMenu.Background
{
    /// <summary>
    /// Sprint 13 "ANIMATIONS: Floating particles" + "BACKGROUND: small
    /// ambient animation": a handful of drifting gold dust motes rising
    /// slowly up the screen — cheap (a fixed small count, no allocation
    /// after <see cref="Awake"/>) and purely decorative, drawn above the
    /// sky/ground but below every interactive panel.
    /// </summary>
    public sealed class FloatingParticlesView : MonoBehaviour
    {
        private const int ParticleCount = 14;

        private struct Particle
        {
            public float StartXFraction;
            public float Speed;
            public float SwayAmplitude;
            public float SwayFrequency;
            public float Size;
            public double SpawnOffsetSeconds;
        }

        private Particle[] _particles;

        private void Awake()
        {
            var random = SeededRandom.FromTime();
            _particles = new Particle[ParticleCount];
            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i] = new Particle
                {
                    StartXFraction = random.NextFloat01(),
                    Speed = 12f + random.NextFloat01() * 18f,
                    SwayAmplitude = 6f + random.NextFloat01() * 10f,
                    SwayFrequency = 0.1f + random.NextFloat01() * 0.25f,
                    Size = 3f + random.NextFloat01() * 4f,
                    SpawnOffsetSeconds = random.NextFloat01() * 20d
                };
            }
        }

        private void OnGUI()
        {
            if (_particles == null)
            {
                return;
            }

            double elapsed = Time.timeAsDouble;
            Color previous = GUI.color;

            for (int i = 0; i < _particles.Length; i++)
            {
                Particle particle = _particles[i];
                double t = elapsed + particle.SpawnOffsetSeconds;

                float cycleHeight = Screen.height + 60f;
                float risenPixels = (float)((t * particle.Speed) % cycleHeight);
                float y = Screen.height - risenPixels;

                float sway = CelebrationAnimation.EvaluateOffset(t, particle.SwayAmplitude, particle.SwayFrequency);
                float x = particle.StartXFraction * Screen.width + sway;

                float fadeNearTop = Mathf.Clamp01(y / 80f);
                GUI.color = new Color(1f, 0.85f, 0.45f, 0.5f * fadeNearTop);
                GUI.Box(new Rect(x, y, particle.Size, particle.Size), string.Empty);
            }

            GUI.color = previous;
        }
    }
}

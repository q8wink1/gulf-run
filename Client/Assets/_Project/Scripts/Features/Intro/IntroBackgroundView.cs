using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Intro
{
    /// <summary>
    /// Sprint 14 "INTRO ANIMATION": the desert-night backdrop — "Moving
    /// desert sand dunes", "Soft wind particles" and "A Palm Tree silhouette
    /// slowly appears" — drawn with the same flat <see cref="GUI.Box"/>
    /// placeholder shapes as every other visual in this project (see
    /// Sprint 13's <c>LobbyBackgroundView</c>/<c>FloatingParticlesView</c>,
    /// which this deliberately mirrors in style).
    /// </summary>
    public sealed class IntroBackgroundView : MonoBehaviour
    {
        private const int DustParticleCount = 18;
        private const int DuneLayerCount = 3;

        private struct DustParticle
        {
            public float StartXFraction;
            public float StartYFraction;
            public float DriftSpeed;
            public float SwayAmplitude;
            public float SwayFrequency;
            public float Size;
        }

        private DustParticle[] _dustParticles;

        private void Awake()
        {
            var random = SeededRandom.FromTime();
            _dustParticles = new DustParticle[DustParticleCount];
            for (int i = 0; i < _dustParticles.Length; i++)
            {
                _dustParticles[i] = new DustParticle
                {
                    StartXFraction = random.NextFloat01(),
                    StartYFraction = 0.35f + random.NextFloat01() * 0.55f,
                    DriftSpeed = 20f + random.NextFloat01() * 30f,
                    SwayAmplitude = 3f + random.NextFloat01() * 5f,
                    SwayFrequency = 0.3f + random.NextFloat01() * 0.4f,
                    Size = 2f + random.NextFloat01() * 3f
                };
            }
        }

        private void OnGUI()
        {
            IntroSequenceController sequence = IntroSequenceController.Instance;
            double elapsed = sequence != null ? sequence.ElapsedSeconds : 0d;

            float skyAlpha = Mathf.Clamp01((float)(elapsed / IntroTimeline.DunesFadeInEnd));
            DrawSky(skyAlpha);
            DrawDunes(elapsed, skyAlpha);
            DrawWindDust(elapsed);
            DrawPalmTreeSilhouette(elapsed);
        }

        private static void DrawSky(float alpha01)
        {
            Color previous = GUI.color;
            // A deep desert-night gradient (two bands) — the Intro is a
            // fixed "first launch" moment, not tied to the Main Menu's
            // random Morning/Sunset/Night roll.
            GUI.color = new Color(0.05f, 0.06f, 0.14f, alpha01);
            GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height * 0.6f), string.Empty);

            GUI.color = new Color(0.12f, 0.09f, 0.16f, alpha01);
            GUI.Box(new Rect(0f, Screen.height * 0.6f, Screen.width, Screen.height * 0.4f), string.Empty);
            GUI.color = previous;
        }

        private static void DrawDunes(double elapsed, float alpha01)
        {
            Color previous = GUI.color;
            GUI.color = new Color(0.55f, 0.42f, 0.24f, alpha01);

            for (int layer = 0; layer < DuneLayerCount; layer++)
            {
                float depth = layer / (float)(DuneLayerCount - 1);
                float laneY = Screen.height * (0.62f + depth * 0.16f);
                float laneHeight = Screen.height * (0.14f + depth * 0.10f);

                // "Moving desert sand dunes" — a slow parallax drift, farther layers move slower.
                float speed = 4f + layer * 3f;
                float shift = (float)((elapsed * speed) % 120d);
                GUI.Box(new Rect(-40f + shift, laneY, Screen.width + 80f, laneHeight), string.Empty);
            }

            GUI.color = previous;
        }

        private void DrawWindDust(double elapsed)
        {
            if (_dustParticles == null)
            {
                return;
            }

            Color previous = GUI.color;
            GUI.color = new Color(0.92f, 0.85f, 0.68f, 0.5f);

            for (int i = 0; i < _dustParticles.Length; i++)
            {
                DustParticle particle = _dustParticles[i];
                float travel = (float)((elapsed * particle.DriftSpeed) % (Screen.width + 60f));
                float x = travel - 30f;
                float sway = CelebrationAnimation.EvaluateOffset(elapsed + i, particle.SwayAmplitude, particle.SwayFrequency);
                float y = Screen.height * particle.StartYFraction + sway;

                GUI.Box(new Rect(x, y, particle.Size, particle.Size), string.Empty);
            }

            GUI.color = previous;
        }

        /// <summary>"A Palm Tree silhouette slowly appears."</summary>
        private static void DrawPalmTreeSilhouette(double elapsed)
        {
            float progress = InverseLerpClamped((float)elapsed, IntroTimeline.PalmTreeFadeInStart, IntroTimeline.PalmTreeFadeInEnd);
            if (progress <= 0f)
            {
                return;
            }

            Color previous = GUI.color;
            GUI.color = new Color(0.04f, 0.04f, 0.05f, progress);

            float x = Screen.width * 0.14f;
            float groundY = Screen.height * 0.78f;
            float trunkHeight = Screen.height * 0.18f;

            GUI.Box(new Rect(x, groundY - trunkHeight, Screen.width * 0.012f, trunkHeight), string.Empty);
            GUI.Box(new Rect(x - Screen.width * 0.045f, groundY - trunkHeight - Screen.height * 0.02f, Screen.width * 0.10f, Screen.height * 0.02f), string.Empty);
            GUI.Box(new Rect(x - Screen.width * 0.03f, groundY - trunkHeight - Screen.height * 0.045f, Screen.width * 0.07f, Screen.height * 0.02f), string.Empty);
            GUI.Box(new Rect(x - Screen.width * 0.015f, groundY - trunkHeight - Screen.height * 0.065f, Screen.width * 0.045f, Screen.height * 0.02f), string.Empty);

            GUI.color = previous;
        }

        private static float InverseLerpClamped(float value, float from, float to) =>
            to <= from ? (value >= to ? 1f : 0f) : Mathf.Clamp01((value - from) / (to - from));
    }
}

using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Intro
{
    /// <summary>
    /// Sprint 14 "INTRO ANIMATION": "A Falcon appears flying across the
    /// screen" then "The Falcon circles above the dunes" — two phases
    /// driven purely off <see cref="IntroSequenceController.ElapsedSeconds"/>
    /// against <see cref="IntroTimeline"/>, drawn with the same
    /// flat-silhouette stepped-chevron shape as <see cref="Core.Branding.GulfRunBrandMark"/>'s
    /// falcon glyph (Design Rule consistency) but animated with a wing
    /// flap.
    /// </summary>
    public sealed class IntroFalconView : MonoBehaviour
    {
        private void OnGUI()
        {
            IntroSequenceController sequence = IntroSequenceController.Instance;
            if (sequence == null)
            {
                return;
            }

            double t = sequence.ElapsedSeconds;
            if (t < IntroTimeline.FalconFlyAcrossStart || t > IntroTimeline.FalconCircleEnd)
            {
                return;
            }

            Vector2 position = t <= IntroTimeline.FalconFlyAcrossEnd
                ? EvaluateFlyAcross((float)t)
                : EvaluateCircling((float)(t - IntroTimeline.FalconCircleStart));

            float flap = CelebrationAnimation.EvaluateOffset(t, 1f, 4f);
            DrawFalcon(position, flap);
        }

        /// <summary>Straight left-to-right flight path over the dune skyline.</summary>
        private static Vector2 EvaluateFlyAcross(float elapsed)
        {
            float progress = Mathf.Clamp01((elapsed - IntroTimeline.FalconFlyAcrossStart) / (IntroTimeline.FalconFlyAcrossEnd - IntroTimeline.FalconFlyAcrossStart));
            float x = Mathf.Lerp(-60f, Screen.width * 0.7f, progress);
            float y = Screen.height * Mathf.Lerp(0.34f, 0.22f, progress);
            return new Vector2(x, y);
        }

        /// <summary>A gentle circular loop above the dunes, centered where the fly-across ends.</summary>
        private static Vector2 EvaluateCircling(float elapsedSinceCircleStart)
        {
            const float radiusX = 90f;
            const float radiusY = 30f;
            const float angularSpeed = 1.4f;

            float centerX = Screen.width * 0.7f;
            float centerY = Screen.height * 0.20f;
            float angle = elapsedSinceCircleStart * angularSpeed * 2f * Mathf.PI;
            float x = centerX + radiusX * Mathf.Sin(angle);
            float y = centerY - radiusY * Mathf.Sin(angle * 0.5f) * Mathf.Sin(angle * 0.5f);
            return new Vector2(x, y);
        }

        private static void DrawFalcon(Vector2 position, float flap)
        {
            Color previous = GUI.color;
            GUI.color = new Color(0.06f, 0.06f, 0.08f, 0.95f);

            const float wingspan = 34f;
            GUI.Box(new Rect(position.x - 3f, position.y - 5f, 6f, 10f), string.Empty);
            GUI.Box(new Rect(position.x - wingspan * 0.5f, position.y - flap, wingspan * 0.4f, 4f), string.Empty);
            GUI.Box(new Rect(position.x + wingspan * 0.1f, position.y - flap, wingspan * 0.4f, 4f), string.Empty);

            GUI.color = previous;
        }
    }
}

using GulfRun.Core.Branding;
using UnityEngine;

namespace GulfRun.Features.Intro
{
    /// <summary>
    /// Sprint 14 "INTRO ANIMATION": "The GulfRun logo fades in with a
    /// premium golden shine." Purely a thin timing wrapper around
    /// <see cref="GulfRunBrandMark.Draw"/> — the actual mark is drawn by
    /// the one shared routine every branded screen uses (see that class's
    /// remarks), so the Intro's "official" logo is guaranteed pixel-for-
    /// pixel identical to the Loading screen/Main Menu/Store/Battle Pass
    /// watermarks.
    /// </summary>
    public sealed class IntroLogoView : MonoBehaviour
    {
        private GUIStyle _wordmarkStyle;

        private void OnGUI()
        {
            IntroSequenceController sequence = IntroSequenceController.Instance;
            if (sequence == null)
            {
                return;
            }

            double t = sequence.ElapsedSeconds;
            float alpha = InverseLerpClamped((float)t, IntroTimeline.LogoFadeInStart, IntroTimeline.LogoFadeInEnd);
            if (alpha <= 0f)
            {
                return;
            }

            float shine = InverseLerpClamped((float)t, IntroTimeline.ShineSweepStart, IntroTimeline.ShineSweepEnd);
            bool shineActive = t >= IntroTimeline.ShineSweepStart && t <= IntroTimeline.ShineSweepEnd;

            float size = Mathf.Min(Screen.width, Screen.height) * 0.32f;
            Rect rect = new Rect((Screen.width - size) * 0.5f, Screen.height * 0.30f, size, size);
            GulfRunBrandMark.Draw(rect, alpha, shineActive ? shine : -1f);

            DrawWordmark(rect, alpha);
        }

        private void DrawWordmark(Rect logoRect, float alpha01)
        {
            EnsureStyles();
            Color previous = GUI.color;
            _wordmarkStyle.normal.textColor = new Color(1f, 0.87f, 0.48f, alpha01);
            GUI.Label(new Rect(logoRect.x - 40f, logoRect.y + logoRect.height + 12f, logoRect.width + 80f, 40f), GulfRunBrandMark.Wordmark, _wordmarkStyle);
            GUI.color = previous;
        }

        private void EnsureStyles()
        {
            if (_wordmarkStyle != null)
            {
                return;
            }

            _wordmarkStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 30,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
        }

        private static float InverseLerpClamped(float value, float from, float to) =>
            to <= from ? (value >= to ? 1f : 0f) : Mathf.Clamp01((value - from) / (to - from));
    }
}

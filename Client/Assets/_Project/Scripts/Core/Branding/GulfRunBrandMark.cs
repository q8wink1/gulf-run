using UnityEngine;

namespace GulfRun.Core.Branding
{
    /// <summary>
    /// Sprint 14 "GULFRUN BRAND INTRO — LOGO DESIGN": the single, official
    /// drawing routine for the GulfRun mark — Desert Sand Dunes + Falcon +
    /// Palm Tree + Forward Motion, composed into one simple, iconic
    /// silhouette. This lives in <c>Core</c> (not a Feature) specifically
    /// so every screen that must show the brand — Brand Intro, Loading
    /// screen, Main Menu watermark, Store header, Battle Pass header, and
    /// any future screen — draws it through this ONE method instead of
    /// copy-pasting shapes ("DESIGN RULE: every future design must follow
    /// the GulfRun Brand Identity"). Same flat-<see cref="GUI.Box"/>-shape
    /// placeholder posture as every other visual in this project (no final
    /// art/sprite atlas yet — see Sprint 13/14 report Remaining TODOs);
    /// deliberately kept simple/flat/no-rotation so it stays "readable even
    /// as an App Icon" at small sizes (see <see cref="Draw"/>'s scale-
    /// invariant layout, driven entirely off <paramref name="rect"/>).
    /// </summary>
    public static class GulfRunBrandMark
    {
        private static readonly Color Gold = new Color(0.90f, 0.71f, 0.25f, 1f);
        private static readonly Color GoldBright = new Color(1f, 0.87f, 0.48f, 1f);
        private static readonly Color EmblemDark = new Color(0.09f, 0.07f, 0.05f, 1f);

        public const string Wordmark = "GULFRUN";

        /// <summary>
        /// Draws the full composed mark inside <paramref name="rect"/> —
        /// medallion frame, dunes, palm tree, falcon and a forward-motion
        /// accent — entirely scaled off <paramref name="rect"/> so the same
        /// call works from a small 40px lobby watermark up to a full-screen
        /// Brand Intro reveal.
        /// </summary>
        /// <param name="alpha01">Overall opacity (Brand Intro "fades in").</param>
        /// <param name="shineProgress01">-1 disables the shine sweep; 0..1 sweeps a bright highlight left-to-right across the mark ("premium golden shine").</param>
        public static void Draw(Rect rect, float alpha01 = 1f, float shineProgress01 = -1f)
        {
            if (alpha01 <= 0f)
            {
                return;
            }

            Color previous = GUI.color;
            float w = rect.width;
            float h = rect.height;
            float cx = rect.x + w * 0.5f;

            DrawMedallion(rect, alpha01);
            DrawDunes(rect, alpha01);
            DrawPalmTree(cx - w * 0.26f, rect.y + h * 0.40f, w * 0.20f, h * 0.30f, alpha01);
            DrawFalcon(cx + w * 0.06f, rect.y + h * 0.30f, w * 0.34f, alpha01);
            DrawForwardMotion(rect, alpha01);
            DrawShine(rect, alpha01, shineProgress01);

            GUI.color = previous;
        }

        private static void DrawMedallion(Rect rect, float alpha01)
        {
            GUI.color = WithAlpha(EmblemDark, alpha01);
            GUI.Box(rect, string.Empty);

            float ring1 = rect.width * 0.045f;
            GUI.color = WithAlpha(Gold, alpha01);
            GUI.Box(Inset(rect, ring1), string.Empty);

            float ring2 = ring1 + rect.width * 0.02f;
            GUI.color = WithAlpha(EmblemDark, alpha01);
            GUI.Box(Inset(rect, ring2), string.Empty);
        }

        /// <summary>Three overlapping stepped-mound silhouettes — "Desert Sand Dunes".</summary>
        private static void DrawDunes(Rect rect, float alpha01)
        {
            GUI.color = WithAlpha(Gold, alpha01);
            DrawDuneMound(rect, 0.06f, 0.60f, 0.98f, 0.30f);
            DrawDuneMound(rect, 0.40f, 0.66f, 0.66f, 0.26f);
            DrawDuneMound(rect, 0.20f, 0.74f, 0.86f, 0.20f);
        }

        private static void DrawDuneMound(Rect rect, float xFraction01, float baseYFraction01, float widthFraction01, float heightFraction01)
        {
            const int steps = 4;
            float baseX = rect.x + rect.width * xFraction01;
            float baseY = rect.y + rect.height * baseYFraction01;
            float fullWidth = rect.width * widthFraction01;
            float fullHeight = rect.height * heightFraction01;

            for (int i = 0; i < steps; i++)
            {
                float t = (i + 1f) / steps;
                float stepWidth = fullWidth * t;
                float stepHeight = fullHeight / steps;
                float stepY = baseY - fullHeight + i * stepHeight;
                float stepX = baseX + (fullWidth - stepWidth) * 0.5f;
                GUI.Box(new Rect(stepX, stepY, stepWidth, stepHeight + 1f), string.Empty);
            }
        }

        /// <summary>A trunk plus a small fan of leaf strokes — "Palm Tree".</summary>
        private static void DrawPalmTree(float x, float topY, float width, float height, float alpha01)
        {
            GUI.color = WithAlpha(EmblemDark, alpha01);
            float trunkWidth = width * 0.16f;
            GUI.Box(new Rect(x - trunkWidth * 0.5f, topY + height * 0.35f, trunkWidth, height * 0.65f), string.Empty);

            float leafSpan = width;
            float leafHeight = height * 0.16f;
            GUI.Box(new Rect(x - leafSpan * 0.5f, topY + height * 0.30f, leafSpan, leafHeight), string.Empty);
            GUI.Box(new Rect(x - leafSpan * 0.30f, topY + height * 0.18f, leafSpan * 0.65f, leafHeight), string.Empty);
            GUI.Box(new Rect(x - leafSpan * 0.10f, topY + height * 0.08f, leafSpan * 0.45f, leafHeight), string.Empty);
        }

        /// <summary>A simple stepped chevron pair — a minimal, iconic "Falcon" silhouette (same flat-rect language as every other bird in this project).</summary>
        private static void DrawFalcon(float centerX, float centerY, float wingspan, float alpha01)
        {
            GUI.color = WithAlpha(EmblemDark, alpha01);
            float bodyWidth = wingspan * 0.10f;
            float bodyHeight = wingspan * 0.22f;
            GUI.Box(new Rect(centerX - bodyWidth * 0.5f, centerY - bodyHeight * 0.5f, bodyWidth, bodyHeight), string.Empty);

            const int wingSteps = 3;
            float stepWidth = wingspan * 0.5f / wingSteps;
            float stepHeight = wingspan * 0.06f;
            for (int i = 0; i < wingSteps; i++)
            {
                float stepUp = i * stepHeight * 1.4f;
                float leftX = centerX - bodyWidth * 0.5f - (i + 1) * stepWidth;
                float rightX = centerX + bodyWidth * 0.5f + i * stepWidth;
                float y = centerY - stepUp;
                GUI.Box(new Rect(leftX, y, stepWidth + 1f, stepHeight), string.Empty);
                GUI.Box(new Rect(rightX, y, stepWidth + 1f, stepHeight), string.Empty);
            }
        }

        /// <summary>A small gold chevron beneath the dunes — "Forward motion".</summary>
        private static void DrawForwardMotion(Rect rect, float alpha01)
        {
            GUI.color = WithAlpha(GoldBright, alpha01);
            float y = rect.y + rect.height * 0.86f;
            float chevronHeight = rect.height * 0.05f;
            for (int i = 0; i < 3; i++)
            {
                float width = rect.width * (0.30f - i * 0.06f);
                float x = rect.x + rect.width * 0.5f - width * 0.5f + i * rect.width * 0.05f;
                GUI.Box(new Rect(x, y, width, chevronHeight), string.Empty);
            }
        }

        /// <summary>A bright vertical highlight sweeping left-to-right across the mark once — "premium golden shine" (a flat approximation of a diagonal shine, matching this project's rotation-free OnGUI shape language).</summary>
        private static void DrawShine(Rect rect, float alpha01, float shineProgress01)
        {
            if (shineProgress01 < 0f || shineProgress01 > 1f)
            {
                return;
            }

            float bandWidth = rect.width * 0.16f;
            float x = rect.x - bandWidth + (rect.width + bandWidth * 2f) * shineProgress01;
            GUI.color = WithAlpha(Color.white, alpha01 * 0.30f);
            GUI.Box(new Rect(x, rect.y, bandWidth, rect.height), string.Empty);
        }

        private static Rect Inset(Rect rect, float amount) =>
            new Rect(rect.x + amount, rect.y + amount, rect.width - amount * 2f, rect.height - amount * 2f);

        private static Color WithAlpha(Color color, float alpha01) => new Color(color.r, color.g, color.b, color.a * alpha01);
    }
}

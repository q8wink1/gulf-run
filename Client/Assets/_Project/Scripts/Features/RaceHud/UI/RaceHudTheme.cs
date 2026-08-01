using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.RaceHud.UI
{
    /// <summary>
    /// Modern Gulf Identity palette for the in-race HUD — gold accents, sand
    /// neutrals, high-contrast text. Lives in Features.RaceHud (not MainMenu)
    /// because Features must never reference other Features.
    /// </summary>
    public static class RaceHudTheme
    {
        public static readonly Color Gold = new Color(0.90f, 0.71f, 0.25f, 1f);
        public static readonly Color GoldBright = new Color(1f, 0.84f, 0.40f, 1f);
        public static readonly Color Sand = new Color(0.87f, 0.78f, 0.62f, 1f);
        public static readonly Color SandDark = new Color(0.52f, 0.42f, 0.30f, 1f);
        public static readonly Color PanelBackground = new Color(0.08f, 0.07f, 0.09f, 0.55f);
        public static readonly Color PanelBorderGold = new Color(0.90f, 0.71f, 0.25f, 0.50f);
        public static readonly Color TextPrimary = Color.white;
        public static readonly Color TextMuted = new Color(0.82f, 0.82f, 0.82f, 1f);
        public static readonly Color Success = new Color(0.40f, 0.85f, 0.45f, 1f);
        public static readonly Color Danger = new Color(0.88f, 0.32f, 0.30f, 1f);
        public static readonly Color SpeedBoost = new Color(0.35f, 0.75f, 1f, 1f);
        public static readonly Color Shield = new Color(0.55f, 0.85f, 1f, 1f);
        public static readonly Color Blindness = new Color(0.70f, 0.55f, 0.90f, 1f);
        public static readonly Color SandSlow = new Color(0.85f, 0.65f, 0.30f, 1f);
        public static readonly Color CoffeeStun = new Color(0.90f, 0.40f, 0.35f, 1f);
        public static readonly Color LocalMarker = GoldBright;
        public static readonly Color OpponentMarker = new Color(0.95f, 0.95f, 0.95f, 0.95f);
        public static readonly Color FinishLine = new Color(0.95f, 0.25f, 0.25f, 1f);

        private static GUIStyle _hugeStyle;
        private static GUIStyle _titleStyle;
        private static GUIStyle _labelStyle;
        private static GUIStyle _mutedStyle;
        private static float _builtScale = -1f;

        public static GUIStyle Huge(float scale) { Ensure(scale); return _hugeStyle; }
        public static GUIStyle Title(float scale) { Ensure(scale); return _titleStyle; }
        public static GUIStyle Label(float scale) { Ensure(scale); return _labelStyle; }
        public static GUIStyle Muted(float scale) { Ensure(scale); return _mutedStyle; }

        public static Color ColorFor(HudEffectKind kind) => kind switch
        {
            HudEffectKind.SpeedBoost => SpeedBoost,
            HudEffectKind.Shield => Shield,
            HudEffectKind.Blindness => Blindness,
            HudEffectKind.SandSlow => SandSlow,
            HudEffectKind.CoffeeStun => CoffeeStun,
            _ => TextMuted
        };

        public static void DrawPanel(Rect rect)
        {
            Color previous = GUI.color;
            GUI.color = PanelBorderGold;
            GUI.Box(rect, string.Empty);
            GUI.color = PanelBackground;
            GUI.Box(new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f, rect.height - 4f), string.Empty);
            GUI.color = previous;
        }

        public static void DrawBar(Rect rect, float fill01, Color fill)
        {
            Color previous = GUI.color;
            GUI.color = SandDark;
            GUI.Box(rect, string.Empty);
            float width = rect.width * Mathf.Clamp01(fill01);
            if (width > 1f)
            {
                GUI.color = fill;
                GUI.Box(new Rect(rect.x, rect.y, width, rect.height), string.Empty);
            }

            GUI.color = previous;
        }

        private static void Ensure(float scale)
        {
            if (_hugeStyle != null && Mathf.Approximately(_builtScale, scale))
            {
                return;
            }

            _builtScale = scale;
            _hugeStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.CeilToInt(72f * scale),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _hugeStyle.normal.textColor = GoldBright;

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.CeilToInt(28f * scale),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            _titleStyle.normal.textColor = TextPrimary;

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.CeilToInt(18f * scale),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            _labelStyle.normal.textColor = TextPrimary;

            _mutedStyle = new GUIStyle(_labelStyle) { fontSize = Mathf.CeilToInt(14f * scale) };
            _mutedStyle.normal.textColor = TextMuted;
        }
    }
}

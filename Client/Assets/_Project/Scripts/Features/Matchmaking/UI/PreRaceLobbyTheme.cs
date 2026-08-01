using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Matchmaking.UI
{
    /// <summary>
    /// Sprint 14 Matchmaking "Modern Gulf Identity" for the Pre-Race Lobby —
    /// gold accents, sand neutrals, desert-night panels. Lives in
    /// Features.Matchmaking (not MainMenu) because Features must never
    /// reference other Features.
    /// </summary>
    public static class PreRaceLobbyTheme
    {
        public static readonly Color Gold = new Color(0.90f, 0.71f, 0.25f, 1f);
        public static readonly Color GoldBright = new Color(1f, 0.84f, 0.40f, 1f);
        public static readonly Color Sand = new Color(0.87f, 0.78f, 0.62f, 1f);
        public static readonly Color SandDark = new Color(0.52f, 0.42f, 0.30f, 1f);
        public static readonly Color DesertNight = new Color(0.07f, 0.10f, 0.18f, 0.92f);
        public static readonly Color PanelBackground = new Color(0.10f, 0.09f, 0.10f, 0.78f);
        public static readonly Color PanelBorderGold = new Color(0.90f, 0.71f, 0.25f, 0.55f);
        public static readonly Color TextPrimary = Color.white;
        public static readonly Color TextMuted = new Color(0.80f, 0.80f, 0.80f, 1f);
        public static readonly Color Success = new Color(0.40f, 0.85f, 0.45f, 1f);
        public static readonly Color Danger = new Color(0.88f, 0.32f, 0.30f, 1f);
        public static readonly Color QualityExcellent = new Color(0.35f, 0.90f, 0.45f, 1f);
        public static readonly Color QualityGood = new Color(0.55f, 0.85f, 0.35f, 1f);
        public static readonly Color QualityFair = new Color(0.95f, 0.80f, 0.30f, 1f);
        public static readonly Color QualityPoor = new Color(0.95f, 0.45f, 0.30f, 1f);

        private static GUIStyle _titleStyle;
        private static GUIStyle _headerStyle;
        private static GUIStyle _labelStyle;
        private static GUIStyle _mutedStyle;
        private static GUIStyle _goldButtonStyle;
        private static GUIStyle _panelButtonStyle;
        private static GUIStyle _countdownStyle;

        public static GUIStyle Title { get { Ensure(); return _titleStyle; } }
        public static GUIStyle Header { get { Ensure(); return _headerStyle; } }
        public static GUIStyle Label { get { Ensure(); return _labelStyle; } }
        public static GUIStyle Muted { get { Ensure(); return _mutedStyle; } }
        public static GUIStyle GoldButton { get { Ensure(); return _goldButtonStyle; } }
        public static GUIStyle PanelButton { get { Ensure(); return _panelButtonStyle; } }
        public static GUIStyle Countdown { get { Ensure(); return _countdownStyle; } }

        public static Color ColorFor(ConnectionQuality quality) => quality switch
        {
            ConnectionQuality.Excellent => QualityExcellent,
            ConnectionQuality.Good => QualityGood,
            ConnectionQuality.Fair => QualityFair,
            ConnectionQuality.Poor => QualityPoor,
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

        public static void DrawGoldAccentLine(float x, float y, float width)
        {
            Color previous = GUI.color;
            GUI.color = Gold;
            GUI.Box(new Rect(x, y, width, 2f), string.Empty);
            GUI.color = previous;
        }

        private static void Ensure()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _titleStyle.normal.textColor = GoldBright;

            _headerStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _headerStyle.normal.textColor = TextPrimary;

            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleLeft, wordWrap = true };
            _labelStyle.normal.textColor = TextPrimary;

            _mutedStyle = new GUIStyle(_labelStyle);
            _mutedStyle.normal.textColor = TextMuted;

            _goldButtonStyle = new GUIStyle(GUI.skin.button) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            _goldButtonStyle.normal.textColor = new Color(0.20f, 0.14f, 0.02f, 1f);

            _panelButtonStyle = new GUIStyle(GUI.skin.button) { fontSize = 12, alignment = TextAnchor.MiddleCenter };
            _panelButtonStyle.normal.textColor = TextPrimary;

            _countdownStyle = new GUIStyle(GUI.skin.label) { fontSize = 72, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            _countdownStyle.normal.textColor = GoldBright;
        }
    }
}

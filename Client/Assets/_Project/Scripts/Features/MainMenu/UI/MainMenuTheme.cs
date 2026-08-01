using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.MainMenu.UI
{
    /// <summary>
    /// The single source of truth for the Sprint 13 "Modern Gulf Identity"
    /// palette (warm gold accents, sand neutrals, deep desert-night teal)
    /// and its shared <see cref="GUIStyle"/>s — every Main Menu view pulls
    /// colors/fonts from here instead of inlining its own magic numbers, so
    /// the whole lobby reads as one consistent design system (Code Quality:
    /// "No hardcoded values") and a future palette pass only ever touches
    /// this one file.
    /// </summary>
    public static class MainMenuTheme
    {
        // --- Palette ---
        public static readonly Color Gold = new Color(0.90f, 0.71f, 0.25f, 1f);
        public static readonly Color GoldBright = new Color(1f, 0.84f, 0.40f, 1f);
        public static readonly Color Sand = new Color(0.87f, 0.78f, 0.62f, 1f);
        public static readonly Color SandDark = new Color(0.52f, 0.42f, 0.30f, 1f);
        public static readonly Color DesertNight = new Color(0.07f, 0.10f, 0.18f, 0.92f);
        public static readonly Color PanelBackground = new Color(0.10f, 0.09f, 0.10f, 0.72f);
        public static readonly Color PanelBorderGold = new Color(0.90f, 0.71f, 0.25f, 0.55f);
        public static readonly Color TextPrimary = Color.white;
        public static readonly Color TextMuted = new Color(0.80f, 0.80f, 0.80f, 1f);
        public static readonly Color Success = new Color(0.40f, 0.85f, 0.45f, 1f);
        public static readonly Color Danger = new Color(0.88f, 0.32f, 0.30f, 1f);

        // --- Sky palettes (Sprint 13 "Random: Morning / Sunset / Night") ---
        public static Color SkyTop(TimeOfDay timeOfDay) => timeOfDay switch
        {
            TimeOfDay.Morning => new Color(0.55f, 0.75f, 0.95f, 1f),
            TimeOfDay.Sunset => new Color(0.55f, 0.28f, 0.42f, 1f),
            TimeOfDay.Night => new Color(0.04f, 0.06f, 0.16f, 1f),
            _ => new Color(0.55f, 0.75f, 0.95f, 1f)
        };

        public static Color SkyBottom(TimeOfDay timeOfDay) => timeOfDay switch
        {
            TimeOfDay.Morning => new Color(0.95f, 0.85f, 0.60f, 1f),
            TimeOfDay.Sunset => new Color(0.95f, 0.55f, 0.30f, 1f),
            TimeOfDay.Night => new Color(0.16f, 0.14f, 0.28f, 1f),
            _ => new Color(0.95f, 0.85f, 0.60f, 1f)
        };

        public static Color SunOrMoon(TimeOfDay timeOfDay) => timeOfDay switch
        {
            TimeOfDay.Night => new Color(0.92f, 0.93f, 0.85f, 1f),
            _ => GoldBright
        };

        // --- Styles (lazily built — GUIStyle/GUI.skin are only safe to touch inside OnGUI) ---
        private static GUIStyle _titleStyle;
        private static GUIStyle _headerStyle;
        private static GUIStyle _labelStyle;
        private static GUIStyle _mutedLabelStyle;
        private static GUIStyle _goldButtonStyle;
        private static GUIStyle _panelButtonStyle;
        private static GUIStyle _playButtonStyle;

        public static GUIStyle Title
        {
            get
            {
                EnsureBuilt();
                return _titleStyle;
            }
        }

        public static GUIStyle Header
        {
            get
            {
                EnsureBuilt();
                return _headerStyle;
            }
        }

        public static GUIStyle Label
        {
            get
            {
                EnsureBuilt();
                return _labelStyle;
            }
        }

        public static GUIStyle MutedLabel
        {
            get
            {
                EnsureBuilt();
                return _mutedLabelStyle;
            }
        }

        public static GUIStyle GoldButton
        {
            get
            {
                EnsureBuilt();
                return _goldButtonStyle;
            }
        }

        public static GUIStyle PanelButton
        {
            get
            {
                EnsureBuilt();
                return _panelButtonStyle;
            }
        }

        public static GUIStyle PlayButton
        {
            get
            {
                EnsureBuilt();
                return _playButtonStyle;
            }
        }

        private static void EnsureBuilt()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _titleStyle.normal.textColor = GoldBright;

            _headerStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _headerStyle.normal.textColor = TextPrimary;

            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleLeft, wordWrap = true };
            _labelStyle.normal.textColor = TextPrimary;

            _mutedLabelStyle = new GUIStyle(_labelStyle);
            _mutedLabelStyle.normal.textColor = TextMuted;

            _goldButtonStyle = new GUIStyle(GUI.skin.button) { fontSize = 13, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            _goldButtonStyle.normal.textColor = new Color(0.20f, 0.14f, 0.02f, 1f);

            _panelButtonStyle = new GUIStyle(GUI.skin.button) { fontSize = 12, alignment = TextAnchor.MiddleCenter };
            _panelButtonStyle.normal.textColor = TextPrimary;

            _playButtonStyle = new GUIStyle(GUI.skin.button) { fontSize = 26, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            _playButtonStyle.normal.textColor = new Color(0.20f, 0.14f, 0.02f, 1f);
        }

        /// <summary>Draws a rounded-corner-flavored panel: a soft sand-dark border box behind a slightly inset gold-bordered content box — the closest two flat <see cref="GUI.Box"/> calls can get to "rounded corners + gold accents" (brief) without a 9-sliced sprite atlas (see Sprint 13 report Remaining TODOs re: real Design Bible sprites).</summary>
        public static void DrawPanel(Rect rect)
        {
            Color previous = GUI.color;
            GUI.color = PanelBorderGold;
            GUI.Box(rect, string.Empty);
            GUI.color = PanelBackground;
            GUI.Box(new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f, rect.height - 4f), string.Empty);
            GUI.color = previous;
        }

        /// <summary>A thin gold accent underline — the "subtle Gulf identity...no excessive ornaments" (brief) geometric touch reused under every panel title.</summary>
        public static void DrawGoldAccentLine(float x, float y, float width)
        {
            Color previous = GUI.color;
            GUI.color = Gold;
            GUI.Box(new Rect(x, y, width, 2f), string.Empty);
            GUI.color = previous;
        }
    }
}

using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Character.UI
{
    /// <summary>
    /// Shared Gulf sand/gold palette + styles for Character/Locker screens.
    /// Lives in Features.Character (not MainMenu) so this assembly never
    /// references Features.MainMenu — Features must not reference Features.
    /// </summary>
    public static class CharacterTheme
    {
        public static readonly Color Gold = new Color(0.90f, 0.71f, 0.25f, 1f);
        public static readonly Color GoldBright = new Color(1f, 0.84f, 0.40f, 1f);
        public static readonly Color Sand = new Color(0.87f, 0.78f, 0.62f, 1f);
        public static readonly Color SandDark = new Color(0.52f, 0.42f, 0.30f, 1f);
        public static readonly Color DesertNight = new Color(0.07f, 0.10f, 0.18f, 0.94f);
        public static readonly Color PanelBackground = new Color(0.10f, 0.09f, 0.10f, 0.78f);
        public static readonly Color PanelBorderGold = new Color(0.90f, 0.71f, 0.25f, 0.55f);
        public static readonly Color TextPrimary = Color.white;
        public static readonly Color TextMuted = new Color(0.80f, 0.80f, 0.80f, 1f);
        public static readonly Color MajlisCarpet = new Color(0.55f, 0.12f, 0.14f, 1f);
        public static readonly Color MajlisWall = new Color(0.78f, 0.70f, 0.55f, 1f);
        public static readonly Color LanternGlow = new Color(1f, 0.78f, 0.35f, 0.55f);
        public static readonly Color SoftLight = new Color(1f, 0.95f, 0.85f, 0.18f);

        private static GUIStyle _titleStyle;
        private static GUIStyle _headerStyle;
        private static GUIStyle _labelStyle;
        private static GUIStyle _mutedLabelStyle;
        private static GUIStyle _goldButtonStyle;
        private static GUIStyle _panelButtonStyle;

        public static GUIStyle Title
        {
            get { EnsureBuilt(); return _titleStyle; }
        }

        public static GUIStyle Header
        {
            get { EnsureBuilt(); return _headerStyle; }
        }

        public static GUIStyle Label
        {
            get { EnsureBuilt(); return _labelStyle; }
        }

        public static GUIStyle MutedLabel
        {
            get { EnsureBuilt(); return _mutedLabelStyle; }
        }

        public static GUIStyle GoldButton
        {
            get { EnsureBuilt(); return _goldButtonStyle; }
        }

        public static GUIStyle PanelButton
        {
            get { EnsureBuilt(); return _panelButtonStyle; }
        }

        public static Color RarityColor(CosmeticRarity rarity) => rarity switch
        {
            CosmeticRarity.Common => new Color(0.75f, 0.75f, 0.75f, 1f),
            CosmeticRarity.Rare => new Color(0.30f, 0.55f, 0.95f, 1f),
            CosmeticRarity.Epic => new Color(0.65f, 0.30f, 0.90f, 1f),
            CosmeticRarity.Legendary => new Color(0.95f, 0.70f, 0.15f, 1f),
            CosmeticRarity.Mythic => new Color(0.95f, 0.25f, 0.45f, 1f),
            _ => Color.white
        };

        public static Color RarityGlow(CosmeticRarity rarity, float pulse01)
        {
            Color baseColor = RarityColor(rarity);
            float intensity = rarity switch
            {
                CosmeticRarity.Common => 0.15f,
                CosmeticRarity.Rare => 0.35f,
                CosmeticRarity.Epic => 0.50f,
                CosmeticRarity.Legendary => 0.70f,
                CosmeticRarity.Mythic => 0.90f,
                _ => 0.2f
            };
            baseColor.a = intensity * (0.55f + 0.45f * pulse01);
            return baseColor;
        }

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

        public static void DrawRarityCard(Rect rect, CosmeticRarity rarity, float pulse01, bool flash)
        {
            Color previous = GUI.color;
            GUI.color = RarityGlow(rarity, flash ? 1f : pulse01);
            GUI.Box(new Rect(rect.x - 3f, rect.y - 3f, rect.width + 6f, rect.height + 6f), string.Empty);
            GUI.color = RarityColor(rarity);
            GUI.Box(rect, string.Empty);
            GUI.color = PanelBackground;
            GUI.Box(new Rect(rect.x + 3f, rect.y + 3f, rect.width - 6f, rect.height - 6f), string.Empty);
            GUI.color = previous;
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
        }
    }
}

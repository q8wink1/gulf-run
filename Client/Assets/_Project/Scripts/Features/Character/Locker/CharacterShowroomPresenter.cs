using GulfRun.Domain;
using GulfRun.Features.Character.Configuration;
using GulfRun.Features.Character.UI;
using UnityEngine;

namespace GulfRun.Features.Character.Locker
{
    /// <summary>
    /// Draws the premium Gulf Majlis showroom + character silhouette with
    /// cinematic rotate/zoom/auto-focus (OnGUI placeholders).
    /// </summary>
    public sealed class CharacterShowroomPresenter
    {
        private float _yawDegrees;
        private float _zoom = 1f;
        private float _focusBlend = 1f;
        private float _targetZoom = 1f;

        public float YawDegrees => _yawDegrees;
        public float Zoom => _zoom;

        public void Reset(LockerUiConfig config)
        {
            _yawDegrees = 0f;
            _zoom = 1f;
            _targetZoom = 1f;
            _focusBlend = 1f;
        }

        public void Tick(float deltaTime, LockerUiConfig config, bool autoRotate)
        {
            if (config == null)
            {
                return;
            }

            if (autoRotate)
            {
                _yawDegrees = (_yawDegrees + config.RotateDegreesPerSecond * deltaTime) % 360f;
            }

            _zoom = Mathf.Lerp(_zoom, _targetZoom, 1f - Mathf.Exp(-config.AutoFocusLerpSpeed * deltaTime));
            _focusBlend = Mathf.Clamp01(_focusBlend + deltaTime / Mathf.Max(0.01f, config.CameraTransitionSeconds));
        }

        public void RotateBy(float degrees) => _yawDegrees = (_yawDegrees + degrees) % 360f;

        public void ZoomBy(float delta, LockerUiConfig config)
        {
            if (config == null)
            {
                return;
            }

            _targetZoom = Mathf.Clamp(_targetZoom + delta, config.ZoomMin, config.ZoomMax);
        }

        public void AutoFocus(LockerUiConfig config)
        {
            if (config == null)
            {
                return;
            }

            _targetZoom = Mathf.Clamp(1f, config.ZoomMin, config.ZoomMax);
            _focusBlend = 0f;
        }

        public void Draw(
            Rect rect,
            Color characterColor,
            string characterLabel,
            Color outfitAccent,
            CharacterPreviewAnimator animator,
            LockerUiConfig config,
            GulfCountry country,
            string cityHint)
        {
            DrawMajlisBackground(rect, country, cityHint);

            float breath = animator != null ? animator.BreathOffsetY(config, Time.unscaledTime) : 0f;
            float sway = animator != null ? animator.IdleSwayDegrees(config, Time.unscaledTime) : 0f;
            float facing = Mathf.Abs(Mathf.Cos((_yawDegrees + sway) * Mathf.Deg2Rad));
            float width = 70f * _zoom * (0.65f + 0.35f * facing);
            float height = 130f * _zoom;
            float cx = rect.x + rect.width * 0.5f;
            float cy = rect.y + rect.height * 0.58f + breath;

            Color previous = GUI.color;

            // Soft spotlight
            GUI.color = CharacterTheme.SoftLight;
            GUI.Box(new Rect(cx - width * 1.2f, cy - height * 0.85f, width * 2.4f, height * 1.5f), string.Empty);

            // Character body (silhouette)
            GUI.color = characterColor;
            GUI.Box(new Rect(cx - width * 0.5f, cy - height, width, height), string.Empty);

            // Outfit accent sash
            GUI.color = outfitAccent;
            GUI.Box(new Rect(cx - width * 0.45f, cy - height * 0.55f, width * 0.9f, height * 0.18f), string.Empty);

            // Head
            float headSize = width * 0.55f;
            GUI.color = characterColor;
            GUI.Box(new Rect(cx - headSize * 0.5f, cy - height - headSize * 0.65f, headSize, headSize), string.Empty);

            // Eyes / blink
            if (animator == null || !animator.EyesClosed)
            {
                GUI.color = Color.white;
                float eyeY = cy - height - headSize * 0.35f;
                GUI.Box(new Rect(cx - headSize * 0.22f, eyeY, 6f, 6f), string.Empty);
                GUI.Box(new Rect(cx + headSize * 0.08f, eyeY, 6f, 6f), string.Empty);
            }

            // Smile
            GUI.color = new Color(0.2f, 0.1f, 0.1f, 0.8f);
            GUI.Box(new Rect(cx - 10f, cy - height - headSize * 0.12f, 20f, 4f), string.Empty);

            // Animation pose cue (legs / arms as simple offset boxes)
            DrawPoseOverlay(cx, cy, width, height, animator);

            GUI.color = previous;
            GUI.Label(new Rect(rect.x + 10f, rect.y + rect.height - 28f, rect.width - 20f, 22f), characterLabel, CharacterTheme.Label);
            GUI.Label(
                new Rect(rect.x + 10f, rect.y + 8f, rect.width - 20f, 20f),
                "Yaw " + Mathf.RoundToInt(_yawDegrees) + "°  Zoom " + _zoom.ToString("0.00") + "  " + (animator != null ? animator.State.ToString() : "Idle"),
                CharacterTheme.MutedLabel);
        }

        private static void DrawPoseOverlay(float cx, float cy, float width, float height, CharacterPreviewAnimator animator)
        {
            if (animator == null)
            {
                return;
            }

            Color previous = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.35f);
            switch (animator.State)
            {
                case CharacterAnimationState.Run:
                    GUI.Box(new Rect(cx - width * 0.7f, cy - height * 0.3f, width * 0.35f, 10f), string.Empty);
                    GUI.Box(new Rect(cx + width * 0.35f, cy - height * 0.25f, width * 0.35f, 10f), string.Empty);
                    break;
                case CharacterAnimationState.DoubleJump:
                    GUI.Box(new Rect(cx - width * 0.15f, cy - height * 1.25f, width * 0.3f, 12f), string.Empty);
                    break;
                case CharacterAnimationState.Win:
                case CharacterAnimationState.Celebrate:
                    GUI.Box(new Rect(cx - width * 0.85f, cy - height * 0.95f, 12f, height * 0.45f), string.Empty);
                    GUI.Box(new Rect(cx + width * 0.7f, cy - height * 0.95f, 12f, height * 0.45f), string.Empty);
                    break;
                case CharacterAnimationState.Lose:
                    GUI.Box(new Rect(cx - width * 0.2f, cy - height * 0.15f, width * 0.4f, 14f), string.Empty);
                    break;
            }

            GUI.color = previous;
        }

        private static void DrawMajlisBackground(Rect rect, GulfCountry country, string cityHint)
        {
            Color previous = GUI.color;

            // Architecture wall
            GUI.color = CharacterTheme.MajlisWall;
            GUI.Box(rect, string.Empty);

            // Large window overlooking Gulf city
            Rect window = new Rect(rect.x + rect.width * 0.12f, rect.y + 16f, rect.width * 0.76f, rect.height * 0.32f);
            GUI.color = CitySkyColor(country);
            GUI.Box(window, string.Empty);
            GUI.color = CitySkylineColor(country);
            GUI.Box(new Rect(window.x + 8f, window.yMax - 36f, window.width - 16f, 28f), string.Empty);
            GUI.Box(new Rect(window.x + window.width * 0.2f, window.yMax - 70f, 18f, 55f), string.Empty);
            GUI.Box(new Rect(window.x + window.width * 0.55f, window.yMax - 90f, 22f, 75f), string.Empty);
            GUI.Box(new Rect(window.x + window.width * 0.75f, window.yMax - 60f, 14f, 45f), string.Empty);

            // Luxury carpet
            GUI.color = CharacterTheme.MajlisCarpet;
            GUI.Box(new Rect(rect.x + 20f, rect.y + rect.height * 0.62f, rect.width - 40f, rect.height * 0.28f), string.Empty);
            GUI.color = CharacterTheme.Gold;
            GUI.Box(new Rect(rect.x + 28f, rect.y + rect.height * 0.66f, rect.width - 56f, 3f), string.Empty);

            // Palm decorations
            GUI.color = new Color(0.15f, 0.45f, 0.22f, 1f);
            GUI.Box(new Rect(rect.x + 18f, rect.y + rect.height * 0.38f, 14f, rect.height * 0.28f), string.Empty);
            GUI.Box(new Rect(rect.xMax - 32f, rect.y + rect.height * 0.40f, 14f, rect.height * 0.26f), string.Empty);
            GUI.Box(new Rect(rect.x + 8f, rect.y + rect.height * 0.36f, 34f, 10f), string.Empty);
            GUI.Box(new Rect(rect.xMax - 42f, rect.y + rect.height * 0.38f, 34f, 10f), string.Empty);

            // Arabian lanterns + soft lighting
            float pulse = 0.65f + 0.35f * Mathf.Sin(Time.unscaledTime * 2f);
            Color lantern = CharacterTheme.LanternGlow;
            lantern.a *= pulse;
            GUI.color = lantern;
            GUI.Box(new Rect(rect.x + 40f, rect.y + 40f, 16f, 22f), string.Empty);
            GUI.Box(new Rect(rect.xMax - 56f, rect.y + 40f, 16f, 22f), string.Empty);

            // Coffee set
            GUI.color = new Color(0.35f, 0.22f, 0.12f, 1f);
            GUI.Box(new Rect(rect.x + rect.width * 0.42f, rect.y + rect.height * 0.72f, 36f, 14f), string.Empty);
            GUI.color = CharacterTheme.GoldBright;
            GUI.Box(new Rect(rect.x + rect.width * 0.45f, rect.y + rect.height * 0.69f, 12f, 10f), string.Empty);

            GUI.color = previous;
            GUI.Label(new Rect(window.x + 8f, window.y + 4f, window.width - 16f, 18f), cityHint, CharacterTheme.MutedLabel);
        }

        private static Color CitySkyColor(GulfCountry country) => country switch
        {
            GulfCountry.Kuwait => new Color(0.45f, 0.70f, 0.95f, 1f),
            GulfCountry.SaudiArabia => new Color(0.55f, 0.75f, 0.95f, 1f),
            GulfCountry.UnitedArabEmirates => new Color(0.35f, 0.55f, 0.85f, 1f),
            GulfCountry.Qatar => new Color(0.50f, 0.65f, 0.90f, 1f),
            GulfCountry.Bahrain => new Color(0.60f, 0.78f, 0.95f, 1f),
            GulfCountry.Oman => new Color(0.55f, 0.72f, 0.88f, 1f),
            GulfCountry.Iraq => new Color(0.70f, 0.80f, 0.92f, 1f),
            GulfCountry.Egypt => new Color(0.75f, 0.82f, 0.90f, 1f),
            _ => new Color(0.50f, 0.70f, 0.92f, 1f)
        };

        private static Color CitySkylineColor(GulfCountry country) => country switch
        {
            GulfCountry.UnitedArabEmirates => new Color(0.15f, 0.18f, 0.28f, 1f),
            GulfCountry.SaudiArabia => new Color(0.25f, 0.22f, 0.20f, 1f),
            GulfCountry.Kuwait => new Color(0.20f, 0.22f, 0.30f, 1f),
            _ => new Color(0.22f, 0.24f, 0.30f, 1f)
        };

        public static string CityHintFor(GulfCountry country) => country switch
        {
            GulfCountry.Kuwait => "Window: Kuwait City skyline",
            GulfCountry.SaudiArabia => "Window: Riyadh skyline",
            GulfCountry.UnitedArabEmirates => "Window: Dubai skyline",
            GulfCountry.Qatar => "Window: Doha Corniche",
            GulfCountry.Bahrain => "Window: Manama skyline",
            GulfCountry.Oman => "Window: Muscat coast",
            GulfCountry.Iraq => "Window: Baghdad skyline",
            GulfCountry.Egypt => "Window: Cairo skyline",
            _ => "Window: Gulf city skyline"
        };
    }
}

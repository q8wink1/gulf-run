using UnityEngine;

namespace GulfRun.Features.Character.Configuration
{
    /// <summary>
    /// Tunables for the Sprint 16 Locker / Character Showroom — camera,
    /// animation, layout scaling, rarity glow. No magic numbers in views.
    /// </summary>
    [CreateAssetMenu(fileName = "LockerUiConfig", menuName = "GulfRun/Character/Locker UI Config")]
    public sealed class LockerUiConfig : ScriptableObject
    {
        [Header("Layout (reference resolution for responsive scaling)")]
        [SerializeField] private float referenceWidth = 1920f;
        [SerializeField] private float referenceHeight = 1080f;
        [SerializeField] private float phoneScaleFloor = 0.85f;
        [SerializeField] private float tabletScaleCeiling = 1.25f;

        [Header("Camera")]
        [SerializeField] private float rotateDegreesPerSecond = 45f;
        [SerializeField] private float zoomMin = 0.75f;
        [SerializeField] private float zoomMax = 1.45f;
        [SerializeField] private float zoomStep = 0.08f;
        [SerializeField] private float autoFocusLerpSpeed = 4f;
        [SerializeField] private float cameraTransitionSeconds = 0.35f;

        [Header("Preview animations")]
        [SerializeField] private float breathAmplitude = 3f;
        [SerializeField] private float breathHz = 0.35f;
        [SerializeField] private float blinkClosedSeconds = 0.12f;
        [SerializeField] private float blinkOpenMinSeconds = 2.2f;
        [SerializeField] private float blinkOpenMaxSeconds = 4.5f;
        [SerializeField] private float animationTransitionSeconds = 0.28f;
        [SerializeField] private float idleSwayDegrees = 2.5f;

        [Header("Rarity presentation")]
        [SerializeField] private float rarityGlowPulseHz = 1.2f;
        [SerializeField] private float rarityRewardFlashSeconds = 0.55f;

        [Header("Temporary expiry UI")]
        [SerializeField] private float temporaryTimerRefreshSeconds = 1f;

        public float ReferenceWidth => referenceWidth;
        public float ReferenceHeight => referenceHeight;
        public float PhoneScaleFloor => phoneScaleFloor;
        public float TabletScaleCeiling => tabletScaleCeiling;
        public float RotateDegreesPerSecond => rotateDegreesPerSecond;
        public float ZoomMin => zoomMin;
        public float ZoomMax => zoomMax;
        public float ZoomStep => zoomStep;
        public float AutoFocusLerpSpeed => autoFocusLerpSpeed;
        public float CameraTransitionSeconds => cameraTransitionSeconds;
        public float BreathAmplitude => breathAmplitude;
        public float BreathHz => breathHz;
        public float BlinkClosedSeconds => blinkClosedSeconds;
        public float BlinkOpenMinSeconds => blinkOpenMinSeconds;
        public float BlinkOpenMaxSeconds => blinkOpenMaxSeconds;
        public float AnimationTransitionSeconds => animationTransitionSeconds;
        public float IdleSwayDegrees => idleSwayDegrees;
        public float RarityGlowPulseHz => rarityGlowPulseHz;
        public float RarityRewardFlashSeconds => rarityRewardFlashSeconds;
        public float TemporaryTimerRefreshSeconds => temporaryTimerRefreshSeconds;

        /// <summary>
        /// Uniform scale vs 1920×1080 reference with match≈0.5 (average of
        /// width/height factors), clamped for phone/tablet extremes.
        /// </summary>
        public float ResolveUiScale()
        {
            float sx = Screen.width / Mathf.Max(1f, referenceWidth);
            float sy = Screen.height / Mathf.Max(1f, referenceHeight);
            float scale = Mathf.Lerp(sx, sy, 0.5f);
            return Mathf.Clamp(scale, phoneScaleFloor, tabletScaleCeiling);
        }
    }
}

using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.4 — designer-exposed runner movement tuning.
    /// Forward speed, lanes, jump, and slide — no hardcoded gameplay values in motors.
    /// </summary>
    [CreateAssetMenu(fileName = "RunnerMovementConfig", menuName = "GulfRun/Gameplay/Runner Movement Config")]
    public sealed class RunnerMovementConfig : ScriptableObject
    {
        [Header("Forward")]
        [SerializeField] private float forwardSpeed = 12f;
        [Tooltip("Reserved for future progressive speed-ups (multipliers stack on base).")]
        [SerializeField] private float speedMultiplier = 1f;

        [Header("Lanes")]
        [SerializeField] private float laneSpacing = 2.2f;
        [SerializeField] private float laneChangeDuration = 0.22f;
        [SerializeField] private float laneCenterX;

        [Header("Jump")]
        [SerializeField] private float jumpHeight = 2.4f;
        [SerializeField] private float gravity = 28f;
        [SerializeField] private float groundedY;

        [Header("Slide")]
        [SerializeField] private float slideDuration = 0.75f;
        [SerializeField] private float slideColliderHeight = 0.9f;
        [SerializeField] private float normalColliderHeight = 1.8f;
        [SerializeField] private float normalColliderCenterY = 0.9f;
        [SerializeField] private float slideColliderCenterY = 0.45f;

        [Header("Swipe / Input")]
        [SerializeField] private float swipeThresholdPixels = 48f;

        public float ForwardSpeed => forwardSpeed;
        public float SpeedMultiplier => speedMultiplier;
        public float EffectiveForwardSpeed => forwardSpeed * Mathf.Max(0f, speedMultiplier);
        public float LaneSpacing => laneSpacing;
        public float LaneChangeDuration => Mathf.Max(0.05f, laneChangeDuration);
        public float LaneCenterX => laneCenterX;
        public float JumpHeight => jumpHeight;
        public float Gravity => gravity;
        public float GroundedY => groundedY;
        public float SlideDuration => Mathf.Max(0.05f, slideDuration);
        public float SlideColliderHeight => slideColliderHeight;
        public float NormalColliderHeight => normalColliderHeight;
        public float NormalColliderCenterY => normalColliderCenterY;
        public float SlideColliderCenterY => slideColliderCenterY;
        public float SwipeThresholdPixels => swipeThresholdPixels;

        /// <summary>Initial upward velocity for a jump that peaks at <see cref="JumpHeight"/> under <see cref="Gravity"/>.</summary>
        public float JumpVelocity => Mathf.Sqrt(2f * gravity * Mathf.Max(0.01f, jumpHeight));
    }
}

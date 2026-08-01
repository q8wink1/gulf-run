using System;
using UnityEngine;

namespace GulfRun.Features.PlayerController
{
    /// <summary>
    /// Detects ground contact and landing transitions with a short downward
    /// <see cref="Physics2D.CircleCast"/> instead of a static overlap check,
    /// so a surface is only counted as "ground" when it is actually beneath
    /// the player (via the surface normal) — this avoids false positives
    /// from touching a wall or the underside of a platform. Reports the
    /// specific <see cref="GroundCollider"/> so callers (e.g. <see cref="PlayerMotor"/>)
    /// can look for optional capabilities on it, such as
    /// <see cref="GulfRun.Core.Platforms.IMovingPlatform"/>. Any collider on
    /// the configured ground layer counts — ground, static platforms, and
    /// moving platforms are all "Ground" from this detector's point of view.
    /// Kept independent from <see cref="PlayerMotor"/> so ground sensing can
    /// be unit-tested or reused (e.g. by the debug view) without depending
    /// on motor internals.
    /// </summary>
    public sealed class PlayerGroundDetector : MonoBehaviour
    {
        [SerializeField] private Transform groundCheckOrigin;
        [SerializeField] private PlayerMovementConfig config;

        public bool IsGrounded { get; private set; }

        /// <summary>The specific collider currently supporting the player, or null when airborne.</summary>
        public Collider2D GroundCollider { get; private set; }

        /// <summary>Raised exactly once on the frame the player transitions from airborne to grounded.</summary>
        public event Action Landed;

        private bool _wasGrounded;

        private void FixedUpdate()
        {
            RaycastHit2D hit = Physics2D.CircleCast(
                groundCheckOrigin.position,
                config.GroundCheckRadius,
                Vector2.down,
                config.GroundCheckDistance,
                config.GroundLayerMask);

            bool grounded = hit.collider != null && hit.normal.y >= config.MinGroundNormalY;

            if (grounded && !_wasGrounded)
            {
                Landed?.Invoke();
            }

            IsGrounded = grounded;
            GroundCollider = grounded ? hit.collider : null;
            _wasGrounded = grounded;
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheckOrigin == null || config == null)
            {
                return;
            }

            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheckOrigin.position, config.GroundCheckRadius);
        }
    }
}

using System;
using UnityEngine;

namespace GulfRun.Features.PlayerController
{
    /// <summary>
    /// Detects ground contact and landing transitions using a simple overlap
    /// check against the configured ground layer. Kept independent from
    /// <see cref="PlayerMotor"/> so ground sensing can be unit-tested or
    /// reused (e.g. by the debug view) without depending on motor internals.
    /// </summary>
    public sealed class PlayerGroundDetector : MonoBehaviour
    {
        [SerializeField] private Transform groundCheckOrigin;
        [SerializeField] private PlayerMovementConfig config;

        public bool IsGrounded { get; private set; }

        /// <summary>Raised exactly once on the frame the player transitions from airborne to grounded.</summary>
        public event Action Landed;

        private bool _wasGrounded;

        private void FixedUpdate()
        {
            bool grounded = Physics2D.OverlapCircle(
                groundCheckOrigin.position,
                config.GroundCheckRadius,
                config.GroundLayerMask);

            if (grounded && !_wasGrounded)
            {
                Landed?.Invoke();
            }

            IsGrounded = grounded;
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

using System;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.PlayerController
{
    /// <summary>
    /// Drives the player's <see cref="Rigidbody2D"/>: constant auto-run,
    /// jump / double jump, and movement-state resolution. Does not read input
    /// or drive animation directly (see <see cref="PlayerInputReader"/> and
    /// <see cref="PlayerAnimatorDriver"/>) so each concern stays independently
    /// testable and swappable — including, later, for networked/authoritative
    /// re-simulation.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerGroundDetector))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private PlayerMovementConfig config;

        private Rigidbody2D _rigidbody2D;
        private PlayerGroundDetector _groundDetector;
        private int _jumpsUsed;
        private bool _justLanded;
        private bool _isRunEnabled = true;

        public PlayerMovementState CurrentState { get; private set; } = PlayerMovementState.Idle;
        public float CurrentSpeed => Mathf.Abs(_rigidbody2D.velocity.x);
        public float VerticalVelocity => _rigidbody2D.velocity.y;

        /// <summary>Raised whenever <see cref="CurrentState"/> changes.</summary>
        public event Action<PlayerMovementState> StateChanged;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _groundDetector = GetComponent<PlayerGroundDetector>();
            _rigidbody2D.gravityScale = config.GravityScale;
        }

        private void OnEnable()
        {
            _groundDetector.Landed += HandleLanded;
        }

        private void OnDisable()
        {
            _groundDetector.Landed -= HandleLanded;
        }

        private void FixedUpdate()
        {
            Vector2 velocity = _rigidbody2D.velocity;
            velocity.x = _isRunEnabled ? config.AutoRunSpeed : 0f;
            _rigidbody2D.velocity = velocity;

            if (_groundDetector.IsGrounded)
            {
                _jumpsUsed = 0;
            }

            PlayerMovementState resolved = PlayerMovementStateResolver.Resolve(
                isGrounded: _groundDetector.IsGrounded,
                justLanded: _justLanded,
                verticalVelocity: _rigidbody2D.velocity.y,
                horizontalSpeed: CurrentSpeed,
                isRunEnabled: _isRunEnabled);

            _justLanded = false;
            SetState(resolved);
        }

        /// <summary>
        /// Requests a jump. Applies the primary jump force while grounded/first
        /// airborne frame, or the double-jump force while airborne with jumps remaining.
        /// </summary>
        public void RequestJump()
        {
            if (_jumpsUsed >= config.MaxJumpCount)
            {
                return;
            }

            float force = _jumpsUsed == 0 ? config.JumpForce : config.DoubleJumpForce;
            Vector2 velocity = _rigidbody2D.velocity;
            velocity.y = force;
            _rigidbody2D.velocity = velocity;
            _jumpsUsed++;

            SetState(PlayerMovementState.Jumping);
        }

        private void HandleLanded()
        {
            _justLanded = true;
        }

        private void SetState(PlayerMovementState newState)
        {
            if (newState == CurrentState)
            {
                return;
            }

            CurrentState = newState;
            StateChanged?.Invoke(newState);
        }
    }
}

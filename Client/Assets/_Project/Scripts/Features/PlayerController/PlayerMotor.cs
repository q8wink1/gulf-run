using System;
using GulfRun.Core.Platforms;
using GulfRun.Core.Services;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.PlayerController
{
    /// <summary>
    /// Drives the player's <see cref="Rigidbody2D"/>: constant auto-run
    /// toward the right, strict double jump, and movement-state resolution.
    /// Does not read input or drive animation directly (see
    /// <see cref="PlayerInputReader"/> and <see cref="PlayerAnimatorDriver"/>)
    /// so each concern stays independently testable and swappable —
    /// including, later, for networked/authoritative re-simulation.
    ///
    /// Auto-run and jumping are only active while the session's game state
    /// (if any is registered via <see cref="GameStateService"/>) is
    /// <see cref="GameLoopState.Running"/> — the player is held in place
    /// during <see cref="GameLoopState.Countdown"/>/GameOver/Paused with
    /// zero coupling to the EndlessRunner feature. When no session is
    /// registered (e.g. this prefab used stand-alone, as in Sprint 2), the
    /// player always runs, preserving Sprint 2 behaviour exactly.
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
        private bool _isRunEnabled;

        public PlayerMovementState CurrentState { get; private set; } = PlayerMovementState.Idle;
        public float CurrentSpeed => Mathf.Abs(_rigidbody2D.velocity.x);
        public float VerticalVelocity => _rigidbody2D.velocity.y;
        public int JumpsUsed => _jumpsUsed;
        public int MaxJumpCount => config.MaxJumpCount;

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
            _isRunEnabled = ResolveRunEnabled();

            Vector2 velocity = _rigidbody2D.velocity;
            velocity.x = _isRunEnabled ? ResolveAutoRunSpeed() : 0f;
            _rigidbody2D.velocity = velocity;

            if (_groundDetector.IsGrounded)
            {
                _jumpsUsed = 0;
                ApplyMovingPlatformDelta();
            }

            PlayerMovementState resolved = ResolveMovementState();
            _justLanded = false;
            SetState(resolved);
        }

        /// <summary>
        /// Requests a jump. Strict double jump: one jump while grounded, one
        /// additional jump while airborne, then every further request is
        /// ignored until the player lands on a valid ground/platform surface
        /// again. Ignored entirely outside <see cref="GameLoopState.Running"/>
        /// (e.g. during the race-start countdown or after game over).
        /// </summary>
        public void RequestJump()
        {
            if (!_isRunEnabled)
            {
                return;
            }

            if (_jumpsUsed >= config.MaxJumpCount)
            {
                return;
            }

            bool isFirstJump = _jumpsUsed == 0;
            float force = isFirstJump ? config.JumpForce : config.DoubleJumpForce;
            Vector2 velocity = _rigidbody2D.velocity;
            velocity.y = force;
            _rigidbody2D.velocity = velocity;
            _jumpsUsed++;

            SetState(isFirstJump ? PlayerMovementState.Jumping : PlayerMovementState.DoubleJumping);
        }

        private void HandleLanded()
        {
            _justLanded = true;
        }

        /// <summary>
        /// Resolves the current auto-run speed. When an endless-runner Game
        /// Speed Controller is present in the scene it drives the speed
        /// (base speed, progressive increase, temporary modifiers); otherwise
        /// falls back to the static config value, so this component behaves
        /// identically to Sprint 2 when used stand-alone. Decoupled via
        /// <see cref="GulfRun.Core.Services.RunSpeedService"/> — PlayerController
        /// never references the EndlessRunner feature directly.
        /// </summary>
        private float ResolveAutoRunSpeed()
        {
            IRunSpeedProvider provider = RunSpeedService.Current;
            return provider != null ? provider.CurrentSpeed : config.AutoRunSpeed;
        }

        /// <summary>Auto-run/jump are enabled only while the session is Running (or no session is registered at all).</summary>
        private bool ResolveRunEnabled()
        {
            IGameStateProvider provider = GameStateService.Current;
            return provider == null || provider.CurrentState == GameLoopState.Running;
        }

        /// <summary>
        /// Carries the player along with whatever they are standing on, if
        /// that surface implements <see cref="IMovingPlatform"/> (e.g. a
        /// moving platform). No-op for plain static ground.
        /// </summary>
        private void ApplyMovingPlatformDelta()
        {
            Collider2D groundCollider = _groundDetector.GroundCollider;
            if (groundCollider == null)
            {
                return;
            }

            IMovingPlatform platform = groundCollider.GetComponent<IMovingPlatform>();
            if (platform == null)
            {
                return;
            }

            _rigidbody2D.position += platform.FrameDelta;
        }

        /// <summary>
        /// Countdown/GameOver take priority over the physics-derived state so
        /// the player visibly holds still during the race-start countdown and
        /// after the run ends, even though physics itself may still be settling.
        /// </summary>
        private PlayerMovementState ResolveMovementState()
        {
            IGameStateProvider provider = GameStateService.Current;
            if (provider != null)
            {
                if (provider.CurrentState == GameLoopState.Countdown)
                {
                    return PlayerMovementState.Countdown;
                }

                if (provider.CurrentState == GameLoopState.GameOver)
                {
                    return PlayerMovementState.GameOver;
                }
            }

            return PlayerMovementStateResolver.Resolve(
                isGrounded: _groundDetector.IsGrounded,
                justLanded: _justLanded,
                verticalVelocity: _rigidbody2D.velocity.y,
                horizontalSpeed: CurrentSpeed,
                isRunEnabled: _isRunEnabled,
                jumpsUsed: _jumpsUsed);
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

using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.4 — core 3-lane runner motor (transform-based).
    /// Auto-run +Z, lane X interpolation, single jump, timed slide with collider shrink.
    /// No obstacles, coins, networking, or race logic. Camera-follow ready via Transform.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RunnerSwipeInput))]
    public sealed class RunnerPlayerController : MonoBehaviour
    {
        [SerializeField] private RunnerMovementConfig config;
        [SerializeField] private CapsuleCollider bodyCollider;
        [SerializeField] private RunnerLane startingLane = RunnerLane.Center;

        private RunnerSwipeInput _input;
        private RunnerLane _lane;
        private RunnerLane _targetLane;
        private bool _isChangingLane;
        private float _laneT;
        private float _laneFromX;
        private float _laneToX;
        private float _verticalVelocity;
        private bool _isGrounded = true;
        private bool _isSliding;
        private float _slideRemaining;
        private PlayerMovementState _state = PlayerMovementState.Idle;
        private float _normalHeight;
        private float _normalCenterY;
        private float _speedScale = 1f;

        public Transform FollowTarget => transform;
        public RunnerLane CurrentLane => _lane;
        public PlayerMovementState MovementState => _state;
        public bool IsGrounded => _isGrounded;
        public bool IsSliding => _isSliding;
        public bool IsChangingLane => _isChangingLane;
        public float VerticalVelocity => _verticalVelocity;
        public float ForwardSpeed => config != null ? config.EffectiveForwardSpeed * _speedScale : 0f;

        /// <summary>Future progressive speed-ups without rewriting the motor.</summary>
        public void SetSpeedScale(float scale) => _speedScale = Mathf.Max(0f, scale);

        private void Awake()
        {
            _input = GetComponent<RunnerSwipeInput>();
            if (bodyCollider == null)
            {
                bodyCollider = GetComponent<CapsuleCollider>();
            }

            EnsureKinematicBody();

            if (config != null && _input != null)
            {
                _input.SetSwipeThreshold(config.SwipeThresholdPixels);
            }

            CacheColliderDefaults();
            _lane = startingLane;
            _targetLane = startingLane;
            ApplyLaneXImmediate(_lane);
        }

        /// <summary>
        /// Sprint 23.10 — kinematic Rigidbody so obstacle trigger colliders register
        /// hits while the motor remains transform-based.
        /// </summary>
        private void EnsureKinematicBody()
        {
            Rigidbody body = GetComponent<Rigidbody>();
            if (body == null)
            {
                body = gameObject.AddComponent<Rigidbody>();
            }

            body.isKinematic = true;
            body.useGravity = false;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        private void OnEnable()
        {
            if (_input == null)
            {
                return;
            }

            _input.LaneLeft += OnLaneLeft;
            _input.LaneRight += OnLaneRight;
            _input.Jump += OnJump;
            _input.Slide += OnSlide;
        }

        private void OnDisable()
        {
            if (_input == null)
            {
                return;
            }

            _input.LaneLeft -= OnLaneLeft;
            _input.LaneRight -= OnLaneRight;
            _input.Jump -= OnJump;
            _input.Slide -= OnSlide;
        }

        private void Update()
        {
            if (config == null)
            {
                return;
            }

            float dt = Time.deltaTime;
            IntegrateForward(dt);
            IntegrateLane(dt);
            IntegrateVertical(dt);
            IntegrateSlide(dt);
            ResolveState();
        }

        private void OnLaneLeft()
        {
            if (_isChangingLane || config == null)
            {
                return;
            }

            RunnerLane next = RunnerLaneMath.Shift(_lane, -1);
            if (next == _lane)
            {
                return;
            }

            BeginLaneChange(next);
        }

        private void OnLaneRight()
        {
            if (_isChangingLane || config == null)
            {
                return;
            }

            RunnerLane next = RunnerLaneMath.Shift(_lane, 1);
            if (next == _lane)
            {
                return;
            }

            BeginLaneChange(next);
        }

        private void OnJump()
        {
            if (config == null || !_isGrounded || _isSliding)
            {
                return;
            }

            _verticalVelocity = config.JumpVelocity;
            _isGrounded = false;
            _state = PlayerMovementState.Jumping;
        }

        private void OnSlide()
        {
            if (config == null || !_isGrounded || _isSliding)
            {
                return;
            }

            _isSliding = true;
            _slideRemaining = config.SlideDuration;
            ApplySlideCollider(true);
            _state = PlayerMovementState.Sliding;
        }

        private void BeginLaneChange(RunnerLane next)
        {
            _targetLane = next;
            _isChangingLane = true;
            _laneT = 0f;
            _laneFromX = transform.position.x;
            _laneToX = RunnerLaneMath.LaneX(next, config.LaneCenterX, config.LaneSpacing);
        }

        private void IntegrateForward(float dt)
        {
            Vector3 pos = transform.position;
            pos.z += ForwardSpeed * dt;
            transform.position = pos;
        }

        private void IntegrateLane(float dt)
        {
            if (!_isChangingLane)
            {
                return;
            }

            _laneT += dt / config.LaneChangeDuration;
            float t = _laneT < 1f ? (_laneT * _laneT * (3f - (2f * _laneT))) : 1f;
            Vector3 pos = transform.position;
            pos.x = Mathf.Lerp(_laneFromX, _laneToX, t);
            transform.position = pos;

            if (_laneT >= 1f)
            {
                _lane = _targetLane;
                _isChangingLane = false;
                pos.x = _laneToX;
                transform.position = pos;
            }
        }

        private void IntegrateVertical(float dt)
        {
            Vector3 pos = transform.position;
            if (_isGrounded)
            {
                pos.y = config.GroundedY;
                _verticalVelocity = 0f;
                transform.position = pos;
                return;
            }

            _verticalVelocity -= config.Gravity * dt;
            pos.y += _verticalVelocity * dt;
            if (pos.y <= config.GroundedY)
            {
                pos.y = config.GroundedY;
                _verticalVelocity = 0f;
                _isGrounded = true;
                // Landing animation placeholder — state lands on Landing for one resolve cycle.
                _state = PlayerMovementState.Landing;
            }

            transform.position = pos;
        }

        private void IntegrateSlide(float dt)
        {
            if (!_isSliding)
            {
                return;
            }

            _slideRemaining -= dt;
            if (_slideRemaining > 0f)
            {
                return;
            }

            _isSliding = false;
            ApplySlideCollider(false);
        }

        private void ResolveState()
        {
            if (_isSliding && _isGrounded)
            {
                _state = PlayerMovementState.Sliding;
                return;
            }

            if (!_isGrounded)
            {
                _state = _verticalVelocity > 0.01f
                    ? PlayerMovementState.Jumping
                    : PlayerMovementState.Falling;
                return;
            }

            if (_state == PlayerMovementState.Landing)
            {
                // One-frame landing placeholder then back to run.
                _state = PlayerMovementState.Running;
                return;
            }

            _state = ForwardSpeed > 0.01f ? PlayerMovementState.Running : PlayerMovementState.Idle;
        }

        private void ApplyLaneXImmediate(RunnerLane lane)
        {
            if (config == null)
            {
                return;
            }

            Vector3 pos = transform.position;
            pos.x = RunnerLaneMath.LaneX(lane, config.LaneCenterX, config.LaneSpacing);
            pos.y = config.GroundedY;
            transform.position = pos;
        }

        private void CacheColliderDefaults()
        {
            if (bodyCollider == null)
            {
                return;
            }

            if (config != null)
            {
                _normalHeight = config.NormalColliderHeight;
                _normalCenterY = config.NormalColliderCenterY;
                bodyCollider.height = _normalHeight;
                Vector3 center = bodyCollider.center;
                center.y = _normalCenterY;
                bodyCollider.center = center;
                return;
            }

            _normalHeight = bodyCollider.height;
            _normalCenterY = bodyCollider.center.y;
        }

        private void ApplySlideCollider(bool sliding)
        {
            if (bodyCollider == null || config == null)
            {
                return;
            }

            bodyCollider.height = sliding ? config.SlideColliderHeight : _normalHeight;
            Vector3 center = bodyCollider.center;
            center.y = sliding ? config.SlideColliderCenterY : _normalCenterY;
            bodyCollider.center = center;
        }
    }
}

using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.PlayerController
{
    /// <summary>
    /// Translates <see cref="PlayerMotor"/> state into Animator parameters.
    /// Matches the parameters declared on
    /// Assets/_Project/Animations/PlayerAnimatorController.controller:
    /// IsGrounded (bool), Speed (float), VerticalVelocity (float),
    /// JumpTrigger (trigger), DoubleJumpTrigger (trigger). The controller
    /// currently uses placeholder (empty) motions for every state — see the
    /// Sprint 3 report for the "swap in real clips" follow-up.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public sealed class PlayerAnimatorDriver : MonoBehaviour
    {
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");
        private static readonly int JumpTriggerHash = Animator.StringToHash("JumpTrigger");
        private static readonly int DoubleJumpTriggerHash = Animator.StringToHash("DoubleJumpTrigger");

        private Animator _animator;
        private PlayerMotor _motor;

        /// <summary>Wires this driver to a motor instance. Called by <see cref="PlayerController"/>.</summary>
        public void Initialize(PlayerMotor motor)
        {
            _motor = motor;
            _motor.StateChanged += HandleStateChanged;
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnDestroy()
        {
            if (_motor != null)
            {
                _motor.StateChanged -= HandleStateChanged;
            }
        }

        private void Update()
        {
            if (_motor == null)
            {
                return;
            }

            _animator.SetBool(IsGroundedHash, _motor.CurrentState != PlayerMovementState.Jumping
                                               && _motor.CurrentState != PlayerMovementState.DoubleJumping
                                               && _motor.CurrentState != PlayerMovementState.Falling);
            _animator.SetFloat(SpeedHash, _motor.CurrentSpeed);
            _animator.SetFloat(VerticalVelocityHash, _motor.VerticalVelocity);
        }

        private void HandleStateChanged(PlayerMovementState newState)
        {
            if (newState == PlayerMovementState.Jumping)
            {
                _animator.SetTrigger(JumpTriggerHash);
            }
            else if (newState == PlayerMovementState.DoubleJumping)
            {
                _animator.SetTrigger(DoubleJumpTriggerHash);
            }
        }
    }
}

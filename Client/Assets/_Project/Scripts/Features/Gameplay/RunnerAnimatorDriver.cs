using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.4 — animation state placeholders for the runner.
    /// Drives Animator bool/trigger params when present; otherwise exposes
    /// <see cref="CurrentAnim"/> for visual debugging / future mesh swaps.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RunnerAnimatorDriver : MonoBehaviour
    {
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int IsSlidingHash = Animator.StringToHash("IsSliding");
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");
        private static readonly int JumpTriggerHash = Animator.StringToHash("JumpTrigger");
        private static readonly int SlideTriggerHash = Animator.StringToHash("SlideTrigger");
        private static readonly int HitTriggerHash = Animator.StringToHash("HitTrigger");

        [SerializeField] private RunnerPlayerController runner;
        [SerializeField] private Animator animator;

        private PlayerMovementState _lastState;
        private CharacterAnimationState _currentAnim = CharacterAnimationState.Idle;

        public CharacterAnimationState CurrentAnim => _currentAnim;

        /// <summary>
        /// Sprint 23.10 — placeholder hit cue. Fires Animator HitTrigger when present;
        /// no locomotion change / penalty.
        /// </summary>
        public void PrepareHit()
        {
            if (animator != null)
            {
                animator.SetTrigger(HitTriggerHash);
            }
        }

        private void Awake()
        {
            if (runner == null)
            {
                runner = GetComponent<RunnerPlayerController>();
            }

            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }

        private void Update()
        {
            if (runner == null)
            {
                return;
            }

            PlayerMovementState state = runner.MovementState;
            _currentAnim = MapAnim(state);

            if (animator != null)
            {
                animator.SetBool(IsGroundedHash, runner.IsGrounded);
                animator.SetBool(IsSlidingHash, runner.IsSliding);
                animator.SetFloat(SpeedHash, runner.ForwardSpeed);
                animator.SetFloat(VerticalVelocityHash, runner.VerticalVelocity);

                if (state != _lastState)
                {
                    if (state == PlayerMovementState.Jumping)
                    {
                        animator.SetTrigger(JumpTriggerHash);
                    }
                    else if (state == PlayerMovementState.Sliding)
                    {
                        animator.SetTrigger(SlideTriggerHash);
                    }
                }
            }

            _lastState = state;
        }

        private static CharacterAnimationState MapAnim(PlayerMovementState state) => state switch
        {
            PlayerMovementState.Idle => CharacterAnimationState.Idle,
            PlayerMovementState.Running => CharacterAnimationState.Run,
            PlayerMovementState.Jumping => CharacterAnimationState.Jump,
            PlayerMovementState.Falling => CharacterAnimationState.Fall,
            PlayerMovementState.Landing => CharacterAnimationState.Run,
            PlayerMovementState.Sliding => CharacterAnimationState.Slide,
            _ => CharacterAnimationState.Idle
        };
    }
}

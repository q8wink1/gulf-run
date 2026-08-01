using GulfRun.Core.Services;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.PlayerController
{
    /// <summary>
    /// Translates <see cref="PlayerMotor"/> state into Animator parameters.
    /// Matches the parameters declared on
    /// Assets/_Project/Animations/PlayerAnimatorController.controller:
    /// IsGrounded (bool), Speed (float), VerticalVelocity (float),
    /// JumpTrigger/DoubleJumpTrigger (Sprint 3), and — Sprint 8 —
    /// WinTrigger/LoseTrigger/CelebrateTrigger, completing the full
    /// "Idle, Run, Jump, Double Jump, Fall, Win, Lose, Celebrate" animation
    /// vocabulary the Sprint 8 brief requires (see
    /// <see cref="Domain.CharacterAnimationState"/>/
    /// <see cref="Domain.CharacterAnimationResolver"/>). Every character and
    /// every cosmetic loadout drives this exact same Animator Controller
    /// asset — swapping a character/outfit only ever swaps the visible mesh,
    /// never the controller or its parameters — which is what makes the
    /// system "Shared Animation Controller... Reusable Assets... Minimal
    /// Memory Usage" (Sprint 8 Performance requirement) rather than one
    /// controller per character. The controller currently uses placeholder
    /// (empty) motions for every state — see the Sprint 3 report for the
    /// "swap in real clips" follow-up.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public sealed class PlayerAnimatorDriver : MonoBehaviour
    {
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");
        private static readonly int JumpTriggerHash = Animator.StringToHash("JumpTrigger");
        private static readonly int DoubleJumpTriggerHash = Animator.StringToHash("DoubleJumpTrigger");
        private static readonly int WinTriggerHash = Animator.StringToHash("WinTrigger");
        private static readonly int LoseTriggerHash = Animator.StringToHash("LoseTrigger");
        private static readonly int CelebrateTriggerHash = Animator.StringToHash("CelebrateTrigger");

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

        private void OnEnable()
        {
            CharacterAnimationCueService.LocalCueRaised += HandleLocalCueRaised;
        }

        private void OnDisable()
        {
            CharacterAnimationCueService.LocalCueRaised -= HandleLocalCueRaised;
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

        /// <summary>Win/Lose/Celebrate — the three <see cref="CharacterAnimationState"/> members that come from race-outcome/ceremony events (Features.RaceFinish) rather than locomotion, delivered via the decoupled <see cref="CharacterAnimationCueService"/> seam.</summary>
        private void HandleLocalCueRaised(CharacterAnimationState state)
        {
            switch (state)
            {
                case CharacterAnimationState.Win:
                    _animator.SetTrigger(WinTriggerHash);
                    break;
                case CharacterAnimationState.Lose:
                    _animator.SetTrigger(LoseTriggerHash);
                    break;
                case CharacterAnimationState.Celebrate:
                    _animator.SetTrigger(CelebrateTriggerHash);
                    break;
            }
        }
    }
}

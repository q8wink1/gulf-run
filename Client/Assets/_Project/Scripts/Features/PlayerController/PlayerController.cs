using UnityEngine;

namespace GulfRun.Features.PlayerController
{
    /// <summary>
    /// Composition root for the player character. Wires the independent
    /// single-responsibility components together (input, motor, animation)
    /// instead of having them reference each other directly, so any one of
    /// them can be replaced (e.g. a networked input reader) without touching
    /// the others.
    /// </summary>
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(PlayerGroundDetector))]
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerAnimatorDriver))]
    public sealed class PlayerController : MonoBehaviour
    {
        private PlayerMotor _motor;
        private PlayerInputReader _inputReader;
        private PlayerAnimatorDriver _animatorDriver;

        private void Awake()
        {
            _motor = GetComponent<PlayerMotor>();
            _inputReader = GetComponent<PlayerInputReader>();
            _animatorDriver = GetComponent<PlayerAnimatorDriver>();

            _animatorDriver.Initialize(_motor);
        }

        private void OnEnable()
        {
            _inputReader.JumpPressed += _motor.RequestJump;
        }

        private void OnDisable()
        {
            _inputReader.JumpPressed -= _motor.RequestJump;
        }
    }
}

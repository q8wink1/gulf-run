using UnityEngine;

namespace GulfRun.Features.PlayerController
{
    /// <summary>
    /// On-screen/gizmo debug visualization for ground check, movement state,
    /// and current speed. Compiled only into the Editor and development
    /// builds — never shipped in release — mirroring the intent of
    /// GulfRun.Debug without adding a Features -> Debug or Debug -> Features
    /// assembly dependency (this stays inside the PlayerController feature).
    /// </summary>
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(PlayerGroundDetector))]
    public sealed class PlayerDebugView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private bool showOnScreenDebug = true;

        private PlayerMotor _motor;
        private PlayerGroundDetector _groundDetector;

        private void Awake()
        {
            _motor = GetComponent<PlayerMotor>();
            _groundDetector = GetComponent<PlayerGroundDetector>();
        }

        private void OnGUI()
        {
            if (!showOnScreenDebug)
            {
                return;
            }

            GUI.Label(new Rect(10, 10, 320, 20), $"State: {_motor.CurrentState}");
            GUI.Label(new Rect(10, 28, 320, 20), $"Grounded: {_groundDetector.IsGrounded}");
            GUI.Label(new Rect(10, 46, 320, 20), $"Speed: {_motor.CurrentSpeed:F2} m/s");
            GUI.Label(new Rect(10, 64, 320, 20), $"Vertical Velocity: {_motor.VerticalVelocity:F2} m/s");
            GUI.Label(new Rect(10, 82, 320, 20), $"Jumps Used: {_motor.JumpsUsed}/{_motor.MaxJumpCount}");
        }
#endif
    }
}

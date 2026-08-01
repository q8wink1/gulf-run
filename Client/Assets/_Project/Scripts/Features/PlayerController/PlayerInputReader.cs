using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GulfRun.Features.PlayerController
{
    /// <summary>
    /// Polls the active input devices and raises a single, device-agnostic
    /// jump event. Built against the Unity Input System package (the project's
    /// Active Input Handling is set to "Input System Package (New)" — see
    /// Sprint 1 ProjectSettings.asset), not the legacy UnityEngine.Input API.
    /// Keyboard support is compiled only into the Editor / development builds,
    /// per the Sprint 2 brief ("Keyboard: Development Only").
    /// </summary>
    public sealed class PlayerInputReader : MonoBehaviour
    {
        /// <summary>Raised the frame a jump is requested via touch, mouse, or (dev-only) keyboard.</summary>
        public event Action JumpPressed;

        private void Update()
        {
            if (WasJumpPressedThisFrame())
            {
                JumpPressed?.Invoke();
            }
        }

        private static bool WasJumpPressedThisFrame()
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                return true;
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                return true;
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                return true;
            }
#endif

            return false;
        }
    }
}

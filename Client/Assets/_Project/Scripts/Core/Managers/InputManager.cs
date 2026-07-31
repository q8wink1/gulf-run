using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Central entry point for player input. Intended to wrap the Unity Input
    /// System package once it is resolved by the Editor; kept dependency-free
    /// in this sprint so the project compiles without package resolution.
    /// References: P049 (Technical Architecture, Input Manager).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class InputManager : Singleton<InputManager>
    {
        protected override void OnInitialize()
        {
            // TODO(Sprint 2+): Bind generated Input Actions asset once the
            // Input System package is resolved and control schemes are defined.
        }
    }
}

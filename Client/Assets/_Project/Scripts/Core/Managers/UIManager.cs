using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Coordinates screen/popup navigation and the shared UI design language.
    /// References: P047 (UI / UX Design System).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UIManager : Singleton<UIManager>
    {
        protected override void OnInitialize()
        {
            // TODO(Sprint 2+): Implement screen stack / popup navigation once
            // concrete UI screens and design tokens (P047) are produced.
        }
    }
}

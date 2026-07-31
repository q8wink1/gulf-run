using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Handles scene loading and transitions between Boot, MainMenu, Loading,
    /// Gameplay and Results. Named "SceneManager" per the approved Sprint 1
    /// brief; callers inside this file must fully qualify
    /// <see cref="UnityEngine.SceneManagement.SceneManager"/> to avoid
    /// ambiguity with this type.
    /// References: P049 (Technical Architecture), Sprint 1 (Scenes).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SceneManager : Singleton<SceneManager>
    {
        protected override void OnInitialize()
        {
            // TODO(Sprint 2+): Implement scene load/unload flow (e.g. via
            // UnityEngine.SceneManagement.SceneManager and/or Addressables)
            // once transition rules are specified.
        }
    }
}

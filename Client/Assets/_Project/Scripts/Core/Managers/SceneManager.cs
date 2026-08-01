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
    /// Sprint 13 adds the two calls the Main Menu's PLAY button/Gameplay
    /// exit flow actually need — still a thin placeholder (no
    /// Loading-scene/async/Addressables handoff yet, see Sprint 13 report
    /// Remaining TODOs) but enough for both scenes to actually transition
    /// instead of only existing side by side.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SceneManager : Singleton<SceneManager>
    {
        /// <summary>Scene file names — kept as one named constant per scene (Code Quality: "no hardcoded values") instead of inline string literals at every call site.</summary>
        public const string MainMenuSceneName = "MainMenu";

        public const string GameplaySceneName = "Gameplay";

        protected override void OnInitialize()
        {
            // TODO(Sprint 2+): Implement a real Loading-scene/async/Addressables
            // scene transition flow once transition rules are specified —
            // LoadGameplay/LoadMainMenu below are still a direct, synchronous
            // placeholder load.
        }

        /// <summary>Sprint 13 (Main Menu PLAY button): leaves the lobby scene for the Gameplay scene.</summary>
        public void LoadGameplay() => UnityEngine.SceneManagement.SceneManager.LoadScene(GameplaySceneName);

        /// <summary>Sprint 13 (post-match/leave-match flow): returns to the Main Menu scene.</summary>
        public void LoadMainMenu() => UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenuSceneName);
    }
}

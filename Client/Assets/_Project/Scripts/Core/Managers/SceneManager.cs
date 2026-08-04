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
    /// Sprint 14 adds <see cref="LoadIntro"/> — Boot now always hands off
    /// to the Brand Intro scene first (see <c>GameManager.Start</c>), which
    /// itself calls <see cref="LoadMainMenu"/> once it finishes/is skipped.
    /// Sprint 15 adds <see cref="LoadLobby"/> — Quick Play/Private Room now
    /// hand off to the new Pre-Race Lobby scene instead of jumping straight
    /// from Main Menu to Gameplay (see <c>Features.MainMenu.Bottom.PlayButtonView</c>
    /// and the new <c>Features.MainMenu.Bottom.PrivateRoomPanelView</c>); the
    /// Lobby scene itself calls <see cref="LoadGameplay"/> once its Auto
    /// Start countdown reaches GO.
    /// Character Selection adds <see cref="LoadCharacterSelection"/> — Main Menu
    /// Play Now opens the Character Selection UI scene before matchmaking.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SceneManager : Singleton<SceneManager>
    {
        /// <summary>Scene file names — kept as one named constant per scene (Code Quality: "no hardcoded values") instead of inline string literals at every call site.</summary>
        public const string IntroSceneName = "Intro";

        public const string MainMenuSceneName = "MainMenu";

        /// <summary>Character Selection UI scene — entered from Main Menu Play Now before matchmaking.</summary>
        public const string CharacterSelectionSceneName = "CharacterSelection";

        /// <summary>Sprint 15: the Pre-Race Lobby scene, entered once a Quick Play match is found or a Private Room is created/joined.</summary>
        public const string LobbySceneName = "Lobby";

        public const string GameplaySceneName = "Gameplay";

        protected override void OnInitialize()
        {
            // TODO(Sprint 2+): Implement a real Loading-scene/async/Addressables
            // scene transition flow once transition rules are specified —
            // LoadGameplay/LoadMainMenu below are still a direct, synchronous
            // placeholder load.
        }

        /// <summary>Sprint 14 (Boot startup): the very first scene load after Boot finishes initializing — the GulfRun Brand Intro.</summary>
        public void LoadIntro() => UnityEngine.SceneManagement.SceneManager.LoadScene(IntroSceneName);

        /// <summary>Main Menu Play Now → Character Selection (UI layout only; unlock/select gameplay comes later).</summary>
        public void LoadCharacterSelection() =>
            UnityEngine.SceneManagement.SceneManager.LoadScene(CharacterSelectionSceneName);

        /// <summary>Sprint 15 (Quick Play match found / Private Room created or joined): leaves Main Menu for the Pre-Race Lobby scene.</summary>
        public void LoadLobby() => UnityEngine.SceneManagement.SceneManager.LoadScene(LobbySceneName);

        /// <summary>Sprint 13 (Main Menu PLAY button)/Sprint 15 (Pre-Race Lobby Auto Start "GO"): leaves the current scene for the Gameplay scene.</summary>
        public void LoadGameplay() => UnityEngine.SceneManagement.SceneManager.LoadScene(GameplaySceneName);

        /// <summary>Sprint 13 (post-match/leave-match flow)/Sprint 14 (end of Brand Intro)/Sprint 15 (Cancel/Leave Room from the Lobby): returns to (or first enters) the Main Menu scene.</summary>
        public void LoadMainMenu() => UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenuSceneName);
    }
}

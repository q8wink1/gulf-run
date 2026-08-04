using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Handles scene loading and transitions between Boot, MainMenu, Loading,
    /// Gameplay and Results. Named "SceneManager" per the approved Sprint 1
    /// brief; callers inside this file must fully qualify
    /// <see cref="UnityEngine.SceneManagement.SceneManager"/> to avoid
    /// ambiguity with this type.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SceneManager : Singleton<SceneManager>
    {
        public const string IntroSceneName = "Intro";
        public const string MainMenuSceneName = "MainMenu";
        public const string PlayMenuSceneName = "PlayMenu";
        public const string QuickPlaySceneName = "QuickPlay";
        public const string InviteFriendsSceneName = "InviteFriends";
        public const string LobbyScreenSceneName = "LobbyScreen";
        public const string LobbySceneName = "Lobby";
        public const string MapVotingSceneName = "MapVoting";
        public const string WinningMapRevealSceneName = "WinningMapReveal";
        public const string LoadingScreenSceneName = "LoadingScreen";
        public const string PreRaceIntroSceneName = "PreRaceIntro";
        public const string LoadingSceneName = "Loading";
        public const string GameplaySceneName = "Gameplay";

        protected override void OnInitialize()
        {
            // TODO(Sprint 2+): real Loading-scene/async/Addressables handoff.
        }

        public void LoadIntro() => UnityEngine.SceneManagement.SceneManager.LoadScene(IntroSceneName);

        public void LoadPlayMenu() => UnityEngine.SceneManagement.SceneManager.LoadScene(PlayMenuSceneName);

        public void LoadQuickPlay() => UnityEngine.SceneManagement.SceneManager.LoadScene(QuickPlaySceneName);

        public void LoadInviteFriends() => UnityEngine.SceneManagement.SceneManager.LoadScene(InviteFriendsSceneName);

        /// <summary>Premium Lobby UI foundation (Sprint 21.1). Distinct from pre-race <see cref="LoadLobby"/>.</summary>
        public void LoadLobbyScreen() => UnityEngine.SceneManagement.SceneManager.LoadScene(LobbyScreenSceneName);

        public void LoadLobby() => UnityEngine.SceneManagement.SceneManager.LoadScene(LobbySceneName);

        public void LoadMapVoting() => UnityEngine.SceneManagement.SceneManager.LoadScene(MapVotingSceneName);

        public void LoadWinningMapReveal() => UnityEngine.SceneManagement.SceneManager.LoadScene(WinningMapRevealSceneName);

        /// <summary>Premium Loading Screen UI (Sprint 22.5). Distinct from gameplay <see cref="LoadLoading"/>.</summary>
        public void LoadLoadingScreen() => UnityEngine.SceneManagement.SceneManager.LoadScene(LoadingScreenSceneName);

        /// <summary>Pre-Race Intro UI (Sprint 23.1). Presentation only — no race / countdown logic.</summary>
        public void LoadPreRaceIntro() => UnityEngine.SceneManagement.SceneManager.LoadScene(PreRaceIntroSceneName);

        public void LoadLoading() => UnityEngine.SceneManagement.SceneManager.LoadScene(LoadingSceneName);

        public void LoadGameplay() => UnityEngine.SceneManagement.SceneManager.LoadScene(GameplaySceneName);

        public void LoadMainMenu() => UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenuSceneName);
    }
}

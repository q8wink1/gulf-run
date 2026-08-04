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

        public void LoadIntro() => LoadSceneLogged(IntroSceneName, nameof(LoadIntro));

        public void LoadPlayMenu() => LoadSceneLogged(PlayMenuSceneName, nameof(LoadPlayMenu));

        public void LoadQuickPlay() => LoadSceneLogged(QuickPlaySceneName, nameof(LoadQuickPlay));

        public void LoadInviteFriends() => LoadSceneLogged(InviteFriendsSceneName, nameof(LoadInviteFriends));

        /// <summary>Premium Lobby UI foundation (Sprint 21.1). Distinct from pre-race <see cref="LoadLobby"/>.</summary>
        public void LoadLobbyScreen() => LoadSceneLogged(LobbyScreenSceneName, nameof(LoadLobbyScreen));

        public void LoadLobby() => LoadSceneLogged(LobbySceneName, nameof(LoadLobby));

        public void LoadMapVoting() => LoadSceneLogged(MapVotingSceneName, nameof(LoadMapVoting));

        public void LoadWinningMapReveal() =>
            LoadSceneLogged(WinningMapRevealSceneName, nameof(LoadWinningMapReveal));

        /// <summary>Premium Loading Screen UI (Sprint 22.5). Distinct from gameplay <see cref="LoadLoading"/>.</summary>
        public void LoadLoadingScreen() => LoadSceneLogged(LoadingScreenSceneName, nameof(LoadLoadingScreen));

        /// <summary>Pre-Race Intro + Sprint 23.2 countdown overlay. Presentation only — no movement / networking.</summary>
        public void LoadPreRaceIntro() => LoadSceneLogged(PreRaceIntroSceneName, nameof(LoadPreRaceIntro));

        public void LoadLoading() => LoadSceneLogged(LoadingSceneName, nameof(LoadLoading));

        public void LoadGameplay() => LoadSceneLogged(GameplaySceneName, nameof(LoadGameplay));

        public void LoadMainMenu() => LoadSceneLogged(MainMenuSceneName, nameof(LoadMainMenu));

        /// <summary>
        /// Shared LoadScene with before/after logs. Callers that find
        /// <see cref="Instance"/> null should invoke
        /// <see cref="UnityEngine.SceneManagement.SceneManager.LoadScene(string)"/>
        /// directly with the matching scene-name constant.
        /// </summary>
        private static void LoadSceneLogged(string sceneName, string methodName)
        {
            try
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[SceneManager] " + methodName + "() LoadScene('" + sceneName
                    + "') threw: " + ex);
                throw;
            }
        }
    }
}

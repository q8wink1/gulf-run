using GulfRun.Core.Managers;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Scene-gated visibility for Boot DontDestroyOnLoad OnGUI chrome (Locker,
    /// Account Creation, Multiplayer/Character debug). Those components live on
    /// Boot singletons and would otherwise paint over Lobby / Gameplay.
    /// </summary>
    public static class PersistentUiScope
    {
        public static bool IsMainMenuActive =>
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == SceneManager.MainMenuSceneName;

        public static bool IsLobbyActive =>
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == SceneManager.LobbySceneName;

        /// <summary>Main-menu-only entry chrome and account creation.</summary>
        public static bool AllowsMainMenuChrome => IsMainMenuActive;

        /// <summary>Dev overlays that must not cover the Pre-Race Lobby.</summary>
        public static bool AllowsPersistentDebugOverlay => IsMainMenuActive;
    }
}

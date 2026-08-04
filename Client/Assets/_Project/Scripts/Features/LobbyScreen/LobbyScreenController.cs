using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.LobbyScreen
{
    /// <summary>
    /// Premium Lobby UI foundation (Sprint 21.1). Back → Play Menu only.
    /// Slot/header/footer content is static placeholder data in the scene —
    /// no SessionManager, matchmaking, ready, or host logic.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LobbyScreenController : MonoBehaviour
    {
        [SerializeField] private Button backButton;

        private void Awake()
        {
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }
        }

        private void OnDestroy()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveListener(OnBackClicked);
            }
        }

        private static void OnBackClicked()
        {
            // Leave any Quick Play match started before this UI-only screen loaded,
            // so returning to Play Menu does not immediately re-enter LobbyScreen.
            MatchLobbySummaryService.Current?.CancelOrLeaveMatch();

            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadPlayMenu();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.PlayMenuSceneName);
        }
    }
}

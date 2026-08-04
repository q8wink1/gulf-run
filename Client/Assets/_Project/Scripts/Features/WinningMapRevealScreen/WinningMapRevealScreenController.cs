using GulfRun.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.WinningMapRevealScreen
{
    /// <summary>
    /// Winning Map Reveal UI (Sprint 22.4). Placeholder continue → LoadingScreen.
    /// No winner calculation, networking, loading logic, or gameplay.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WinningMapRevealScreenController : MonoBehaviour
    {
        [SerializeField] private Button continueButton;

        private void Awake()
        {
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueClicked);
            }
        }

        private void OnDestroy()
        {
            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(OnContinueClicked);
            }
        }

        private static void OnContinueClicked()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadLoadingScreen();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.LoadingScreenSceneName);
        }
    }
}

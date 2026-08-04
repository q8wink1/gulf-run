using GulfRun.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.PreRaceIntro
{
    /// <summary>
    /// Pre-Race Intro UI (Sprint 23.1). Presentation placeholders only —
    /// no countdown, movement, networking, or race logic.
    /// Optional Continue stub loads Gameplay for Editor flow testing.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PreRaceIntroController : MonoBehaviour
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
                SceneManager.Instance.LoadGameplay();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.GameplaySceneName);
        }
    }
}

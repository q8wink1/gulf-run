using GulfRun.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.LoadingScreen
{
    /// <summary>
    /// Premium Loading Screen UI (Sprint 22.5). Visual-only placeholders —
    /// no scene load progress, networking, sync, or gameplay logic.
    /// Optional Continue stub loads Gameplay for Editor flow testing.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LoadingScreenController : MonoBehaviour
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private RectTransform spinner;
        [SerializeField] private float spinnerDegreesPerSecond = 180f;

        private void Awake()
        {
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueClicked);
            }
        }

        private void Update()
        {
            if (spinner == null || spinnerDegreesPerSecond == 0f)
            {
                return;
            }

            spinner.Rotate(0f, 0f, -spinnerDegreesPerSecond * Time.unscaledDeltaTime);
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

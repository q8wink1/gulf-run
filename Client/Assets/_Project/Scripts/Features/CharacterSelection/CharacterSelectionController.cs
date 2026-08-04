using GulfRun.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.CharacterSelection
{
    /// <summary>
    /// Thin Character Selection UI wiring: Back returns to Main Menu.
    /// Select / arrows are present for layout only (no unlock or gameplay).
    /// Named stage roots support adding more characters later without redesign.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CharacterSelectionController : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button selectCharacterButton;
        [SerializeField] private Button arrowLeftButton;
        [SerializeField] private Button arrowRightButton;

        private void Awake()
        {
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }

            // Select / arrows: UI placeholders only — hooks reserved for a later sprint.
            if (selectCharacterButton != null)
            {
                selectCharacterButton.onClick.AddListener(OnSelectClicked);
            }

            if (arrowLeftButton != null)
            {
                arrowLeftButton.onClick.AddListener(OnArrowLeftClicked);
            }

            if (arrowRightButton != null)
            {
                arrowRightButton.onClick.AddListener(OnArrowRightClicked);
            }
        }

        private void OnDestroy()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveListener(OnBackClicked);
            }

            if (selectCharacterButton != null)
            {
                selectCharacterButton.onClick.RemoveListener(OnSelectClicked);
            }

            if (arrowLeftButton != null)
            {
                arrowLeftButton.onClick.RemoveListener(OnArrowLeftClicked);
            }

            if (arrowRightButton != null)
            {
                arrowRightButton.onClick.RemoveListener(OnArrowRightClicked);
            }
        }

        private void OnBackClicked()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadMainMenu();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.MainMenuSceneName);
        }

        private static void OnSelectClicked()
        {
            // Intentionally empty — Select Character is UI-only until unlock/flow exists.
        }

        private static void OnArrowLeftClicked()
        {
            // Intentionally empty — character carousel wired in a later sprint.
        }

        private static void OnArrowRightClicked()
        {
            // Intentionally empty — character carousel wired in a later sprint.
        }
    }
}

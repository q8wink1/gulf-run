using GulfRun.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.MainMenu
{
    /// <summary>
    /// Thin Play Now wiring on Main Menu <c>PlayButtonImage</c>: opens Play Menu.
    /// Does not alter layout, sprites, or RectTransforms — Button + this script only.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class MainMenuPlayButton : MonoBehaviour
    {
        private void Awake()
        {
            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnPlayClicked);
            }
        }

        private void OnDestroy()
        {
            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveListener(OnPlayClicked);
            }
        }

        private static void OnPlayClicked()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadPlayMenu();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.PlayMenuSceneName);
        }
    }
}

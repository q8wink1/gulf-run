using GulfRun.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.MainMenu
{
    /// <summary>
    /// Minimal Play Now click wiring on the Main Menu canvas Play button image.
    /// Loads Character Selection without changing layout or artwork.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class MainMenuPlayButton : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnPlayClicked);
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnPlayClicked);
            }
        }

        private void OnPlayClicked()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadCharacterSelection();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.CharacterSelectionSceneName);
        }
    }
}

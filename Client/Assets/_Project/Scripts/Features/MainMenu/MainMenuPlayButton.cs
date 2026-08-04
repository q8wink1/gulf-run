using GulfRun.Core.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private Button _button;
        private bool _runtimeListenerAdded;

        private void Awake()
        {
            EnsureEventSystem();

            _button = GetComponent<Button>();
            if (_button == null)
            {
                return;
            }

            _button.interactable = true;

            Image graphic = _button.targetGraphic as Image;
            if (graphic == null)
            {
                graphic = GetComponent<Image>();
                if (graphic != null)
                {
                    _button.targetGraphic = graphic;
                }
            }

            if (graphic != null)
            {
                graphic.raycastTarget = true;
            }

            // Prefer Inspector/persistent OnClick; add a runtime listener only if none are wired.
            if (_button.onClick.GetPersistentEventCount() == 0)
            {
                _button.onClick.AddListener(OnPlayClicked);
                _runtimeListenerAdded = true;
            }
        }

        private void OnDestroy()
        {
            if (_button != null && _runtimeListenerAdded)
            {
                _button.onClick.RemoveListener(OnPlayClicked);
            }
        }

        /// <summary>Opens Play Menu (Single load). Safe for Button OnClick persistent calls.</summary>
        public void OnPlayClicked()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadPlayMenu();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.PlayMenuSceneName);
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            if (FindObjectOfType<EventSystem>() != null)
            {
                return;
            }

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }
}

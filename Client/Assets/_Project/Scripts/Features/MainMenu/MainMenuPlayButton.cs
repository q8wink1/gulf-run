using GulfRun.Core.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

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
        private bool _navigating;

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

            // Always add a runtime listener so Play works even if Inspector persistent
            // OnClick target is missing/broken after assembly reloads.
            _button.onClick.AddListener(OnPlayClicked);
            _runtimeListenerAdded = true;
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
            // Persistent + runtime listeners can both fire for one click.
            if (_navigating)
            {
                return;
            }

            _navigating = true;
            Debug.Log("Play button clicked");

            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadPlayMenu();
                return;
            }

            Debug.Log("[MainMenuPlayButton] SceneManager.Instance null — LoadPlayMenu via SceneManager.LoadScene('" + SceneManager.PlayMenuSceneName + "')");
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.PlayMenuSceneName);
        }

        private static void EnsureEventSystem()
        {
            EventSystem es = EventSystem.current;
            if (es == null)
            {
                es = FindObjectOfType<EventSystem>();
            }

            if (es == null)
            {
                GameObject go = new GameObject("EventSystem");
                es = go.AddComponent<EventSystem>();
                AddUiInputModule(go);
                return;
            }

            EnsureUiInputModule(es.gameObject);
        }

        private static void AddUiInputModule(GameObject go) => EnsureUiInputModule(go);

        private static void EnsureUiInputModule(GameObject go)
        {
#if ENABLE_INPUT_SYSTEM
            // Prefer Input System UI module; StandaloneInputModule often receives no
            // pointer events when the project has com.unity.inputsystem enabled.
            StandaloneInputModule legacy = go.GetComponent<StandaloneInputModule>();
            if (legacy != null)
            {
                DestroyImmediate(legacy);
            }

            if (go.GetComponent<InputSystemUIInputModule>() == null)
            {
                InputSystemUIInputModule uiModule = go.AddComponent<InputSystemUIInputModule>();
                uiModule.AssignDefaultActions();
            }
#else
            if (go.GetComponent<StandaloneInputModule>() == null)
            {
                go.AddComponent<StandaloneInputModule>();
            }
#endif
        }
    }
}

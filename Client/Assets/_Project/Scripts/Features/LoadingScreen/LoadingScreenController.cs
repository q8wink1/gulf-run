using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.LoadingScreen
{
    /// <summary>
    /// Premium Loading Screen UI (Sprint 22.5) + Sprint 23.13 offline
    /// auto-advance: when Quick Play set <see cref="OfflineRaceEntryService"/>,
    /// dwell 2–3s then load Gameplay. Continue stub still loads PreRaceIntro
    /// for the premium map-vote path (non-offline).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LoadingScreenController : MonoBehaviour
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private RectTransform spinner;
        [SerializeField] private float spinnerDegreesPerSecond = 180f;
        [SerializeField] private float offlineAutoAdvanceSeconds = OfflineRaceEntryService.DefaultLoadingSeconds;

        private float _autoAdvanceRemaining = -1f;
        private bool _navigatedAway;

        private void Awake()
        {
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueClicked);
            }
        }

        private void Start()
        {
            if (!OfflineRaceEntryService.ConsumeLoadingAutoAdvance())
            {
                return;
            }

            float seconds = offlineAutoAdvanceSeconds;
            if (seconds < 2f)
            {
                seconds = 2f;
            }
            else if (seconds > 3f)
            {
                seconds = 3f;
            }

            _autoAdvanceRemaining = seconds;
        }

        private void Update()
        {
            if (spinner != null && spinnerDegreesPerSecond != 0f)
            {
                spinner.Rotate(0f, 0f, -spinnerDegreesPerSecond * Time.unscaledDeltaTime);
            }

            if (_navigatedAway || _autoAdvanceRemaining < 0f)
            {
                return;
            }

            _autoAdvanceRemaining -= Time.unscaledDeltaTime;
            if (_autoAdvanceRemaining <= 0f)
            {
                _autoAdvanceRemaining = -1f;
                GoToGameplay();
            }
        }

        private void OnDestroy()
        {
            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(OnContinueClicked);
            }
        }

        private void OnContinueClicked()
        {
            if (_navigatedAway)
            {
                return;
            }

            // Offline Quick Play: Continue also enters the race (skip remaining timer).
            if (OfflineRaceEntryService.IsActive)
            {
                GoToGameplay();
                return;
            }

            GoToPreRaceIntro();
        }

        private void GoToGameplay()
        {
            if (_navigatedAway)
            {
                return;
            }

            _navigatedAway = true;
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadGameplay();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.GameplaySceneName);
        }

        private static void GoToPreRaceIntro()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadPreRaceIntro();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.PreRaceIntroSceneName);
        }
    }
}

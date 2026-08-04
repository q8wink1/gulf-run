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
            Debug.Log("[LoadingScreen] Start — activeScene="
                + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                + " OfflineRaceEntryService.IsActive=" + OfflineRaceEntryService.IsActive
                + " PendingLoadingAutoAdvance=" + OfflineRaceEntryService.PendingLoadingAutoAdvance
                + " SceneManager.Instance=" + (SceneManager.Instance != null));

            bool shouldAutoAdvance = OfflineRaceEntryService.ConsumeLoadingAutoAdvance()
                                     || OfflineRaceEntryService.IsActive;
            if (!shouldAutoAdvance)
            {
                Debug.Log("[LoadingScreen] No offline auto-advance — waiting for Continue (PreRaceIntro path).");
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
            Debug.Log("[LoadingScreen] Offline auto-advance armed for " + seconds
                + "s → Gameplay (skip PreRaceIntro).");
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
                Debug.Log("[LoadingScreen] Offline timer finished — calling GoToGameplay()");
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
                Debug.Log("[LoadingScreen] Continue clicked (offline) → Gameplay");
                GoToGameplay();
                return;
            }

            Debug.Log("[LoadingScreen] Continue clicked → PreRaceIntro");
            GoToPreRaceIntro();
        }

        private void GoToGameplay()
        {
            if (_navigatedAway)
            {
                return;
            }

            _navigatedAway = true;
            Debug.Log("[LoadingScreen] Before LoadGameplay — SceneManager.Instance="
                + (SceneManager.Instance != null)
                + " target='" + SceneManager.GameplaySceneName + "'");

            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadGameplay();
                Debug.Log("[LoadingScreen] After SceneManager.Instance.LoadGameplay()");
                return;
            }

            Debug.Log("[LoadingScreen] SceneManager.Instance null — UnityEngine.SceneManagement.SceneManager.LoadScene('"
                + SceneManager.GameplaySceneName + "')");
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.GameplaySceneName);
            Debug.Log("[LoadingScreen] After direct LoadScene('" + SceneManager.GameplaySceneName
                + "') activeScene=" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        private void GoToPreRaceIntro()
        {
            if (_navigatedAway)
            {
                return;
            }

            _navigatedAway = true;
            Debug.Log("[LoadingScreen] Before LoadPreRaceIntro — SceneManager.Instance="
                + (SceneManager.Instance != null));

            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadPreRaceIntro();
                Debug.Log("[LoadingScreen] After SceneManager.Instance.LoadPreRaceIntro()");
                return;
            }

            Debug.Log("[LoadingScreen] SceneManager.Instance null — UnityEngine.SceneManagement.SceneManager.LoadScene('"
                + SceneManager.PreRaceIntroSceneName + "')");
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.PreRaceIntroSceneName);
            Debug.Log("[LoadingScreen] After direct LoadScene('" + SceneManager.PreRaceIntroSceneName + "')");
        }
    }
}

using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.QuickPlay
{
    /// <summary>
    /// Sprint 23.13 — Quick Play entry: skip public matchmaking / LobbyScreen,
    /// create a local one-player stub when SessionManager exists, then hand off
    /// to LoadingScreen (2–3s) → Gameplay. Lobby / matchmaking code remains for
    /// other entry points. Offline race flag is set even without Boot managers
    /// so Editor Play Mode from MainMenu/PlayMenu still reaches Gameplay.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class QuickPlayController : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Text statusText;
        [SerializeField] private RectTransform spinner;
        [SerializeField] private GameObject playersFoundLabel;
        [SerializeField] private GameObject joiningRoomLabel;
        [SerializeField] private GameObject creatingRoomLabel;
        [SerializeField] private GameObject waitingForPlayersLabel;

        private bool _navigatedAway;

        private void Awake()
        {
            if (backButton != null)
            {
                backButton.onClick.AddListener(CancelAndReturn);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.AddListener(CancelAndReturn);
            }
        }

        private void Start()
        {
            SetActive(playersFoundLabel, false);
            SetActive(joiningRoomLabel, false);
            SetActive(creatingRoomLabel, false);
            SetActive(waitingForPlayersLabel, false);

            // Arm offline LoadingScreen → Gameplay only when this scene is entered
            // via Play Menu Quick Play (or SessionManager match path).
            OfflineRaceEntryService.BeginPendingEntry();

            string displayName = LocalProfileProviderService.Current != null &&
                                 LocalProfileProviderService.Current.HasProfile
                ? LocalProfileProviderService.Current.LocalProfile.Nickname
                : "Player";

            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby != null)
            {
                try
                {
                    lobby.CreateLocalOfflinePrototype(displayName);
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning("[QuickPlay] CreateLocalOfflinePrototype failed (continuing offline): "
                        + ex.Message);
                }
            }

            SetStatus("Preparing offline race...");
            GoToLoadingScreen();
        }

        private void OnDestroy()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveListener(CancelAndReturn);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveListener(CancelAndReturn);
            }
        }

        private void Update()
        {
            if (spinner != null)
            {
                spinner.Rotate(0f, 0f, -180f * Time.deltaTime);
            }
        }

        private void SetStatus(string value)
        {
            if (statusText != null)
            {
                statusText.text = value;
            }
        }

        private static void SetActive(GameObject go, bool active)
        {
            if (go != null)
            {
                go.SetActive(active);
            }
        }

        private void GoToLoadingScreen()
        {
            if (_navigatedAway)
            {
                return;
            }

            _navigatedAway = true;
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadLoadingScreen();
                return;
            }

            // Last resort when Boot managers were never created (dev entered QuickPlay directly).
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.LoadingScreenSceneName);
        }

        private void CancelAndReturn()
        {
            if (_navigatedAway)
            {
                return;
            }

            _navigatedAway = true;
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            lobby?.CancelOrLeaveMatch();
            OfflineRaceEntryService.Clear();

            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadPlayMenu();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.PlayMenuSceneName);
        }
    }
}

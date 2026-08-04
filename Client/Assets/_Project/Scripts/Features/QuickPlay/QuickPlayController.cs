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
            Debug.Log("[QuickPlay] Start — activeScene="
                + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                + " SceneManager.Instance=" + (SceneManager.Instance != null)
                + " MatchLobbySummaryService.Current=" + (MatchLobbySummaryService.Current != null));

            SetActive(playersFoundLabel, false);
            SetActive(joiningRoomLabel, false);
            SetActive(creatingRoomLabel, false);
            SetActive(waitingForPlayersLabel, false);

            // Always arm offline LoadingScreen → Gameplay, even if Boot/SessionManager
            // are missing (common when entering Play Mode from MainMenu/PlayMenu).
            OfflineRaceEntryService.BeginPendingEntry();
            Debug.Log("[QuickPlay] OfflineRaceEntryService.BeginPendingEntry() — IsActive="
                + OfflineRaceEntryService.IsActive
                + " PendingLoadingAutoAdvance=" + OfflineRaceEntryService.PendingLoadingAutoAdvance);

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
                    Debug.Log("[QuickPlay] CreateLocalOfflinePrototype OK for '" + displayName + "'");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning("[QuickPlay] CreateLocalOfflinePrototype failed (continuing offline): "
                        + ex.Message);
                }
            }
            else
            {
                Debug.LogWarning("[QuickPlay] MatchLobbySummaryService.Current is null — "
                    + "skipping stub match; continuing offline LoadingScreen → Gameplay.");
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
            Debug.Log("[QuickPlay] Before LoadLoadingScreen — SceneManager.Instance="
                + (SceneManager.Instance != null));

            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadLoadingScreen();
                Debug.Log("[QuickPlay] After SceneManager.Instance.LoadLoadingScreen()");
                return;
            }

            Debug.Log("[QuickPlay] SceneManager.Instance null — UnityEngine.SceneManagement.SceneManager.LoadScene('"
                + SceneManager.LoadingScreenSceneName + "')");
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.LoadingScreenSceneName);
            Debug.Log("[QuickPlay] After direct LoadScene('" + SceneManager.LoadingScreenSceneName
                + "') activeScene=" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
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

            Debug.Log("[QuickPlay] CancelAndReturn → PlayMenu");
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadPlayMenu();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.PlayMenuSceneName);
        }
    }
}

using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.QuickPlay
{
    /// <summary>
    /// Sprint 23.13 — Quick Play entry: skip public matchmaking / LobbyScreen,
    /// create a local one-player stub, then hand off to LoadingScreen (2–3s)
    /// → Gameplay. Lobby / matchmaking code remains for other entry points.
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

            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby == null)
            {
                SetStatus("Matchmaking unavailable");
                return;
            }

            string displayName = LocalProfileProviderService.Current != null &&
                                 LocalProfileProviderService.Current.HasProfile
                ? LocalProfileProviderService.Current.LocalProfile.Nickname
                : "Player";

            lobby.CreateLocalOfflinePrototype(displayName);
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

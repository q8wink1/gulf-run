using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.QuickPlay
{
    /// <summary>
    /// Quick Play searching screen: starts mock public matchmaking on enter,
    /// mirrors SessionManager status text, Cancel/Back → Play Menu, and
    /// auto-loads Lobby once a match is formed.
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

        private bool _searchStarted;
        private bool _navigatedToLobby;

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
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby == null)
            {
                SetStatus("Matchmaking unavailable");
                return;
            }

            if (lobby.IsInMatch)
            {
                GoToLobby();
                return;
            }

            string displayName = LocalProfileProviderService.Current != null &&
                                 LocalProfileProviderService.Current.HasProfile
                ? LocalProfileProviderService.Current.LocalProfile.Nickname
                : "Player";

            lobby.StartQuickMatch(displayName);
            _searchStarted = true;
            RefreshStatus(lobby);
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

            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby == null)
            {
                return;
            }

            RefreshStatus(lobby);

            if (!_navigatedToLobby && lobby.IsInMatch && !lobby.IsMatchmaking)
            {
                // Brief beat so "Joining/Creating" copy is readable, then enter Lobby.
                if (_searchStarted)
                {
                    GoToLobby();
                }
            }
        }

        private void RefreshStatus(IMatchLobbySummaryProvider lobby)
        {
            string message = string.IsNullOrEmpty(lobby.MatchmakingStatusMessage)
                ? (lobby.IsInMatch ? "Entering Lobby..." : "Searching for available players...")
                : lobby.MatchmakingStatusMessage;
            SetStatus(message);

            string lower = message.ToLowerInvariant();
            SetActive(playersFoundLabel, lower.Contains("players found"));
            SetActive(joiningRoomLabel, lower.Contains("joining"));
            SetActive(creatingRoomLabel, lower.Contains("creating"));
            SetActive(waitingForPlayersLabel, lower.Contains("waiting"));
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

        private void GoToLobby()
        {
            _navigatedToLobby = true;
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadLobby();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.LobbySceneName);
        }

        private void CancelAndReturn()
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            lobby?.CancelOrLeaveMatch();

            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadPlayMenu();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.PlayMenuSceneName);
        }
    }
}

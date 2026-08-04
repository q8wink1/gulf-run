using GulfRun.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.QuickPlay
{
    /// <summary>
    /// Quick Play UI placeholder: demo status flow only (no matchmaking).
    /// Searching → Joining Room → Waiting For Players → Ready To Start.
    /// Back / Cancel always return to Play Menu.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class QuickPlayController : MonoBehaviour
    {
        private const float SecondsPerState = 2.0f;

        public enum MatchmakingUiState
        {
            Searching = 0,
            PlayersFound = 1,
            JoiningRoom = 2,
            CreatingRoom = 3,
            WaitingForPlayers = 4,
            ReadyToStart = 5
        }

        [SerializeField] private Button backButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Text statusText;
        [SerializeField] private RectTransform spinner;
        [SerializeField] private GameObject playersFoundLabel;
        [SerializeField] private GameObject joiningRoomLabel;
        [SerializeField] private GameObject creatingRoomLabel;
        [SerializeField] private GameObject waitingForPlayersLabel;

        private MatchmakingUiState _state = MatchmakingUiState.Searching;
        private float _elapsedInState;

        private void Awake()
        {
            if (backButton != null)
            {
                backButton.onClick.AddListener(ReturnToPlayMenu);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.AddListener(ReturnToPlayMenu);
            }

            ApplyState(_state);
        }

        private void OnDestroy()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveListener(ReturnToPlayMenu);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveListener(ReturnToPlayMenu);
            }
        }

        private void Update()
        {
            if (spinner != null)
            {
                spinner.Rotate(0f, 0f, -180f * Time.deltaTime);
            }

            if (_state == MatchmakingUiState.ReadyToStart)
            {
                return;
            }

            _elapsedInState += Time.deltaTime;
            if (_elapsedInState < SecondsPerState)
            {
                return;
            }

            _elapsedInState = 0f;
            ApplyState(NextState(_state));
        }

        private static MatchmakingUiState NextState(MatchmakingUiState current) => current switch
        {
            MatchmakingUiState.Searching => MatchmakingUiState.PlayersFound,
            MatchmakingUiState.PlayersFound => MatchmakingUiState.JoiningRoom,
            MatchmakingUiState.JoiningRoom => MatchmakingUiState.CreatingRoom,
            MatchmakingUiState.CreatingRoom => MatchmakingUiState.WaitingForPlayers,
            MatchmakingUiState.WaitingForPlayers => MatchmakingUiState.ReadyToStart,
            _ => MatchmakingUiState.ReadyToStart
        };

        private void ApplyState(MatchmakingUiState state)
        {
            _state = state;
            if (statusText != null)
            {
                statusText.text = StatusCopy(state);
            }

            SetActive(playersFoundLabel, state == MatchmakingUiState.PlayersFound);
            SetActive(joiningRoomLabel, state == MatchmakingUiState.JoiningRoom);
            SetActive(creatingRoomLabel, state == MatchmakingUiState.CreatingRoom);
            SetActive(waitingForPlayersLabel, state == MatchmakingUiState.WaitingForPlayers);
        }

        private static string StatusCopy(MatchmakingUiState state) => state switch
        {
            MatchmakingUiState.Searching => "Searching for available players...",
            MatchmakingUiState.PlayersFound => "Players Found",
            MatchmakingUiState.JoiningRoom => "Joining Room...",
            MatchmakingUiState.CreatingRoom => "Creating Room...",
            MatchmakingUiState.WaitingForPlayers => "Waiting For Players...",
            MatchmakingUiState.ReadyToStart => "Ready To Start",
            _ => "Searching for available players..."
        };

        private static void SetActive(GameObject go, bool active)
        {
            if (go != null)
            {
                go.SetActive(active);
            }
        }

        private static void ReturnToPlayMenu()
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

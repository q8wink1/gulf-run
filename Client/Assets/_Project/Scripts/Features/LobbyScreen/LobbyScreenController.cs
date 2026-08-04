using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.LobbyScreen
{
    /// <summary>
    /// Premium Lobby UI (Sprint 21.1–21.5). Back → Play Menu.
    /// Ready is a local visual demo only. Play Start Match (Sprint 22.1) loads
    /// MapVoting UI-only — no SessionManager, matchmaking, kick, or network sync.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LobbyScreenController : MonoBehaviour
    {
        private static readonly Color ReadyIdleBg = new Color(0.90f, 0.71f, 0.25f, 1f);
        private static readonly Color ReadyIdleLabel = new Color(0.20f, 0.14f, 0.02f, 1f);
        private static readonly Color ReadyPressedBg = new Color(0.40f, 0.85f, 0.45f, 1f);
        private static readonly Color ReadyPressedLabel = new Color(0.08f, 0.18f, 0.10f, 1f);
        private static readonly Color SlotReadyColor = new Color(0.40f, 0.85f, 0.45f, 1f);
        private static readonly Color SlotNotReadyColor = new Color(0.55f, 0.55f, 0.55f, 1f);

        private static readonly Color PlayWaitingBg = new Color(0.18f, 0.16f, 0.14f, 0.72f);
        private static readonly Color PlayWaitingLabel = new Color(0.62f, 0.60f, 0.56f, 0.85f);
        private static readonly Color PlayPreparedBg = new Color(0.90f, 0.71f, 0.25f, 1f);
        private static readonly Color PlayPreparedLabel = new Color(0.20f, 0.14f, 0.02f, 1f);

        [SerializeField] private Button backButton;
        [SerializeField] private Button readyButton;
        [SerializeField] private Image readyButtonImage;
        [SerializeField] private Text readyButtonLabel;
        [SerializeField] private Image localReadyStatus;
        [SerializeField] private Text localReadyLabel;
        [SerializeField] private Button playButton;
        [SerializeField] private Image playButtonImage;
        [SerializeField] private Text playButtonLabel;

        private bool _localReadyVisual;
        private bool _playPreparedVisual;

        private void Awake()
        {
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }

            if (readyButton != null)
            {
                readyButton.onClick.AddListener(OnReadyClicked);
            }

            if (playButton != null)
            {
                // Visual demo only — never starts a match.
                playButton.onClick.AddListener(OnPlayClicked);
            }

            // Default idle Ready chrome — do not overwrite slot mock data on load.
            _localReadyVisual = false;
            if (readyButtonImage != null)
            {
                readyButtonImage.color = ReadyIdleBg;
            }

            if (readyButtonLabel != null)
            {
                readyButtonLabel.text = "Ready";
                readyButtonLabel.color = ReadyIdleLabel;
            }

            // Sprint 22.1 temporary Host preview: Start Match → MapVoting (UI-only).
            ApplyPlayPreparedVisual(true);
        }

        private void OnDestroy()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveListener(OnBackClicked);
            }

            if (readyButton != null)
            {
                readyButton.onClick.RemoveListener(OnReadyClicked);
            }

            if (playButton != null)
            {
                playButton.onClick.RemoveListener(OnPlayClicked);
            }
        }

        private void OnReadyClicked()
        {
            // Visual demo only — does not call SessionManager or sync ready state.
            ApplyReadyVisual(!_localReadyVisual);
        }

        private void OnPlayClicked()
        {
            // Sprint 22.1 temporary nav — LoadMapVoting only when Start Match is prepared.
            // No SessionManager.RequestHostStart, ready checks, or matchmaking.
            if (playButton == null || !playButton.interactable)
            {
                return;
            }

            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadMapVoting();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.MapVotingSceneName);
        }

        private void ApplyReadyVisual(bool ready)
        {
            _localReadyVisual = ready;

            if (readyButtonImage != null)
            {
                readyButtonImage.color = ready ? ReadyPressedBg : ReadyIdleBg;
            }

            if (readyButtonLabel != null)
            {
                readyButtonLabel.text = ready ? "Ready ✓" : "Ready";
                readyButtonLabel.color = ready ? ReadyPressedLabel : ReadyIdleLabel;
            }

            if (localReadyStatus != null)
            {
                localReadyStatus.color = ready ? SlotReadyColor : SlotNotReadyColor;
            }

            if (localReadyLabel != null)
            {
                localReadyLabel.text = ready ? "Ready" : "Not Ready";
                localReadyLabel.color = ready ? SlotReadyColor : SlotNotReadyColor;
            }
        }

        /// <summary>
        /// Local visual placeholder for Play prepared vs waiting. No match start.
        /// </summary>
        public void ApplyPlayPreparedVisual(bool prepared)
        {
            _playPreparedVisual = prepared;

            if (playButtonImage != null)
            {
                playButtonImage.color = prepared ? PlayPreparedBg : PlayWaitingBg;
            }

            if (playButtonLabel != null)
            {
                playButtonLabel.text = prepared ? "Start Match" : "Waiting for Players...";
                playButtonLabel.color = prepared ? PlayPreparedLabel : PlayWaitingLabel;
                playButtonLabel.fontSize = prepared ? 30 : 26;
            }

            if (playButton != null)
            {
                playButton.interactable = prepared;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Demo: Toggle Start Match Visual")]
        private void DemoTogglePlayPreparedVisual()
        {
            ApplyPlayPreparedVisual(!_playPreparedVisual);
        }
#endif

        private static void OnBackClicked()
        {
            // Leave any Quick Play match started before this UI-only screen loaded,
            // so returning to Play Menu does not immediately re-enter LobbyScreen.
            MatchLobbySummaryService.Current?.CancelOrLeaveMatch();

            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadPlayMenu();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.PlayMenuSceneName);
        }
    }
}

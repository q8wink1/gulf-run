using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.LobbyScreen
{
    /// <summary>
    /// Premium Lobby UI (Sprint 21.1–21.3). Back → Play Menu.
    /// Ready button is a local visual toggle only (text/color + optional local
    /// slot chrome) — no SessionManager, matchmaking, or network sync.
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

        [SerializeField] private Button backButton;
        [SerializeField] private Button readyButton;
        [SerializeField] private Image readyButtonImage;
        [SerializeField] private Text readyButtonLabel;
        [SerializeField] private Image localReadyStatus;
        [SerializeField] private Text localReadyLabel;

        private bool _localReadyVisual;

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

            // Default idle button chrome only — do not overwrite slot mock data on load.
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
        }

        private void OnReadyClicked()
        {
            // Visual demo only — does not call SessionManager or sync ready state.
            ApplyReadyVisual(!_localReadyVisual);
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

using GulfRun.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.MapVotingScreen
{
    /// <summary>
    /// Premium Map Voting UI (Sprint 22.2). Back → LobbyScreen.
    /// Vote buttons are local visual highlight only — no countdown, vote
    /// counting, networking, SessionManager, or matchmaking.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MapVotingScreenController : MonoBehaviour
    {
        private static readonly Color CardBorderIdle = new Color(0.90f, 0.71f, 0.25f, 0.55f);
        private static readonly Color CardBorderSelected = new Color(1f, 0.88f, 0.35f, 1f);
        private static readonly Color VoteIdleBg = new Color(0.90f, 0.71f, 0.25f, 1f);
        private static readonly Color VoteIdleLabel = new Color(0.20f, 0.14f, 0.02f, 1f);
        private static readonly Color VoteSelectedBg = new Color(0.40f, 0.85f, 0.45f, 1f);
        private static readonly Color VoteSelectedLabel = new Color(0.08f, 0.18f, 0.10f, 1f);

        [SerializeField] private Button backButton;
        [SerializeField] private Button[] voteButtons;
        [SerializeField] private Image[] cardBorders;
        [SerializeField] private Image[] voteButtonImages;
        [SerializeField] private Text[] voteButtonLabels;
        [SerializeField] private GameObject[] selectedCheckmarks;
        [SerializeField] private MapCardVisual[] cardVisuals;

        private int _selectedIndex = -1;

        private void Awake()
        {
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }

            if (voteButtons == null)
            {
                return;
            }

            for (int i = 0; i < voteButtons.Length; i++)
            {
                int index = i;
                Button voteButton = voteButtons[i];
                if (voteButton == null)
                {
                    continue;
                }

                voteButton.onClick.AddListener(() => OnVoteClicked(index));
            }

            ApplySelectionVisual(-1);
        }

        private void OnDestroy()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveListener(OnBackClicked);
            }

            if (voteButtons == null)
            {
                return;
            }

            for (int i = 0; i < voteButtons.Length; i++)
            {
                if (voteButtons[i] != null)
                {
                    voteButtons[i].onClick.RemoveAllListeners();
                }
            }
        }

        private void OnVoteClicked(int index)
        {
            // Visual demo only — does not cast network votes or change counts.
            ApplySelectionVisual(_selectedIndex == index ? -1 : index);
        }

        private void ApplySelectionVisual(int index)
        {
            _selectedIndex = index;

            int cardCount = cardBorders != null ? cardBorders.Length : 0;
            for (int i = 0; i < cardCount; i++)
            {
                bool selected = i == _selectedIndex;
                if (cardBorders[i] != null)
                {
                    cardBorders[i].color = selected ? CardBorderSelected : CardBorderIdle;
                }

                if (voteButtonImages != null && i < voteButtonImages.Length && voteButtonImages[i] != null)
                {
                    voteButtonImages[i].color = selected ? VoteSelectedBg : VoteIdleBg;
                }

                if (voteButtonLabels != null && i < voteButtonLabels.Length && voteButtonLabels[i] != null)
                {
                    voteButtonLabels[i].text = selected ? "Voted" : "Vote";
                    voteButtonLabels[i].color = selected ? VoteSelectedLabel : VoteIdleLabel;
                }

                if (selectedCheckmarks != null && i < selectedCheckmarks.Length && selectedCheckmarks[i] != null)
                {
                    selectedCheckmarks[i].SetActive(selected);
                }

                if (cardVisuals != null && i < cardVisuals.Length && cardVisuals[i] != null)
                {
                    cardVisuals[i].SetSelected(selected);
                }
            }
        }

        private static void OnBackClicked()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadLobbyScreen();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.LobbyScreenSceneName);
        }
    }
}

using System.Collections.Generic;
using GulfRun.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.InviteFriends
{
    /// <summary>
    /// Invite Friends UI placeholder: fake friends list, player-ID send feedback,
    /// and WhatsApp copy/share stubs. No networking or friend backend.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class InviteFriendsController : MonoBehaviour
    {
        private const string PlaceholderInviteUrl = "https://gulfrun.game/invite/DEMO-ROOM-CODE";
        private const float StatusClearSeconds = 2.5f;

        [SerializeField] private Button backButton;
        [SerializeField] private InputField playerIdInput;
        [SerializeField] private Button sendInvitationButton;
        [SerializeField] private Button copyLinkButton;
        [SerializeField] private Button shareWhatsAppButton;
        [SerializeField] private Text statusText;
        [SerializeField] private List<Button> friendRowButtons = new List<Button>();
        [SerializeField] private List<Image> friendRowHighlights = new List<Image>();

        private readonly HashSet<int> _selectedFriendIndexes = new HashSet<int>();
        private float _statusClearAt = -1f;

        private void Awake()
        {
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }

            if (sendInvitationButton != null)
            {
                sendInvitationButton.onClick.AddListener(OnSendInvitationClicked);
            }

            if (copyLinkButton != null)
            {
                copyLinkButton.onClick.AddListener(OnCopyLinkClicked);
            }

            if (shareWhatsAppButton != null)
            {
                shareWhatsAppButton.onClick.AddListener(OnShareWhatsAppClicked);
            }

            for (int i = 0; i < friendRowButtons.Count; i++)
            {
                int index = i;
                Button row = friendRowButtons[i];
                if (row != null)
                {
                    row.onClick.AddListener(() => ToggleFriendSelection(index));
                }
            }

            RefreshFriendHighlights();
            SetStatus(string.Empty);
        }

        private void OnDestroy()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveListener(OnBackClicked);
            }

            if (sendInvitationButton != null)
            {
                sendInvitationButton.onClick.RemoveListener(OnSendInvitationClicked);
            }

            if (copyLinkButton != null)
            {
                copyLinkButton.onClick.RemoveListener(OnCopyLinkClicked);
            }

            if (shareWhatsAppButton != null)
            {
                shareWhatsAppButton.onClick.RemoveListener(OnShareWhatsAppClicked);
            }

            for (int i = 0; i < friendRowButtons.Count; i++)
            {
                // Listeners capture index; clearing via RemoveAllListeners is safest for placeholders.
                if (friendRowButtons[i] != null)
                {
                    friendRowButtons[i].onClick.RemoveAllListeners();
                }
            }
        }

        private void Update()
        {
            if (_statusClearAt > 0f && Time.unscaledTime >= _statusClearAt)
            {
                _statusClearAt = -1f;
                SetStatus(string.Empty);
            }
        }

        private void ToggleFriendSelection(int index)
        {
            if (!_selectedFriendIndexes.Add(index))
            {
                _selectedFriendIndexes.Remove(index);
            }

            RefreshFriendHighlights();
            SetStatus(_selectedFriendIndexes.Count == 0
                ? string.Empty
                : _selectedFriendIndexes.Count + " friend(s) selected (UI only)");
        }

        private void RefreshFriendHighlights()
        {
            for (int i = 0; i < friendRowHighlights.Count; i++)
            {
                Image highlight = friendRowHighlights[i];
                if (highlight == null)
                {
                    continue;
                }

                bool selected = _selectedFriendIndexes.Contains(i);
                Color c = highlight.color;
                c.a = selected ? 0.55f : 0.12f;
                highlight.color = c;
            }
        }

        private void OnSendInvitationClicked()
        {
            string code = playerIdInput != null ? playerIdInput.text : string.Empty;
            if (string.IsNullOrWhiteSpace(code))
            {
                ShowTransientStatus("Enter a Player ID or Invite Code");
                return;
            }

            // TODO: real invite transport — UI feedback only for now.
            ShowTransientStatus("Invitation sent (placeholder)");
        }

        private void OnCopyLinkClicked()
        {
            GUIUtility.systemCopyBuffer = PlaceholderInviteUrl;
            ShowTransientStatus("Invite link copied");
        }

        private void OnShareWhatsAppClicked()
        {
            // TODO: real WhatsApp share — stub only.
            ShowTransientStatus("WhatsApp share (placeholder)");
        }

        private void ShowTransientStatus(string message)
        {
            SetStatus(message);
            _statusClearAt = Time.unscaledTime + StatusClearSeconds;
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message ?? string.Empty;
            }
        }

        private static void OnBackClicked()
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

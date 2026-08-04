using GulfRun.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.PlayMenu
{
    /// <summary>
    /// Play Menu hub: Back → Main Menu; Quick Play / Invite Friends cards open
    /// their dedicated UI placeholder scenes. No networking.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayMenuController : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button quickPlayButton;
        [SerializeField] private Button inviteFriendsButton;

        private void Awake()
        {
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }

            if (quickPlayButton != null)
            {
                quickPlayButton.onClick.AddListener(OnQuickPlayClicked);
            }

            if (inviteFriendsButton != null)
            {
                inviteFriendsButton.onClick.AddListener(OnInviteFriendsClicked);
            }
        }

        private void OnDestroy()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveListener(OnBackClicked);
            }

            if (quickPlayButton != null)
            {
                quickPlayButton.onClick.RemoveListener(OnQuickPlayClicked);
            }

            if (inviteFriendsButton != null)
            {
                inviteFriendsButton.onClick.RemoveListener(OnInviteFriendsClicked);
            }
        }

        private static void OnBackClicked()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadMainMenu();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.MainMenuSceneName);
        }

        private static void OnQuickPlayClicked()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadQuickPlay();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.QuickPlaySceneName);
        }

        private static void OnInviteFriendsClicked()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadInviteFriends();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.InviteFriendsSceneName);
        }
    }
}

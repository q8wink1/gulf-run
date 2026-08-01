using GulfRun.Core.Services;
using GulfRun.Features.MainMenu.UI;
using UnityEngine;

namespace GulfRun.Features.MainMenu.Widgets
{
    /// <summary>
    /// Sprint 13 "SOCIAL": Friends Online, Clan Online, Invite button, Room
    /// Code. Reads exclusively through <see cref="IFriendsSummaryProvider"/>
    /// and <see cref="IMatchLobbySummaryProvider"/>. "Clan Online" always
    /// reads 0/0 — no Clan Feature exists anywhere in the project yet (see
    /// <c>LeftMenuView</c> remarks and Sprint 13 report Remaining TODOs).
    /// </summary>
    public sealed class SocialPanelView : MonoBehaviour
    {
        private const float PanelWidth = 260f;
        private const float PanelHeight = 100f;

        private ButtonPressAnimator _inviteAnim;

        private void OnGUI()
        {
            float x = Screen.width - PanelWidth - 16f;
            float y = 76f + 6 * 56f + 40f;

            MainMenuTheme.DrawPanel(new Rect(x, y, PanelWidth, PanelHeight));

            IFriendsSummaryProvider friends = FriendsSummaryService.Current;
            int onlineFriends = friends != null ? friends.OnlineFriendsCount : 0;
            int totalFriends = friends != null ? friends.TotalFriendsCount : 0;

            GUI.Label(new Rect(x + 10f, y + 6f, PanelWidth - 20f, 20f), "Friends Online: " + onlineFriends + "/" + totalFriends, MainMenuTheme.Label);
            GUI.Label(new Rect(x + 10f, y + 26f, PanelWidth - 20f, 20f), "Clan Online: 0/0", MainMenuTheme.MutedLabel);

            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            string roomCodeText = lobby != null && lobby.IsHost && !lobby.LocalRoomCode.IsNone ? lobby.LocalRoomCode.ToString() : "—";
            GUI.Label(new Rect(x + 10f, y + 46f, PanelWidth - 20f, 20f), "Room Code: " + roomCodeText, MainMenuTheme.Label);

            Rect inviteRect = _inviteAnim.Apply(new Rect(x + 10f, y + 68f, PanelWidth - 20f, 24f), 2f);
            if (GUI.Button(inviteRect, "Invite Friends", MainMenuTheme.PanelButton))
            {
                _inviteAnim.NotifyPressed();
                // Best-effort only — no push-notification/deep-link channel exists for an offline friend yet (see Sprint 9 report Remaining TODOs, still unresolved); Friends screen's own "Invite" button per-row already covers the honest local confirmation flow.
                MenuScreenRouter.TryOpen(MenuScreen.Friends);
            }
        }
    }
}

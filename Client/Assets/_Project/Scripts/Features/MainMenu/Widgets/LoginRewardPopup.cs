using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.MainMenu.UI;
using UnityEngine;

namespace GulfRun.Features.MainMenu.Widgets
{
    /// <summary>
    /// Sprint 13 "LOGIN REWARD": if a reward is available, shows an
    /// animated popup with a Claim button — reads/writes exclusively
    /// through <see cref="ILoginRewardStatusProvider"/> so
    /// Features.MainMenu never references Features.Progression directly.
    /// Dismissible without claiming (closing just re-shows it next OnGUI
    /// pass until actually claimed, matching every other "Coming Soon"/
    /// info popup's disposable style in this assembly) so a player is
    /// never blocked from the rest of the lobby.
    /// </summary>
    public sealed class LoginRewardPopup : MonoBehaviour
    {
        private bool _dismissedThisSession;
        private ButtonPressAnimator _claimAnim;
        private GUIStyle _centeredTitleStyle;
        private GUIStyle _centeredLabelStyle;

        private void OnGUI()
        {
            ILoginRewardStatusProvider provider = LoginRewardStatusService.Current;
            if (provider == null || _dismissedThisSession || provider.HasClaimedToday())
            {
                return;
            }

            EnsureStyles();

            const float width = 360f;
            const float height = 220f;
            float x = (Screen.width - width) * 0.5f;
            float y = (Screen.height - height) * 0.5f;

            MainMenuTheme.DrawPanel(new Rect(x, y, width, height));

            if (GUI.Button(new Rect(x + width - 34f, y + 8f, 24f, 24f), "X"))
            {
                _dismissedThisSession = true;
                return;
            }

            GUI.Label(new Rect(x, y + 14f, width, 26f), "DAILY LOGIN REWARD", _centeredTitleStyle);

            double elapsed = Time.timeAsDouble;
            float bob = CelebrationAnimation.EvaluateOffset(elapsed, 6f, 0.6f);

            const float chestSize = 84f;
            Color previous = GUI.color;
            GUI.color = MainMenuTheme.Gold;
            GUI.Box(new Rect(x + width * 0.5f - chestSize * 0.5f, y + 50f + bob, chestSize, chestSize), string.Empty);
            GUI.color = previous;

            string streakLine = "Day " + provider.Status.CurrentStreakDay + " Streak" +
                (!string.IsNullOrEmpty(provider.ActiveSpecialEventLabel) ? "  (" + provider.ActiveSpecialEventLabel + ")" : string.Empty);
            GUI.Label(new Rect(x, y + 148f, width, 20f), streakLine, _centeredLabelStyle);

            Rect claimRect = _claimAnim.Apply(new Rect(x + width * 0.5f - 90f, y + 172f, 180f, 34f), 2f);
            if (GUI.Button(claimRect, "Claim Reward", MainMenuTheme.GoldButton))
            {
                _claimAnim.NotifyPressed();
                if (provider.TryClaimDailyLogin())
                {
                    _dismissedThisSession = true;
                }
            }
        }

        private void EnsureStyles()
        {
            if (_centeredTitleStyle != null)
            {
                return;
            }

            _centeredTitleStyle = new GUIStyle(MainMenuTheme.Title) { alignment = TextAnchor.MiddleCenter };
            _centeredLabelStyle = new GUIStyle(MainMenuTheme.Label) { alignment = TextAnchor.MiddleCenter };
        }
    }
}

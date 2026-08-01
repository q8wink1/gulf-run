using GulfRun.Core.Branding;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.MainMenu.UI;
using GulfRun.Features.MainMenu.Widgets;
using UnityEngine;

namespace GulfRun.Features.MainMenu.TopBar
{
    /// <summary>
    /// Sprint 13 "TOP BAR": Player Name, Level, League, World Rank,
    /// Country Rank, Coins, Gems, Settings, Notifications — one bar
    /// spanning the top of the screen. Reads exclusively through
    /// <see cref="ILocalProfileProvider"/>/<see cref="INotificationSummaryProvider"/>
    /// and opens Settings/Notifications through <see cref="MenuScreenRouter"/>,
    /// so Features.MainMenu never references Features.Online/Progression
    /// directly — the same composition-root seam shape every other Sprint
    /// 13 view in this assembly uses.
    /// Sprint 14 "BRANDING: Use this official logo everywhere ... Main
    /// Lobby": a small <see cref="GulfRunBrandMark"/> badge sits to the
    /// left of the player identity — the one spot in the bar with no
    /// existing content to collide with at any screen width.
    /// </summary>
    public sealed class TopBarView : MonoBehaviour
    {
        private const float BarHeight = 56f;
        private const float BrandBadgeSize = 34f;

        private ButtonPressAnimator _settingsAnim;
        private ButtonPressAnimator _notificationsAnim;

        private void OnGUI()
        {
            MainMenuTheme.DrawPanel(new Rect(0f, 0f, Screen.width, BarHeight));
            DrawBrandBadge();

            ILocalProfileProvider profileProvider = LocalProfileProviderService.Current;
            if (profileProvider == null || !profileProvider.HasProfile)
            {
                GUI.Label(new Rect(16f + BrandBadgeSize + 8f, 0f, 400f, BarHeight), "Loading profile...", MainMenuTheme.MutedLabel);
                DrawTopRightButtons();
                return;
            }

            PlayerProfileSummary summary = profileProvider.LocalProfile;
            DrawIdentityBlock(summary);
            DrawRankBlock(summary);
            DrawCurrencyBlock(summary);
            DrawTopRightButtons();
        }

        private static void DrawBrandBadge()
        {
            Rect rect = new Rect(12f, (BarHeight - BrandBadgeSize) * 0.5f, BrandBadgeSize, BrandBadgeSize);
            GulfRunBrandMark.Draw(rect);
        }

        private static void DrawIdentityBlock(PlayerProfileSummary summary)
        {
            float x = 16f + BrandBadgeSize + 8f;
            GUI.Label(new Rect(x, 4f, 260f, 24f), summary.Nickname, MainMenuTheme.Title);

            string levelLine = "Lv." + summary.Level + "  (" + summary.CurrentXp + "/" + summary.XpRequiredForNextLevel + " XP)";
            GUI.Label(new Rect(x, 28f, 260f, 22f), levelLine, MainMenuTheme.MutedLabel);
        }

        private static void DrawRankBlock(PlayerProfileSummary summary)
        {
            const float x = 300f;
            string leagueLine = summary.Season.CurrentLeague + " League";
            GUI.Label(new Rect(x, 4f, 220f, 22f), leagueLine, MainMenuTheme.Label);

            string ranksLine = "World #" + FormatRank(summary.WorldRank) + "   Country #" + FormatRank(summary.CountryRank);
            GUI.Label(new Rect(x, 28f, 320f, 22f), ranksLine, MainMenuTheme.MutedLabel);
        }

        private static string FormatRank(int rank) => rank > 0 ? rank.ToString() : "—";

        private void DrawCurrencyBlock(PlayerProfileSummary summary)
        {
            float x = Screen.width - 470f;

            DrawCurrencyChip(x, "Coins", summary.Coins, new Color(0.72f, 0.55f, 0.28f, 1f));
            DrawCurrencyChip(x + 150f, "Gems", summary.Gems, new Color(0.35f, 0.75f, 0.85f, 1f));
        }

        private static void DrawCurrencyChip(float x, string label, int amount, Color iconColor)
        {
            const float chipWidth = 140f;
            const float chipHeight = 34f;
            float y = (BarHeight - chipHeight) * 0.5f;

            MainMenuTheme.DrawPanel(new Rect(x, y, chipWidth, chipHeight));

            Color previous = GUI.color;
            GUI.color = iconColor;
            // Old Money Bag / Arabian Gem icon placeholder — a colored diamond stand-in (see Sprint 13 report Remaining TODOs re: real icon sprites).
            GUI.Box(new Rect(x + 8f, y + 7f, 20f, 20f), string.Empty);
            GUI.color = previous;

            GUI.Label(new Rect(x + 34f, y, chipWidth - 40f, chipHeight), amount.ToString(), MainMenuTheme.Header);
        }

        private void DrawTopRightButtons()
        {
            const float buttonSize = 40f;
            float y = (BarHeight - buttonSize) * 0.5f;
            float notificationsX = Screen.width - buttonSize - 10f;
            float settingsX = notificationsX - buttonSize - 8f;

            int unread = NotificationSummaryService.Current != null ? NotificationSummaryService.Current.UnreadCount : 0;
            string bellLabel = unread > 0 ? "🔔" + unread : "🔔";

            Rect settingsRect = _settingsAnim.Apply(new Rect(settingsX, y, buttonSize, buttonSize), 3f);
            if (GUI.Button(settingsRect, "⚙", MainMenuTheme.PanelButton))
            {
                _settingsAnim.NotifyPressed();
                SettingsView.Instance?.Open();
            }

            Rect notificationsRect = _notificationsAnim.Apply(new Rect(notificationsX, y, buttonSize, buttonSize), 3f);
            if (GUI.Button(notificationsRect, bellLabel, MainMenuTheme.PanelButton))
            {
                _notificationsAnim.NotifyPressed();
                MenuScreenRouter.TryOpen(MenuScreen.Notifications);
            }
        }
    }
}

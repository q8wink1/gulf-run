using GulfRun.Core.Services;
using GulfRun.Features.MainMenu.UI;
using UnityEngine;

namespace GulfRun.Features.MainMenu.SideMenu
{
    /// <summary>
    /// Sprint 13 "LEFT MENU": Friends, Clan, Leaderboard, Missions, Battle
    /// Pass, Mail. Every button that has a real screen behind it
    /// (Friends/Leaderboard/Missions/Battle Pass) routes through
    /// <see cref="MenuScreenRouter"/>; Clan and Mail have no backend
    /// system anywhere in the project yet (no Clan/Mail Feature exists —
    /// see Sprint 13 report Remaining TODOs), so their buttons open a
    /// small honest "Coming Soon" panel owned directly by this Feature
    /// instead of silently doing nothing.
    /// </summary>
    public sealed class LeftMenuView : MonoBehaviour
    {
        private const float ButtonWidth = 150f;
        private const float ButtonHeight = 46f;
        private const float Spacing = 10f;

        private ButtonPressAnimator[] _anims = new ButtonPressAnimator[6];
        private string _comingSoonLabel = string.Empty;
        private double _comingSoonUntilSeconds;

        private void OnGUI()
        {
            float x = 16f;
            float y = 76f;

            DrawMenuButton(0, x, ref y, "Friends", () => MenuScreenRouter.TryOpen(MenuScreen.Friends));
            DrawMenuButton(1, x, ref y, "Clan", () => ShowComingSoon("Clan System"));
            DrawMenuButton(2, x, ref y, "Leaderboard", () => MenuScreenRouter.TryOpen(MenuScreen.Leaderboard));
            DrawMenuButton(3, x, ref y, "Missions", () => MenuScreenRouter.TryOpen(MenuScreen.Missions));
            DrawMenuButton(4, x, ref y, "Battle Pass", () => MenuScreenRouter.TryOpen(MenuScreen.BattlePass));
            DrawMenuButton(5, x, ref y, "Mail", () => ShowComingSoon("Mail"));

            DrawComingSoonToast(x);
        }

        private void DrawMenuButton(int index, float x, ref float y, string label, System.Action onClick)
        {
            Rect rect = _anims[index].Apply(new Rect(x, y, ButtonWidth, ButtonHeight), 3f);
            if (GUI.Button(rect, label, MainMenuTheme.PanelButton))
            {
                _anims[index].NotifyPressed();
                onClick();
            }

            y += ButtonHeight + Spacing;
        }

        private void ShowComingSoon(string systemName)
        {
            _comingSoonLabel = systemName + " — Coming Soon!";
            _comingSoonUntilSeconds = Time.timeAsDouble + 2.5d;
        }

        private void DrawComingSoonToast(float x)
        {
            if (string.IsNullOrEmpty(_comingSoonLabel) || Time.timeAsDouble >= _comingSoonUntilSeconds)
            {
                return;
            }

            MainMenuTheme.DrawPanel(new Rect(x, 76f + 6 * (ButtonHeight + Spacing), 220f, 32f));
            GUI.Label(new Rect(x + 10f, 76f + 6 * (ButtonHeight + Spacing) + 6f, 200f, 20f), _comingSoonLabel, MainMenuTheme.MutedLabel);
        }
    }
}

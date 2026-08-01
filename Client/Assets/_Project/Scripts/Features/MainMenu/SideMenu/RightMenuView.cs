using GulfRun.Core.Services;
using GulfRun.Features.MainMenu.UI;
using UnityEngine;

namespace GulfRun.Features.MainMenu.SideMenu
{
    /// <summary>
    /// Sprint 13 "RIGHT MENU": Store, Characters, Customize, Inventory,
    /// Events, Championships — every button routes through
    /// <see cref="MenuScreenRouter"/>. "Customize" opens the exact same
    /// screen as "Characters" (<c>CharacterMenuView</c> already covers
    /// outfit selection too — no separate Customize screen exists, see
    /// that view's remarks) rather than a second near-duplicate panel.
    /// </summary>
    public sealed class RightMenuView : MonoBehaviour
    {
        private const float ButtonWidth = 150f;
        private const float ButtonHeight = 46f;
        private const float Spacing = 10f;

        private ButtonPressAnimator[] _anims = new ButtonPressAnimator[6];

        private void OnGUI()
        {
            float x = Screen.width - ButtonWidth - 16f;
            float y = 76f;

            DrawMenuButton(0, x, ref y, "Store", () => MenuScreenRouter.TryOpen(MenuScreen.Store));
            DrawMenuButton(1, x, ref y, "Characters", () => MenuScreenRouter.TryOpen(MenuScreen.Characters));
            DrawMenuButton(2, x, ref y, "Customize", () => MenuScreenRouter.TryOpen(MenuScreen.Characters));
            DrawMenuButton(3, x, ref y, "Inventory", () => MenuScreenRouter.TryOpen(MenuScreen.Inventory));
            DrawMenuButton(4, x, ref y, "Events", () => MenuScreenRouter.TryOpen(MenuScreen.Events));
            DrawMenuButton(5, x, ref y, "Championships", () => MenuScreenRouter.TryOpen(MenuScreen.Championships));
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
    }
}

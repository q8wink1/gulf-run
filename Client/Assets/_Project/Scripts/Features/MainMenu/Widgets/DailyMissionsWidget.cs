using System.Collections.Generic;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.MainMenu.UI;
using UnityEngine;

namespace GulfRun.Features.MainMenu.Widgets
{
    /// <summary>
    /// Sprint 13 "DAILY MISSIONS": a small widget showing the 3 active
    /// missions with progress and a claim button — reads exclusively
    /// through <see cref="IDailyMissionsPreviewProvider"/> so
    /// Features.MainMenu never references Features.Progression directly.
    /// </summary>
    public sealed class DailyMissionsWidget : MonoBehaviour
    {
        private const float WidgetWidth = 260f;
        private const float RowHeight = 40f;

        private ButtonPressAnimator[] _claimAnims = new ButtonPressAnimator[3];

        private void OnGUI()
        {
            IDailyMissionsPreviewProvider provider = DailyMissionsPreviewService.Current;
            if (provider == null)
            {
                return;
            }

            IReadOnlyList<ActiveMission> missions = provider.ActiveMissions;
            float height = 32f + missions.Count * RowHeight + 8f;
            float x = 16f;
            float y = 76f + 6 * 56f + 40f;

            MainMenuTheme.DrawPanel(new Rect(x, y, WidgetWidth, height));
            GUI.Label(new Rect(x + 10f, y + 4f, WidgetWidth - 20f, 22f), "DAILY MISSIONS", MainMenuTheme.Header);
            MainMenuTheme.DrawGoldAccentLine(x + 10f, y + 26f, WidgetWidth - 20f);

            float rowY = y + 32f;
            for (int i = 0; i < missions.Count && i < 3; i++)
            {
                DrawMissionRow(provider, missions[i], i, x + 10f, rowY, WidgetWidth - 20f);
                rowY += RowHeight;
            }
        }

        private void DrawMissionRow(IDailyMissionsPreviewProvider provider, ActiveMission mission, int slotIndex, float x, float y, float width)
        {
            GUI.Label(new Rect(x, y, width - 70f, 18f), mission.DisplayName, MainMenuTheme.Label);

            float progress01 = mission.TargetAmount > 0 ? Mathf.Clamp01((float)mission.CurrentAmount / mission.TargetAmount) : 0f;
            DrawProgressBar(x, y + 18f, width - 70f, 8f, progress01);

            if (mission.IsClaimed)
            {
                GUI.Label(new Rect(x + width - 64f, y, 64f, 26f), "Done", MainMenuTheme.MutedLabel);
                return;
            }

            if (!mission.IsCompleted)
            {
                return;
            }

            Rect buttonRect = _claimAnims[slotIndex].Apply(new Rect(x + width - 64f, y, 64f, 26f), 2f);
            if (GUI.Button(buttonRect, "Claim", MainMenuTheme.PanelButton))
            {
                _claimAnims[slotIndex].NotifyPressed();
                provider.TryClaimMission(slotIndex);
            }
        }

        private static void DrawProgressBar(float x, float y, float width, float height, float progress01)
        {
            Color previous = GUI.color;
            GUI.color = MainMenuTheme.SandDark;
            GUI.Box(new Rect(x, y, width, height), string.Empty);

            GUI.color = MainMenuTheme.Gold;
            GUI.Box(new Rect(x, y, width * progress01, height), string.Empty);
            GUI.color = previous;
        }
    }
}

using System;
using GulfRun.Core;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Progression.Missions
{
    /// <summary>
    /// The Daily Missions screen — brief "DAILY MISSIONS" +
    /// "REWARD ANIMATIONS: ... Reward popup" (a simple inline claim
    /// confirmation stands in for the popup/animation/SFX until real art
    /// and audio exist, same "no final art yet" status every prior
    /// sprint's placeholder UI already carries). A <see cref="SceneSingleton{T}"/>
    /// like every other Gameplay-scene screen. <c>x: 1410</c> is
    /// Gameplay.unity's next free toggle-button slot after Sprint 10's
    /// InventoryView at <c>x: 1230</c>.
    /// </summary>
    public sealed class MissionsView : SceneSingleton<MissionsView>
    {
        private bool _open;
        private string _lastFeedback = string.Empty;
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _rowStyle;
        private GUIStyle _feedbackStyle;

        private void OnGUI()
        {
            EnsureStyles();

            if (GUI.Button(new Rect(1410, 10, 140, 34), _open ? "Close Missions" : "Missions"))
            {
                _open = !_open;
            }

            if (!_open)
            {
                return;
            }

            DrawPanel();
        }

        private void DrawPanel()
        {
            const float panelWidth = 460f;
            const float panelHeight = 320f;
            float x = 1410f;
            float y = 50f;

            GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty);
            GUI.Label(new Rect(x + 14f, y + 8f, panelWidth - 28f, 26f), "DAILY MISSIONS", _titleStyle);

            MissionManager manager = MissionManager.Instance;
            if (manager == null)
            {
                return;
            }

            var missions = manager.ActiveMissions;
            double remainingReset = manager.MissionsResetAtSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            GUI.Label(new Rect(x + 14f, y + 34f, panelWidth - 28f, 20f), "Resets in: " + FormatDuration(remainingReset), _labelStyle);

            float rowY = y + 58f;
            if (!string.IsNullOrEmpty(_lastFeedback))
            {
                GUI.Label(new Rect(x + 14f, rowY, panelWidth - 28f, 20f), _lastFeedback, _feedbackStyle);
                rowY += 22f;
            }

            const float rowHeight = 78f;
            for (int i = 0; i < missions.Count; i++)
            {
                DrawMissionRow(x + 14f, rowY, panelWidth - 28f, rowHeight, i, missions[i]);
                rowY += rowHeight + 6f;
            }
        }

        private void DrawMissionRow(float x, float y, float width, float height, int slotIndex, ActiveMission mission)
        {
            GUI.Box(new Rect(x, y, width, height), string.Empty);

            string header = mission.DisplayName + " [" + mission.Difficulty + "]";
            GUI.Label(new Rect(x + 8f, y + 4f, width - 16f, 20f), header, _labelStyle);

            string progressLine = "Progress: " + mission.CurrentAmount + "/" + mission.TargetAmount + (mission.IsCompleted ? " (Complete)" : string.Empty);
            GUI.Label(new Rect(x + 8f, y + 24f, width - 16f, 20f), progressLine, _labelStyle);

            string rewardLine = "Reward: " + DescribeReward(mission);
            GUI.Label(new Rect(x + 8f, y + 44f, width - 130f, 20f), rewardLine, _labelStyle);

            if (mission.IsClaimed)
            {
                GUI.Label(new Rect(x + width - 118f, y + 42f, 110f, 24f), "Claimed", _labelStyle);
            }
            else if (mission.IsCompleted)
            {
                if (GUI.Button(new Rect(x + width - 118f, y + 42f, 110f, 24f), "Claim", _rowStyle))
                {
                    _lastFeedback = TryClaimAndDescribe(slotIndex, mission);
                }
            }
        }

        private static string TryClaimAndDescribe(int slotIndex, ActiveMission mission)
        {
            bool claimed = MissionManager.Instance != null && MissionManager.Instance.TryClaimMission(slotIndex);
            return claimed ? "Claimed: " + mission.DisplayName + "!" : "Unable to claim.";
        }

        private static string DescribeReward(ActiveMission mission)
        {
            switch (mission.RewardType)
            {
                case RewardType.Coins:
                    return mission.RewardAmount + " Coins";
                case RewardType.Gems:
                    return mission.RewardAmount + " Gems";
                case RewardType.BattlePassXp:
                    return mission.RewardAmount + " Battle Pass XP";
                default:
                    string temp = mission.IsTemporaryCosmeticReward ? " (Temporary, " + mission.RewardDuration + ")" : string.Empty;
                    return mission.RewardType + temp;
            }
        }

        private static string FormatDuration(double seconds)
        {
            if (seconds <= 0d)
            {
                return "now";
            }

            var span = TimeSpan.FromSeconds(seconds);
            return span.Hours + "h " + span.Minutes + "m";
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _titleStyle.normal.textColor = Color.white;

            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleLeft };
            _labelStyle.normal.textColor = Color.white;

            _rowStyle = new GUIStyle(GUI.skin.button) { fontSize = 12 };

            _feedbackStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _feedbackStyle.normal.textColor = Color.green;
        }
    }
}

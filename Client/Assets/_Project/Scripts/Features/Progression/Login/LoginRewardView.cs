using GulfRun.Core;
using GulfRun.Domain;
using GulfRun.Features.Progression.Configuration;
using UnityEngine;

namespace GulfRun.Features.Progression.Login
{
    /// <summary>
    /// The Login Streak / Daily Login Reward screen — brief "LOGIN
    /// REWARDS"/"LOGIN STREAK"/"SPECIAL LOGIN EVENTS". A
    /// <see cref="SceneSingleton{T}"/> like every other Gameplay-scene
    /// screen. <c>x: 1590</c> is Gameplay.unity's next free toggle-button
    /// slot after Sprint 11's own <c>MissionsView</c> at <c>x: 1410</c>.
    /// </summary>
    public sealed class LoginRewardView : SceneSingleton<LoginRewardView>
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

            if (GUI.Button(new Rect(1590, 10, 150, 34), _open ? "Close Login Reward" : "Login Reward"))
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
            const float panelWidth = 480f;
            const float panelHeight = 220f;
            float x = 1590f;
            float y = 50f;

            GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty);
            GUI.Label(new Rect(x + 14f, y + 8f, panelWidth - 28f, 26f), "LOGIN STREAK", _titleStyle);

            LoginRewardManager manager = LoginRewardManager.Instance;
            if (manager == null)
            {
                return;
            }

            LoginStreakStatus status = manager.Status;
            string eventLine = string.IsNullOrEmpty(manager.ActiveSpecialEventLabel) ? "Standard Calendar" : "Special Event: " + manager.ActiveSpecialEventLabel;
            GUI.Label(new Rect(x + 14f, y + 34f, panelWidth - 28f, 20f), eventLine + " — Current Streak: Day " + status.CurrentStreakDay, _labelStyle);

            LoginRewardCalendarConfig calendar = manager.ActiveCalendar;
            float rowY = y + 60f;

            if (calendar == null)
            {
                GUI.Label(new Rect(x + 14f, rowY, panelWidth - 28f, 20f), "No calendar configured.", _labelStyle);
                return;
            }

            var days = calendar.Days;
            float cellWidth = (panelWidth - 28f) / (days.Count > 0 ? days.Count : 1);
            for (int i = 0; i < days.Count; i++)
            {
                LoginRewardCalendarConfig.LoginRewardEntry entry = days[i];
                bool isCurrent = entry.Day == status.CurrentStreakDay;
                GUI.color = isCurrent ? Color.yellow : Color.white;
                string label = "D" + entry.Day + "\n" + (entry.IsMysteryReward ? "???" : entry.RewardType.ToString());
                GUI.Box(new Rect(x + 14f + i * cellWidth, rowY, cellWidth - 4f, 60f), label);
                GUI.color = Color.white;
            }

            rowY += 68f;

            if (!string.IsNullOrEmpty(_lastFeedback))
            {
                GUI.Label(new Rect(x + 14f, rowY, panelWidth - 28f, 20f), _lastFeedback, _feedbackStyle);
                rowY += 22f;
            }

            bool claimedToday = manager.HasClaimedToday();
            if (GUI.Button(new Rect(x + 14f, rowY, panelWidth - 28f, 30f), claimedToday ? "Already Claimed Today" : "Claim Daily Reward", _rowStyle) && !claimedToday)
            {
                _lastFeedback = manager.TryClaimDailyLogin() ? "Daily Login Reward claimed!" : "Unable to claim.";
            }
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

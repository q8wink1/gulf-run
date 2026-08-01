using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Online.Notifications
{
    /// <summary>
    /// The Notifications screen: every category the brief lists (Friend
    /// Requests, Tournament Starting/Ending, Rewards Ready, Promotion/
    /// Relegation, New Event), newest first, with an unread badge on the
    /// toggle button and a Dismiss button per row.
    /// </summary>
    public sealed class NotificationView : MonoBehaviour
    {
        private bool _open;
        private Vector2 _scroll;
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _unreadLabelStyle;

        private void OnGUI()
        {
            EnsureStyles();

            int unread = NotificationManager.Instance != null ? NotificationManager.Instance.UnreadCount : 0;
            string label = _open ? "Close Notifications" : "Notifications" + (unread > 0 ? " (" + unread + ")" : string.Empty);

            if (GUI.Button(new Rect(690, 10, 170, 34), label))
            {
                _open = !_open;
                if (_open)
                {
                    NotificationManager.Instance?.MarkAllRead();
                }
            }

            if (!_open)
            {
                return;
            }

            DrawPanel();
        }

        private void DrawPanel()
        {
            const float panelWidth = 620f;
            const float panelHeight = 260f;
            float x = 10f;
            float y = 660f;

            GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty);
            GUI.Label(new Rect(x + 14f, y + 8f, panelWidth - 28f, 26f), "NOTIFICATIONS", _titleStyle);

            IReadOnlyList<PlayerNotification> notifications = NotificationManager.Instance != null ? NotificationManager.Instance.Notifications : new List<PlayerNotification>();

            Rect viewport = new Rect(x + 14f, y + 40f, panelWidth - 28f, panelHeight - 54f);
            Rect content = new Rect(0f, 0f, panelWidth - 48f, notifications.Count * 26f);
            _scroll = GUI.BeginScrollView(viewport, _scroll, content);

            for (int i = 0; i < notifications.Count; i++)
            {
                DrawRow(i, notifications[i], content.width);
            }

            GUI.EndScrollView();
        }

        private void DrawRow(int index, PlayerNotification notification, float width)
        {
            float rowY = index * 26f;
            GUIStyle style = notification.Read ? _labelStyle : _unreadLabelStyle;
            GUI.Label(new Rect(0f, rowY, width - 90f, 24f), "[" + notification.Type + "] " + notification.Message, style);

            if (GUI.Button(new Rect(width - 84f, rowY, 84f, 22f), "Dismiss"))
            {
                NotificationManager.Instance?.Dismiss(index);
            }
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _titleStyle.normal.textColor = Color.white;

            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleLeft };
            _labelStyle.normal.textColor = Color.white;

            _unreadLabelStyle = new GUIStyle(_labelStyle) { fontStyle = FontStyle.Bold };
            _unreadLabelStyle.normal.textColor = Color.yellow;
        }
    }
}

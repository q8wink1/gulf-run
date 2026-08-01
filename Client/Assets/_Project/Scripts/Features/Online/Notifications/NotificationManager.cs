using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Online.Notifications
{
    /// <summary>
    /// A capped, newest-first queue of every <see cref="PlayerNotification"/>
    /// the brief's Notifications section requires (Friend Requests,
    /// Tournament Starting/Ending, Rewards Ready, Promotion/Relegation, New
    /// Event) — the single place every other Sprint 9 manager (League,
    /// Championship, Friends) reports a user-facing event, and the single
    /// place <see cref="Notifications.NotificationView"/> reads from.
    /// Capped at <see cref="MaxNotifications"/> for "Minimal Memory Usage".
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class NotificationManager : Singleton<NotificationManager>
    {
        private const int MaxNotifications = 50;

        private readonly List<PlayerNotification> _notifications = new List<PlayerNotification>();

        public IReadOnlyList<PlayerNotification> Notifications => _notifications;

        public int UnreadCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < _notifications.Count; i++)
                {
                    if (!_notifications[i].Read)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public event Action NotificationsChanged;

        protected override void OnInitialize()
        {
        }

        public void Raise(NotificationType type, string message)
        {
            _notifications.Insert(0, new PlayerNotification(type, message, Time.timeAsDouble, false));
            if (_notifications.Count > MaxNotifications)
            {
                _notifications.RemoveAt(_notifications.Count - 1);
            }

            NotificationsChanged?.Invoke();
        }

        public void MarkAllRead()
        {
            for (int i = 0; i < _notifications.Count; i++)
            {
                _notifications[i] = _notifications[i].AsRead();
            }

            NotificationsChanged?.Invoke();
        }

        public void Dismiss(int index)
        {
            if (index < 0 || index >= _notifications.Count)
            {
                return;
            }

            _notifications.RemoveAt(index);
            NotificationsChanged?.Invoke();
        }
    }
}

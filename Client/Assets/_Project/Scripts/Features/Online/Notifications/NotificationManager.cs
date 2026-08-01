using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Services;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Online.Notifications
{
    /// <summary>
    /// A capped, newest-first queue of every <see cref="PlayerNotification"/>
    /// the brief's Notifications section requires (Friend Requests,
    /// Tournament Starting/Ending, Rewards Ready, Promotion/Relegation, New
    /// Event — Sprint 9; New Offers/Limited-Time Deals/Battle Pass Expiring/
    /// New Store Items/Purchase Success — Sprint 10; New Missions Available/
    /// Mission Completed/Daily Reward Available/Temporary Item Expiring
    /// Soon — Sprint 11) — the single place every other manager (League,
    /// Championship, Friends, <c>Features.Store</c> via
    /// <see cref="StoreNotificationBridge"/>, and <c>Features.Progression</c>
    /// via <see cref="ProgressionNotificationBridge"/>) reports a
    /// user-facing event, and the single place
    /// <see cref="Notifications.NotificationView"/> reads from. Capped at
    /// <see cref="MaxNotifications"/> for "Minimal Memory Usage".
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class NotificationManager : Singleton<NotificationManager>, INotificationSummaryProvider
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
            NotificationSummaryService.Current = this;
        }

        private void OnEnable()
        {
            StoreNotificationBridge.NotificationRequested += Raise;
            ProgressionNotificationBridge.NotificationRequested += Raise;
        }

        private void OnDisable()
        {
            StoreNotificationBridge.NotificationRequested -= Raise;
            ProgressionNotificationBridge.NotificationRequested -= Raise;

            if (ReferenceEquals(NotificationSummaryService.Current, this))
            {
                NotificationSummaryService.Current = null;
            }
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

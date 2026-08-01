using System;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Lets <c>Features.Progression</c> raise a user-facing notification
    /// without ever referencing <c>Features.Online.Notifications.NotificationManager</c>
    /// directly — the same small static event-bridge shape
    /// <see cref="StoreNotificationBridge"/> already established for
    /// <c>Features.Store</c> in Sprint 10 (and <see cref="FriendRequestBridge"/>
    /// before that in Sprint 9). <c>NotificationManager</c> is the sole
    /// subscriber.
    /// </summary>
    public static class ProgressionNotificationBridge
    {
        public static event Action<NotificationType, string> NotificationRequested;

        public static void Raise(NotificationType type, string message) => NotificationRequested?.Invoke(type, message);
    }
}

using System;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Lets <c>Features.Store</c> raise a Store/Economy notification
    /// (New Offer, Limited-Time Deal, Battle Pass Expiring, New Store Item,
    /// Purchase Success — Sprint 10 brief) without referencing
    /// <c>Features.Online.Notifications.NotificationManager</c> directly —
    /// the same event-bridge shape <see cref="FriendRequestBridge"/>
    /// established in Sprint 9 for exactly this "sibling Feature assemblies
    /// never reference each other" reason. <c>NotificationManager</c> is
    /// the sole subscriber and performs the actual <c>Raise</c> call into
    /// its queue.
    /// </summary>
    public static class StoreNotificationBridge
    {
        public static event Action<NotificationType, string> NotificationRequested;

        public static void Raise(NotificationType type, string message) => NotificationRequested?.Invoke(type, message);
    }
}

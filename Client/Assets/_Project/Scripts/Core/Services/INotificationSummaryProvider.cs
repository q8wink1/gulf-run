namespace GulfRun.Core.Services
{
    /// <summary>
    /// Read-only seam onto the local player's unread notification count
    /// (Sprint 13 Main Menu Top Bar bell icon badge). Implemented by
    /// <c>Features.Online.Notifications.NotificationManager</c> so
    /// Features.MainMenu can show it without ever referencing
    /// Features.Online.
    /// </summary>
    public interface INotificationSummaryProvider
    {
        int UnreadCount { get; }
    }
}

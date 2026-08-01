namespace GulfRun.Core.Services
{
    /// <summary>Minimal service locator for <see cref="INotificationSummaryProvider"/> — same locator shape as <see cref="MapContextService"/>.</summary>
    public static class NotificationSummaryService
    {
        public static INotificationSummaryProvider Current { get; set; }
    }
}

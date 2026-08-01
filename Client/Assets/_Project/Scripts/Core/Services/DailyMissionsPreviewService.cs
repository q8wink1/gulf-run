namespace GulfRun.Core.Services
{
    /// <summary>Minimal service locator for <see cref="IDailyMissionsPreviewProvider"/> — same locator shape as <see cref="MapContextService"/>.</summary>
    public static class DailyMissionsPreviewService
    {
        public static IDailyMissionsPreviewProvider Current { get; set; }
    }
}

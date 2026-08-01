namespace GulfRun.Core.Services
{
    /// <summary>Minimal service locator for <see cref="IFriendsSummaryProvider"/> — same locator shape as <see cref="MapContextService"/>.</summary>
    public static class FriendsSummaryService
    {
        public static IFriendsSummaryProvider Current { get; set; }
    }
}

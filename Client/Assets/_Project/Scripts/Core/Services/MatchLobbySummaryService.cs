namespace GulfRun.Core.Services
{
    /// <summary>Minimal service locator for <see cref="IMatchLobbySummaryProvider"/> — same locator shape as <see cref="MapContextService"/>.</summary>
    public static class MatchLobbySummaryService
    {
        public static IMatchLobbySummaryProvider Current { get; set; }
    }
}

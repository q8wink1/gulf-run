namespace GulfRun.Core.Services
{
    /// <summary>Minimal service locator for <see cref="ILoginRewardStatusProvider"/> — same locator shape as <see cref="MapContextService"/>.</summary>
    public static class LoginRewardStatusService
    {
        public static ILoginRewardStatusProvider Current { get; set; }
    }
}

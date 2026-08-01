namespace GulfRun.Core.Services
{
    /// <summary>Minimal service locator for the local player's <see cref="ILocalProfileProvider"/> — same locator shape as <see cref="MapContextService"/>.</summary>
    public static class LocalProfileProviderService
    {
        public static ILocalProfileProvider Current { get; set; }
    }
}

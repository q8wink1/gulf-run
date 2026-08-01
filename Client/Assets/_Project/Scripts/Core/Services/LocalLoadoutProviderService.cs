namespace GulfRun.Core.Services
{
    /// <summary>Minimal service locator for the active <see cref="ILocalLoadoutProvider"/> — same pattern as <see cref="RunSpeedService"/>.</summary>
    public static class LocalLoadoutProviderService
    {
        public static ILocalLoadoutProvider Current { get; set; }
    }
}

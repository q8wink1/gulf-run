namespace GulfRun.Core.Services
{
    /// <summary>Minimal service locator for the local player's <see cref="ILocalPlayerStateProvider"/>.</summary>
    public static class LocalPlayerStateService
    {
        public static ILocalPlayerStateProvider Current { get; set; }
    }
}

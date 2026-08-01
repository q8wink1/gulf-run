namespace GulfRun.Core.Services
{
    /// <summary>
    /// Static service locator for <see cref="IMapContextProvider"/> — set
    /// once by <c>Features.Maps.MapEnvironmentManager</c> at startup, the
    /// same locator shape as <see cref="RunSpeedService"/>/
    /// <see cref="DifficultyService"/>.
    /// </summary>
    public static class MapContextService
    {
        public static IMapContextProvider Current { get; set; }
    }
}

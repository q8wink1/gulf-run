namespace GulfRun.Core.Services
{
    /// <summary>Minimal service locator for the local player's <see cref="IRaceProgressProvider"/>.</summary>
    public static class RaceProgressService
    {
        public static IRaceProgressProvider Current { get; set; }
    }
}

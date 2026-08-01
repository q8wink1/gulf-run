namespace GulfRun.Core.Services
{
    /// <summary>Minimal runtime service locator for the active <see cref="IDifficultyProvider"/>. See <see cref="RunSpeedService"/> for the pattern this mirrors.</summary>
    public static class DifficultyService
    {
        public static IDifficultyProvider Current { get; set; }
    }
}

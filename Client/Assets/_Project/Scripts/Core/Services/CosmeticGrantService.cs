namespace GulfRun.Core.Services
{
    /// <summary>Minimal service locator for the active <see cref="ICosmeticGrantService"/> — same pattern as <see cref="LocalLoadoutProviderService"/>.</summary>
    public static class CosmeticGrantService
    {
        public static ICosmeticGrantService Current { get; set; }
    }
}

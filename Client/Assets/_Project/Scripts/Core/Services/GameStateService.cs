namespace GulfRun.Core.Services
{
    /// <summary>
    /// Minimal runtime service locator that lets the active
    /// <see cref="IGameStateProvider"/> (the endless-runner Game Loop
    /// Controller, when present) publish itself for the player to consume,
    /// with zero compile-time coupling between the PlayerController and
    /// EndlessRunner feature assemblies. When no provider is registered,
    /// consumers fall back to their own local defaults, so Sprint 2's player
    /// behaves identically stand-alone (always "running", no countdown gate).
    /// </summary>
    public static class GameStateService
    {
        public static IGameStateProvider Current { get; set; }
    }
}

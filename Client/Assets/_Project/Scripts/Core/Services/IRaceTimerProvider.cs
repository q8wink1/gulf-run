namespace GulfRun.Core.Services
{
    /// <summary>
    /// Elapsed race-clock seconds while the local session is Running —
    /// published by Features.RaceHud so other systems can read a single clock
    /// without owning UI state. Optional; RaceHud falls back to its own timer
    /// when null.
    /// </summary>
    public interface IRaceTimerProvider
    {
        float ElapsedRaceSeconds { get; }
    }

    public static class RaceTimerService
    {
        public static IRaceTimerProvider Current { get; set; }
    }
}

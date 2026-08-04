namespace GulfRun.Domain
{
    /// <summary>
    /// Top-level state of a GulfRun race session (Sprint 23.8).
    /// Pure data — Features systems react via <c>RaceManager</c> events;
    /// pause is a flag on the manager, not a separate enum value.
    /// </summary>
    public enum RaceState
    {
        /// <summary>Scene loaded; waiting for countdown / start callers.</summary>
        Waiting,

        /// <summary>3-2-1-GO (or in-game countdown) before Running.</summary>
        Countdown,

        /// <summary>Active race; may also be paused via <c>RaceManager.IsPaused</c>.</summary>
        Running,

        /// <summary>Race ended; no auto-finish logic this sprint.</summary>
        Finished
    }
}

namespace GulfRun.Domain
{
    /// <summary>
    /// Sprint 23.11 — how a race declares a winner (engine-free).
    /// Features systems consult this via <c>GameRulesManager</c>; evaluation
    /// stubs do not run distance / standings checks this sprint.
    /// </summary>
    public enum WinCondition
    {
        /// <summary>First player to reach the configured race distance / finish line.</summary>
        FinishLine = 0,

        /// <summary>Highest progress when the race ends (timeout / host finish).</summary>
        HighestProgress = 1,

        /// <summary>Reserved — last remaining non-eliminated player wins.</summary>
        LastPlayerStanding = 2
    }
}

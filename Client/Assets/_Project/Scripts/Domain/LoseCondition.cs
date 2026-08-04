namespace GulfRun.Domain
{
    /// <summary>
    /// Sprint 23.11 — why a player loses / leaves contention (engine-free).
    /// Reported through <c>GameRulesManager</c> stubs; no elimination gameplay yet.
    /// </summary>
    public enum LoseCondition
    {
        /// <summary>Player disconnected from the match session.</summary>
        Disconnect = 0,

        /// <summary>Player was eliminated (gap / last-standing rules — future).</summary>
        Elimination = 1,

        /// <summary>Race or player safety-net time limit elapsed.</summary>
        Timeout = 2
    }
}

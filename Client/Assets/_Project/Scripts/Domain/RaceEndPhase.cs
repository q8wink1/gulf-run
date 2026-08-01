namespace GulfRun.Domain
{
    /// <summary>
    /// Host-authoritative phase of the post-race presentation flow, broadcast
    /// so every client renders the identical Victory Ceremony / Reward Screen
    /// step at the same time (the Ceremony/Reward Screen networking
    /// requirement) — distinct from <see cref="MatchState"/>, which only
    /// tracks the coarse Waiting/Countdown/Running/Finished lifecycle.
    /// </summary>
    public enum RaceEndPhase
    {
        /// <summary>No post-race presentation active (race is still running, or the flow has finished and the match has returned to <see cref="MatchState.Waiting"/>).</summary>
        None,

        /// <summary>Podium Ceremony: top 3 finishers presented together.</summary>
        Podium,

        /// <summary>Private per-player Reward Screen.</summary>
        Reward
    }
}

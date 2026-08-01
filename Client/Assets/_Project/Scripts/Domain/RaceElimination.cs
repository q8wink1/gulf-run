namespace GulfRun.Domain
{
    /// <summary>
    /// Pure elimination-gap predicates. "Leader" distance is the greatest
    /// currently-known distance among every participant (finished players'
    /// distance is their finish-line distance, so once anyone finishes, the
    /// gap for stragglers is effectively measured against the finish line
    /// itself — deliberately increasing elimination pressure as the race
    /// winds down). A separate, lower recovery gap (vs. the warning gap)
    /// gives a small hysteresis band so a player riding exactly on the
    /// threshold does not flicker between Safe/Warning every tick.
    /// </summary>
    public static class RaceElimination
    {
        public static bool ShouldWarn(float leaderDistanceMeters, float playerDistanceMeters, float warningGapMeters) =>
            (leaderDistanceMeters - playerDistanceMeters) > warningGapMeters;

        public static bool ShouldClearWarning(float leaderDistanceMeters, float playerDistanceMeters, float recoveryGapMeters) =>
            (leaderDistanceMeters - playerDistanceMeters) <= recoveryGapMeters;
    }
}

namespace GulfRun.Domain
{
    /// <summary>
    /// The local player's live Login Streak state — mutable (matching
    /// <see cref="BattlePassStatus"/>'s style), owned by
    /// <c>Core.Backend.IProgressionBackendService</c>. <see cref="LastClaimAtSeconds"/>
    /// is real-world (Unix epoch) seconds — a game-time value would reset
    /// to zero every app restart and could never detect "a calendar day
    /// actually passed", which the brief's "if player misses one day, the
    /// streak resets" rule depends on (see <see cref="LoginStreakCalculator"/>).
    /// </summary>
    public sealed class LoginStreakStatus
    {
        /// <summary>1-based day within the active calendar's cycle (wraps back to 1 after completing the full cycle — e.g. Day 8 of a 7-day calendar becomes Day 1 again, a fresh lap of the same reward cycle).</summary>
        public int CurrentStreakDay { get; set; }

        public double LastClaimAtSeconds { get; set; }

        /// <summary>Total number of daily claims ever made, across every streak (reset or not) — a simple lifetime counter for debug/analytics, never itself reset.</summary>
        public int TotalLoginsEver { get; set; }
    }
}

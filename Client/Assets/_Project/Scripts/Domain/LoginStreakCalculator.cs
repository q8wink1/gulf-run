using System;

namespace GulfRun.Domain
{
    /// <summary>
    /// Pure (no UnityEngine dependency) civil-calendar-day math for the
    /// Login Streak brief rules: "Daily login rewards ... If player misses
    /// one day: Login Streak resets. Reward cycle restarts from Day 1."
    /// Uses whole-day buckets of real-world (Unix epoch) seconds rather
    /// than "24 hours since last claim", so a player who logs in at 11pm
    /// and again at 1am the same calendar day is correctly treated as
    /// "already claimed today", while one who logs in at 1am and again at
    /// 11pm the same day cannot double-claim by waiting a few hours.
    /// </summary>
    public static class LoginStreakCalculator
    {
        private const double DaySeconds = 86400d;

        public static long DayIndex(double epochSeconds) => (long)Math.Floor(epochSeconds / DaySeconds);

        /// <summary>True once a claim has already been recorded for the same calendar day as <paramref name="nowSeconds"/>.</summary>
        public static bool HasClaimedForToday(double lastClaimAtSeconds, double nowSeconds)
        {
            return lastClaimAtSeconds > 0d && DayIndex(lastClaimAtSeconds) == DayIndex(nowSeconds);
        }

        /// <summary>
        /// Resolves which 1-based streak day the NEXT claim should award.
        /// First-ever claim -> Day 1. Exactly one calendar day since the
        /// last claim -> the streak continues (wrapping back to 1 once
        /// <paramref name="cycleLength"/> is exceeded, starting a fresh lap
        /// of the same reward cycle). Two or more calendar days since the
        /// last claim (a missed day) -> resets to Day 1. Callers are
        /// expected to have already checked <see cref="HasClaimedForToday"/>
        /// is false before calling this.
        /// </summary>
        public static int ResolveNextStreakDay(double lastClaimAtSeconds, double nowSeconds, int previousStreakDay, int cycleLength)
        {
            if (cycleLength <= 0)
            {
                cycleLength = 1;
            }

            if (lastClaimAtSeconds <= 0d)
            {
                return 1;
            }

            long dayDelta = DayIndex(nowSeconds) - DayIndex(lastClaimAtSeconds);
            if (dayDelta <= 0)
            {
                // Same day (caller should have filtered this via HasClaimedForToday)
                // or the clock moved backwards — never regress the streak.
                return previousStreakDay < 1 ? 1 : previousStreakDay;
            }

            if (dayDelta == 1)
            {
                int next = previousStreakDay + 1;
                return next > cycleLength ? 1 : next;
            }

            // One or more full calendar days were missed.
            return 1;
        }
    }
}

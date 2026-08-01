using System.Collections.Generic;

namespace GulfRun.Domain
{
    /// <summary>
    /// Pure, engine-free League math: no hardcoded trophy numbers here —
    /// callers (<c>Features.Online.Leagues.LeagueManager</c>) supply the
    /// tunable thresholds from <c>Features.Online.Configuration.LeagueCatalogConfig</c>.
    /// Two separate concerns on purpose (Single Responsibility): computing
    /// a match's trophy delta, and resolving which <see cref="League"/> a
    /// trophy count currently sits in (which also *is* the season
    /// promotion/relegation check — comparing the newly resolved league to
    /// the previously known one is enough, no separate "did we cross a
    /// threshold" bookkeeping needed).
    /// </summary>
    public static class LeagueRules
    {
        /// <summary>
        /// Placeholder, tunable trophy reward/penalty per race outcome —
        /// not a final balance pass (see Sprint 9 report Remaining TODOs).
        /// 1st = +30, 2nd/3rd = +15, any other finish = -10 (never taking a
        /// player below zero total trophies is the caller's job, since only
        /// the caller knows the player's running total).
        /// </summary>
        public static int ComputeTrophyDelta(int finishPosition)
        {
            if (finishPosition == 1)
            {
                return 30;
            }

            if (finishPosition == 2 || finishPosition == 3)
            {
                return 15;
            }

            return finishPosition > 3 ? -10 : 0;
        }

        /// <summary>
        /// Resolves the highest league whose threshold the trophy count has
        /// reached. <paramref name="ascendingThresholds"/> must be indexed
        /// exactly like <see cref="League"/>'s declaration order (index 0 =
        /// <see cref="League.Bronze"/>'s minimum, etc.) and each entry must
        /// be a valid, ascending trophy count for its tier.
        /// </summary>
        public static League ResolveLeague(int trophyCount, IReadOnlyList<int> ascendingThresholds)
        {
            League resolved = League.Bronze;
            if (ascendingThresholds == null)
            {
                return resolved;
            }

            for (int i = 0; i < ascendingThresholds.Count; i++)
            {
                if (trophyCount >= ascendingThresholds[i])
                {
                    resolved = (League)i;
                }
            }

            return resolved;
        }
    }
}

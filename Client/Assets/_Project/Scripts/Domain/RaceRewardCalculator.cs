using System;
using System.Collections.Generic;

namespace GulfRun.Domain
{
    /// <summary>
    /// Pure, host-only reward formula: a placement-indexed lookup (clamped to
    /// the configured table's last entry, so a config authored for 4 players
    /// still degrades gracefully for a smaller or larger lobby) plus a flat
    /// participation bonus for Experience. No values are invented here —
    /// every number is supplied by the caller's config (<c>RaceFinishConfig</c>
    /// in Sprint 7); this class only implements the arithmetic. See the
    /// Sprint 7 report for how this reconciles with P011's "reward amounts
    /// are placeholder-only, not yet defined" status.
    /// </summary>
    public static class RaceRewardCalculator
    {
        public static RaceRewardBreakdown Calculate(
            int connectionId,
            int finishPosition,
            int coinsCollectedRaw,
            float coinRewardMultiplier,
            IReadOnlyList<int> bonusCoinsByPosition,
            IReadOnlyList<int> rankPointsByPosition,
            IReadOnlyList<int> experienceByPosition,
            int participationExperience)
        {
            float safeMultiplier = coinRewardMultiplier <= 0f ? 1f : coinRewardMultiplier;
            int coinsCollected = (int)Math.Round(coinsCollectedRaw * safeMultiplier, MidpointRounding.AwayFromZero);

            int bonusCoins = LookupByPosition(bonusCoinsByPosition, finishPosition);
            int rankPoints = LookupByPosition(rankPointsByPosition, finishPosition);
            int experience = LookupByPosition(experienceByPosition, finishPosition) + Math.Max(0, participationExperience);
            int totalReward = coinsCollected + bonusCoins;

            return new RaceRewardBreakdown(connectionId, coinsCollected, bonusCoins, rankPoints, experience, totalReward);
        }

        private static int LookupByPosition(IReadOnlyList<int> byPosition, int finishPosition)
        {
            if (byPosition == null || byPosition.Count == 0)
            {
                return 0;
            }

            int index = finishPosition - 1;
            if (index < 0)
            {
                index = 0;
            }
            else if (index >= byPosition.Count)
            {
                index = byPosition.Count - 1;
            }

            return byPosition[index];
        }
    }
}

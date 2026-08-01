namespace GulfRun.Domain
{
    /// <summary>Immutable result of a score calculation.</summary>
    public readonly struct ScoreBreakdown
    {
        public readonly float DistanceScore;
        public readonly float CoinScore;
        public readonly float TotalScore;

        public ScoreBreakdown(float distanceScore, float coinScore, float totalScore)
        {
            DistanceScore = distanceScore;
            CoinScore = coinScore;
            TotalScore = totalScore;
        }
    }

    /// <summary>
    /// Pure scoring formula: distance score + coin score, scaled by a
    /// multiplier. Kept multiplier-aware from day one so a future combo
    /// system only needs to feed a different multiplier in, with no change
    /// to the calculation itself.
    /// </summary>
    public static class ScoreCalculator
    {
        public static ScoreBreakdown Calculate(
            double distanceMeters,
            int coinsCollected,
            float distanceScorePerMeter,
            float coinScoreValue,
            float multiplier)
        {
            float safeMultiplier = multiplier <= 0f ? 1f : multiplier;
            float distanceScore = (float)distanceMeters * distanceScorePerMeter * safeMultiplier;
            float coinScore = coinsCollected * coinScoreValue * safeMultiplier;

            return new ScoreBreakdown(distanceScore, coinScore, distanceScore + coinScore);
        }
    }
}

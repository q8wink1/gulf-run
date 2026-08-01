namespace GulfRun.Domain
{
    /// <summary>
    /// Pure "count from 0 up to the final value" easing for the Reward
    /// Screen's animated counters. Linear by design (simplest possible
    /// reading experience for a number that must be trusted, not just
    /// pretty); swapping in an eased curve later only touches this one
    /// function.
    /// </summary>
    public static class RewardCounterAnimation
    {
        public static int EvaluateInt(float elapsedSeconds, float durationSeconds, int targetValue)
        {
            if (durationSeconds <= 0f || elapsedSeconds >= durationSeconds)
            {
                return targetValue;
            }

            if (elapsedSeconds <= 0f)
            {
                return 0;
            }

            float t = elapsedSeconds / durationSeconds;
            return (int)System.Math.Round(targetValue * t, System.MidpointRounding.AwayFromZero);
        }
    }
}

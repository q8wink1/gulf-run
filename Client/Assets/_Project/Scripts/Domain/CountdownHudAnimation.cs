namespace GulfRun.Domain
{
    /// <summary>
    /// Pure scale/alpha curves for the 3-2-1-GO countdown presentation
    /// (punch-in on each whole second, then a short GO slide-away).
    /// </summary>
    public static class CountdownHudAnimation
    {
        /// <summary>Punch scale: 1.35 → 1.0 over the first ~0.35 of each second beat.</summary>
        public static float EvaluatePunchScale(float secondsIntoBeat)
        {
            float t = secondsIntoBeat < 0f ? 0f : (secondsIntoBeat > 0.35f ? 1f : secondsIntoBeat / 0.35f);
            float ease = 1f - (1f - t) * (1f - t);
            return 1.35f + (1f - 1.35f) * ease;
        }

        /// <summary>GO exit: slides up and fades over <paramref name="holdSeconds"/>.</summary>
        public static void EvaluateGoExit(float elapsedSinceGo, float holdSeconds, out float verticalOffset01, out float alpha01)
        {
            float duration = holdSeconds <= 0.01f ? 0.01f : holdSeconds;
            float t = elapsedSinceGo / duration;
            if (t < 0f)
            {
                t = 0f;
            }

            if (t > 1f)
            {
                t = 1f;
            }

            verticalOffset01 = t * -0.35f;
            alpha01 = 1f - t;
        }
    }
}

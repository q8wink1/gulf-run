namespace GulfRun.Domain
{
    /// <summary>
    /// Pure function mapping distance traveled to a normalized 0..1 difficulty
    /// scalar. Consumed by both the speed system (progressive speed increase)
    /// and the spawn system (weight biasing) so "how far into the difficulty
    /// ramp are we" is computed exactly once.
    /// </summary>
    public static class DifficultyCurve
    {
        public static float Evaluate(double distanceMeters, float rampStartMeters, float rampEndMeters)
        {
            if (rampEndMeters <= rampStartMeters)
            {
                return distanceMeters >= rampStartMeters ? 1f : 0f;
            }

            float t = (float)((distanceMeters - rampStartMeters) / (rampEndMeters - rampStartMeters));
            if (t < 0f)
            {
                return 0f;
            }

            if (t > 1f)
            {
                return 1f;
            }

            return t;
        }
    }
}

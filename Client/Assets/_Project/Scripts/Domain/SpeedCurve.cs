namespace GulfRun.Domain
{
    /// <summary>
    /// Pure function for the global run speed's progressive increase: linear
    /// ramp from base to max speed over a configurable distance, then clamped.
    /// </summary>
    public static class SpeedCurve
    {
        public static float Evaluate(double distanceMeters, float baseSpeed, float maxSpeed, float rampDistanceMeters)
        {
            if (rampDistanceMeters <= 0f)
            {
                return maxSpeed;
            }

            float t = (float)(distanceMeters / rampDistanceMeters);
            if (t < 0f)
            {
                t = 0f;
            }
            else if (t > 1f)
            {
                t = 1f;
            }

            return baseSpeed + (maxSpeed - baseSpeed) * t;
        }
    }
}

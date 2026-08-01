namespace GulfRun.Domain
{
    /// <summary>
    /// Pure oscillation used for every Victory Ceremony "gentle motion" —
    /// the Champion's celebration bounce, the 2nd/3rd place celebration
    /// pulse, and the national flags' gentle sway (Sprint 7 addendum) — so
    /// there is exactly one sine-wave implementation behind all of them
    /// instead of a copy per effect.
    /// </summary>
    public static class CelebrationAnimation
    {
        public static float EvaluateOffset(double elapsedSeconds, float amplitude, float frequencyHz) =>
            amplitude * (float)System.Math.Sin(elapsedSeconds * frequencyHz * 2d * System.Math.PI);
    }
}

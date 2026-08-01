namespace GulfRun.Domain
{
    /// <summary>Pure conversion helper — no UnityEngine dependency, matching every other small Domain calculator (e.g. <see cref="TrapDifficulty"/>) in this project.</summary>
    public static class TemporaryCosmeticDurationExtensions
    {
        private const double DaySeconds = 86400d;

        public static double ToSeconds(this TemporaryCosmeticDuration duration)
        {
            switch (duration)
            {
                case TemporaryCosmeticDuration.TwoDays: return 2d * DaySeconds;
                case TemporaryCosmeticDuration.ThreeDays: return 3d * DaySeconds;
                case TemporaryCosmeticDuration.SevenDays: return 7d * DaySeconds;
                default: return 2d * DaySeconds;
            }
        }
    }
}

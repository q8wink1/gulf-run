namespace GulfRun.Domain
{
    /// <summary>
    /// Classic three-lane endless-runner placement (Sprint 23.4).
    /// Engine-free so lane math can be shared / tested without Unity.
    /// </summary>
    public enum RunnerLane
    {
        Left = 0,
        Center = 1,
        Right = 2
    }

    /// <summary>Pure helpers for <see cref="RunnerLane"/> clamping and offsets.</summary>
    public static class RunnerLaneMath
    {
        public static RunnerLane Clamp(int laneIndex)
        {
            if (laneIndex < (int)RunnerLane.Left)
            {
                return RunnerLane.Left;
            }

            if (laneIndex > (int)RunnerLane.Right)
            {
                return RunnerLane.Right;
            }

            return (RunnerLane)laneIndex;
        }

        public static RunnerLane Shift(RunnerLane current, int delta)
        {
            return Clamp((int)current + delta);
        }

        /// <summary>World X for a lane given center X and spacing between lanes.</summary>
        public static float LaneX(RunnerLane lane, float centerX, float laneSpacing)
        {
            return centerX + (((int)lane - 1) * laneSpacing);
        }
    }
}

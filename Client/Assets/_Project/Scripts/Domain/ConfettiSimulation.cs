namespace GulfRun.Domain
{
    /// <summary>
    /// Pure, deterministic "golden confetti" placeholder simulation for the
    /// Champion's celebration (Sprint 7 addendum): each particle falls
    /// continuously and loops back to the top, with no per-frame mutable
    /// state to maintain — every value is a function of particle index plus
    /// elapsed time, the same "one pure function, no animation state stored
    /// in the view" shape as <see cref="RewardCounterAnimation"/> and
    /// <see cref="CelebrationAnimation"/>. A real particle system replaces
    /// this the moment art/VFX assets exist.
    /// </summary>
    public static class ConfettiSimulation
    {
        public static ConfettiParticle Evaluate(int particleIndex, double elapsedSeconds, float fallSpeed)
        {
            float x = Hash01(particleIndex, 1.7f);
            float startY = Hash01(particleIndex, 3.1f);
            float y = Frac(startY + (float)elapsedSeconds * fallSpeed);
            float spinSeed = Hash01(particleIndex, 5.3f);
            float rotationDegrees = ((float)elapsedSeconds * 90f + spinSeed * 360f) % 360f;

            return new ConfettiParticle(x, y, rotationDegrees);
        }

        private static float Hash01(int index, float salt)
        {
            float value = (float)System.Math.Sin(index * 12.9898f + salt * 78.233f) * 43758.5453f;
            return Frac(value);
        }

        private static float Frac(float value) => value - (float)System.Math.Floor(value);
    }
}

namespace GulfRun.Domain
{
    /// <summary>One simulated confetti particle's normalized position/rotation at a given moment — see <see cref="ConfettiSimulation"/>.</summary>
    public readonly struct ConfettiParticle
    {
        /// <summary>0..1 across the effect area.</summary>
        public readonly float NormalizedX;

        /// <summary>0..1 down the effect area; loops back to 0 once it would exceed 1 (continuous falling/recycling).</summary>
        public readonly float NormalizedY;

        public readonly float RotationDegrees;

        public ConfettiParticle(float normalizedX, float normalizedY, float rotationDegrees)
        {
            NormalizedX = normalizedX;
            NormalizedY = normalizedY;
            RotationDegrees = rotationDegrees;
        }
    }
}

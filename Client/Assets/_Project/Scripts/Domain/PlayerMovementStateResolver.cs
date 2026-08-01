namespace GulfRun.Domain
{
    /// <summary>
    /// Pure function that derives the current <see cref="PlayerMovementState"/>
    /// from physics facts. Contains no UnityEngine dependency so it is trivially
    /// unit-testable and can run identically on a future authoritative server
    /// for movement-state reconciliation.
    /// </summary>
    public static class PlayerMovementStateResolver
    {
        /// <summary>
        /// Vertical speed (m/s) below which airborne motion is considered "falling"
        /// rather than "jumping" (rising).
        /// </summary>
        private const float RisingVelocityEpsilon = 0.01f;

        public static PlayerMovementState Resolve(
            bool isGrounded,
            bool justLanded,
            float verticalVelocity,
            float horizontalSpeed,
            bool isRunEnabled)
        {
            if (justLanded)
            {
                return PlayerMovementState.Landing;
            }

            if (!isGrounded)
            {
                return verticalVelocity > RisingVelocityEpsilon
                    ? PlayerMovementState.Jumping
                    : PlayerMovementState.Falling;
            }

            return isRunEnabled && horizontalSpeed > RisingVelocityEpsilon
                ? PlayerMovementState.Running
                : PlayerMovementState.Idle;
        }
    }
}

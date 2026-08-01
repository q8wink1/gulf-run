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

        /// <param name="jumpsUsed">
        /// Jumps consumed since the last ground contact. Used only to distinguish the
        /// visual/animation state of the first jump (<see cref="PlayerMovementState.Jumping"/>)
        /// from the strict second/final jump (<see cref="PlayerMovementState.DoubleJumping"/>);
        /// it does not affect whether a jump is currently allowed (that is enforced by the
        /// caller before requesting a jump).
        /// </param>
        public static PlayerMovementState Resolve(
            bool isGrounded,
            bool justLanded,
            float verticalVelocity,
            float horizontalSpeed,
            bool isRunEnabled,
            int jumpsUsed)
        {
            if (justLanded)
            {
                return PlayerMovementState.Landing;
            }

            if (!isGrounded)
            {
                if (verticalVelocity > RisingVelocityEpsilon)
                {
                    return jumpsUsed >= 2 ? PlayerMovementState.DoubleJumping : PlayerMovementState.Jumping;
                }

                return PlayerMovementState.Falling;
            }

            return isRunEnabled && horizontalSpeed > RisingVelocityEpsilon
                ? PlayerMovementState.Running
                : PlayerMovementState.Idle;
        }
    }
}

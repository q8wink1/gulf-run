namespace GulfRun.Domain
{
    /// <summary>
    /// A single timestamped sample of one player's networked state:
    /// position, rotation, and animation/movement state (reusing
    /// <see cref="PlayerMovementState"/> directly — it already distinguishes
    /// Running/Jumping/DoubleJumping/Falling/Landing, so Jump/Landing/Running
    /// state sync falls out of the existing single-player enum for free,
    /// with no duplicated state machine).
    /// </summary>
    public readonly struct NetworkPlayerSnapshot
    {
        public readonly int ConnectionId;
        public readonly NetVector2 Position;
        public readonly float RotationDegrees;
        public readonly PlayerMovementState AnimationState;
        public readonly double TimestampSeconds;

        public NetworkPlayerSnapshot(
            int connectionId,
            NetVector2 position,
            float rotationDegrees,
            PlayerMovementState animationState,
            double timestampSeconds)
        {
            ConnectionId = connectionId;
            Position = position;
            RotationDegrees = rotationDegrees;
            AnimationState = animationState;
            TimestampSeconds = timestampSeconds;
        }
    }
}

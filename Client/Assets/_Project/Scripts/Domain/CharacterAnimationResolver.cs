namespace GulfRun.Domain
{
    /// <summary>
    /// Pure mapping from the existing locomotion/race-outcome state machines
    /// onto the unified <see cref="CharacterAnimationState"/> vocabulary the
    /// Sprint 8 brief requires ("Support: Idle, Run, Jump, Double Jump, Fall,
    /// Win, Lose, Celebrate"). Deliberately does not introduce a second,
    /// competing state machine — <see cref="PlayerMovementState"/> (Sprint 2)
    /// and <see cref="FinishReason"/> (Sprint 7) remain the single sources of
    /// truth; this type only ever translates.
    /// </summary>
    public static class CharacterAnimationResolver
    {
        public static CharacterAnimationState FromMovementState(PlayerMovementState state)
        {
            switch (state)
            {
                case PlayerMovementState.Running:
                    return CharacterAnimationState.Run;
                case PlayerMovementState.Jumping:
                    return CharacterAnimationState.Jump;
                case PlayerMovementState.DoubleJumping:
                    return CharacterAnimationState.DoubleJump;
                case PlayerMovementState.Falling:
                    return CharacterAnimationState.Fall;
                default:
                    return CharacterAnimationState.Idle;
            }
        }

        public static CharacterAnimationState FromFinishReason(FinishReason reason) =>
            reason == FinishReason.Completed ? CharacterAnimationState.Win : CharacterAnimationState.Lose;
    }
}

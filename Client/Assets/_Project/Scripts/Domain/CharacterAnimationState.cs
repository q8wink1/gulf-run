namespace GulfRun.Domain
{
    /// <summary>
    /// The unified character animation vocabulary required by the Sprint 8
    /// brief. <see cref="Idle"/>/<see cref="Run"/>/<see cref="Jump"/>/
    /// <see cref="DoubleJump"/>/<see cref="Fall"/> are derived every frame
    /// from the existing <see cref="PlayerMovementState"/> locomotion state
    /// machine (see <see cref="CharacterAnimationResolver"/>) — no duplicated
    /// movement logic. <see cref="Win"/>/<see cref="Lose"/> are raised once
    /// per race from a player's own <see cref="PlayerRaceResult.Reason"/>
    /// (Sprint 7), and <see cref="Celebrate"/> is raised for the local
    /// top-3 the instant the Podium Ceremony begins — see
    /// <c>Core.Services.CharacterAnimationCueService</c>.
    /// </summary>
    public enum CharacterAnimationState
    {
        Idle,
        Run,
        Jump,
        DoubleJump,
        Fall,
        /// <summary>Sprint 23.4 — slide / duck locomotion.</summary>
        Slide,
        Win,
        Lose,
        Celebrate
    }
}

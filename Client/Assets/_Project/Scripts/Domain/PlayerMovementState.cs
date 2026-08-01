namespace GulfRun.Domain
{
    /// <summary>
    /// High-level movement state of a runner-style player character.
    /// Pure data — no engine dependency — so it can be resolved identically
    /// on client and (future) server for multiplayer reconciliation.
    /// </summary>
    public enum PlayerMovementState
    {
        Idle,
        Running,
        Jumping,
        Falling,
        Landing,

        /// <summary>Race-start countdown (3-2-1-GO) is active; the player is held in place.</summary>
        Countdown,

        /// <summary>Airborne on the second (final) jump, as distinct from the first <see cref="Jumping"/>.</summary>
        DoubleJumping,

        /// <summary>The run has ended; the player no longer responds to input.</summary>
        GameOver
    }
}

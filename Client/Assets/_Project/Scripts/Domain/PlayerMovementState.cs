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
        Landing
    }
}

namespace GulfRun.Domain
{
    /// <summary>
    /// Top-level state of a single endless-runner session. Pure data — the
    /// same enum can drive a future authoritative server's session state for
    /// multiplayer synchronization.
    /// </summary>
    public enum GameLoopState
    {
        Ready,
        Running,
        Paused,
        GameOver,
        Restart
    }
}

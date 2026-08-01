namespace GulfRun.Domain
{
    /// <summary>
    /// Lifecycle state of an online match/lobby, as distinct from the
    /// single-player <see cref="GameLoopState"/>. Pure data — no engine
    /// dependency — so a future authoritative session server can drive the
    /// exact same state machine as the client.
    /// </summary>
    public enum MatchState
    {
        /// <summary>Lobby/waiting room: players joining and readying up.</summary>
        Waiting,

        /// <summary>Shared 3-2-1-GO countdown; identical on every connected client.</summary>
        Countdown,

        /// <summary>Race in progress.</summary>
        Running,

        /// <summary>Race has ended; results are being resolved/displayed.</summary>
        Finished,

        /// <summary>Local client has lost its connection to the match.</summary>
        Disconnected
    }
}

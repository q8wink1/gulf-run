namespace GulfRun.Domain
{
    /// <summary>
    /// Health of a single participant's network connection, tracked
    /// independently of their <see cref="PlayerReadyState"/> or match
    /// standing so a temporarily-degraded link never has to imply the
    /// player left the match.
    /// </summary>
    public enum ConnectionState
    {
        Connecting,
        Connected,

        /// <summary>No data received within the configured timeout window; not yet removed from the match.</summary>
        TimedOut,

        /// <summary>Was <see cref="TimedOut"/> and is receiving data again.</summary>
        Reconnecting,

        Disconnected
    }
}

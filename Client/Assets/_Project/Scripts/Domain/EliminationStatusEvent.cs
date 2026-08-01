namespace GulfRun.Domain
{
    /// <summary>Host-authoritative broadcast of one player's live elimination standing (the "Show warning" / countdown requirement).</summary>
    public readonly struct EliminationStatusEvent
    {
        public readonly int ConnectionId;
        public readonly EliminationStatus Status;

        /// <summary>Whole seconds left before automatic elimination while <see cref="Status"/> is <see cref="EliminationStatus.Warning"/>; 0 otherwise.</summary>
        public readonly int WarningSecondsRemaining;
        public readonly double TimestampSeconds;

        public EliminationStatusEvent(int connectionId, EliminationStatus status, int warningSecondsRemaining, double timestampSeconds)
        {
            ConnectionId = connectionId;
            Status = status;
            WarningSecondsRemaining = warningSecondsRemaining;
            TimestampSeconds = timestampSeconds;
        }
    }
}

namespace GulfRun.Domain
{
    /// <summary>
    /// A periodic "how far along am I" report a client sends to the host:
    /// distance traveled and coins collected so far this race. The host-only
    /// <c>RaceFinishAuthority</c> is the sole consumer — it uses these reports
    /// to detect finish-line crossings and to compute the elimination gap,
    /// exactly the same "client reports, host decides" shape every other
    /// Sprint 5/6 network message already uses (e.g. <see cref="TrapTriggerEvent"/>).
    /// Deliberately separate from <see cref="NetworkPlayerSnapshot"/> (which
    /// exists purely for render interpolation) so race-outcome logic never
    /// depends on visual-sync cadence/precision.
    /// </summary>
    public readonly struct RaceProgressReport
    {
        public readonly int ConnectionId;
        public readonly float DistanceMeters;
        public readonly int CoinsCollected;
        public readonly double TimestampSeconds;

        public RaceProgressReport(int connectionId, float distanceMeters, int coinsCollected, double timestampSeconds)
        {
            ConnectionId = connectionId;
            DistanceMeters = distanceMeters;
            CoinsCollected = coinsCollected;
            TimestampSeconds = timestampSeconds;
        }
    }
}

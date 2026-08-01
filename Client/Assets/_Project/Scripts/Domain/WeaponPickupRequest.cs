namespace GulfRun.Domain
{
    /// <summary>
    /// Client -> authority "I touched this Item Box" ask. Carries no rolled
    /// weapon — only the authority (host) decides what (if anything) is
    /// granted, via <see cref="WeaponPickupEvent"/>, so a client can never
    /// forge a favorable roll.
    /// </summary>
    public readonly struct WeaponPickupRequest
    {
        /// <summary>Stable per-instance identifier for the touched box (its GameObject instance ID).</summary>
        public readonly int BoxId;
        public readonly int CollectorConnectionId;
        public readonly double TimestampSeconds;

        public WeaponPickupRequest(int boxId, int collectorConnectionId, double timestampSeconds)
        {
            BoxId = boxId;
            CollectorConnectionId = collectorConnectionId;
            TimestampSeconds = timestampSeconds;
        }
    }
}

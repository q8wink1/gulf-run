namespace GulfRun.Domain
{
    /// <summary>
    /// Authority -> everyone: the validated outcome of a <see cref="WeaponPickupRequest"/>.
    /// <see cref="Granted"/> is false when the collector's inventory was
    /// already full — "the player CANNOT collect another Item Box. The Item
    /// Box is lost" — the box is still consumed (it despawns for everyone),
    /// but no weapon is added to any inventory.
    /// </summary>
    public readonly struct WeaponPickupEvent
    {
        public readonly int BoxId;
        public readonly WeaponId Weapon;
        public readonly bool Granted;
        public readonly int CollectorConnectionId;
        public readonly double TimestampSeconds;

        public WeaponPickupEvent(int boxId, WeaponId weapon, bool granted, int collectorConnectionId, double timestampSeconds)
        {
            BoxId = boxId;
            Weapon = weapon;
            Granted = granted;
            CollectorConnectionId = collectorConnectionId;
            TimestampSeconds = timestampSeconds;
        }
    }
}

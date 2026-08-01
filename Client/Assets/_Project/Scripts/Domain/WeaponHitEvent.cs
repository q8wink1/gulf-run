namespace GulfRun.Domain
{
    /// <summary>
    /// A single resolved "weapon X, thrown by A, hits B" outcome. Used both
    /// for a client-reported hit candidate (the seam for a future
    /// collision-based prediction system, once remote avatars are physically
    /// networked) and for the authority's confirmed broadcast — every
    /// connected client reacts identically to the confirmed event, applying
    /// the resulting <see cref="PlayerStatusEffect"/> to B only.
    /// </summary>
    public readonly struct WeaponHitEvent
    {
        public readonly WeaponId Weapon;
        public readonly int AttackerConnectionId;
        public readonly int TargetConnectionId;
        public readonly double TimestampSeconds;

        public WeaponHitEvent(WeaponId weapon, int attackerConnectionId, int targetConnectionId, double timestampSeconds)
        {
            Weapon = weapon;
            AttackerConnectionId = attackerConnectionId;
            TargetConnectionId = targetConnectionId;
            TimestampSeconds = timestampSeconds;
        }
    }
}

namespace GulfRun.Domain
{
    /// <summary>
    /// Host-authoritative "a trap has appeared" broadcast — there is no
    /// client request counterpart (unlike <see cref="WeaponPickupRequest"/>):
    /// traps belong to the map, so only <c>Features.Traps.Authority.TrapAuthority</c>
    /// ever decides to spawn one; every client (including the host's own
    /// scene) reacts identically to this one event. <see cref="TrapInstanceId"/>
    /// is a host-minted counter, not a pooled GameObject's instance id, so
    /// every connected client can refer to the same logical trap.
    /// </summary>
    public readonly struct TrapSpawnEvent
    {
        public readonly int TrapInstanceId;
        public readonly TrapId Trap;
        public readonly NetVector2 Position;
        public readonly double LifetimeSeconds;
        public readonly double TimestampSeconds;

        public TrapSpawnEvent(int trapInstanceId, TrapId trap, NetVector2 position, double lifetimeSeconds, double timestampSeconds)
        {
            TrapInstanceId = trapInstanceId;
            Trap = trap;
            Position = position;
            LifetimeSeconds = lifetimeSeconds;
            TimestampSeconds = timestampSeconds;
        }
    }
}

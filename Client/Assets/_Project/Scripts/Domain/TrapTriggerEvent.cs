namespace GulfRun.Domain
{
    /// <summary>
    /// Both the client's candidate "a player touched this active trap"
    /// report and the host's validated confirmation of the same event —
    /// exactly the dual role <see cref="WeaponHitEvent"/> plays for weapons.
    /// Every player can be affected equally by the same
    /// <see cref="TrapInstanceId"/> (traps have no owner and grant no
    /// immunity), so unlike a weapon hit, more than one confirmed trigger
    /// per trap instance is expected and valid.
    /// </summary>
    public readonly struct TrapTriggerEvent
    {
        public readonly int TrapInstanceId;
        public readonly TrapId Trap;
        public readonly int TargetConnectionId;
        public readonly double TimestampSeconds;

        public TrapTriggerEvent(int trapInstanceId, TrapId trap, int targetConnectionId, double timestampSeconds)
        {
            TrapInstanceId = trapInstanceId;
            Trap = trap;
            TargetConnectionId = targetConnectionId;
            TimestampSeconds = timestampSeconds;
        }
    }
}

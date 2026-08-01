namespace GulfRun.Domain
{
    /// <summary>
    /// What kind of thing caused a <see cref="PlayerStatusEffect"/> — a
    /// player-carried <see cref="WeaponId"/> (Sprint 5) or a map-owned
    /// <see cref="TrapId"/> (Sprint 6). Lets both features share the exact
    /// same effect vocabulary (<see cref="WeaponEffectFlags"/>) and
    /// application pipeline (<c>IPlayerStatusEffectReceiver</c>) without
    /// either one owning the other's identifier type.
    /// </summary>
    public enum EffectSourceKind
    {
        Weapon,
        Trap
    }
}

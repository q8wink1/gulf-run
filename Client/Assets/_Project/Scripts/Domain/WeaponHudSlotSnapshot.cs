namespace GulfRun.Domain
{
    /// <summary>One weapon inventory slot as the Race HUD needs it (identity + uses + cooldown).</summary>
    public readonly struct WeaponHudSlotSnapshot
    {
        public readonly WeaponId? Weapon;
        public readonly int UsesRemaining;
        public readonly float CooldownRemaining01;
        public readonly bool JustPickedUp;

        public WeaponHudSlotSnapshot(WeaponId? weapon, int usesRemaining, float cooldownRemaining01, bool justPickedUp)
        {
            Weapon = weapon;
            UsesRemaining = usesRemaining;
            CooldownRemaining01 = cooldownRemaining01;
            JustPickedUp = justPickedUp;
        }

        public bool IsEmpty => !Weapon.HasValue;
    }
}

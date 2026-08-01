namespace GulfRun.Domain
{
    /// <summary>
    /// A resolved, ready-to-apply gameplay effect targeting one player —
    /// the output of <see cref="WeaponEffectResolver"/>. Pure data so the
    /// exact same resolution can run identically on a future dedicated
    /// server and on every client.
    /// </summary>
    public readonly struct PlayerStatusEffect
    {
        public readonly WeaponEffectFlags Flags;
        public readonly double DurationSeconds;
        public readonly float Magnitude;
        public readonly WeaponId SourceWeapon;

        public PlayerStatusEffect(WeaponEffectFlags flags, double durationSeconds, float magnitude, WeaponId sourceWeapon)
        {
            Flags = flags;
            DurationSeconds = durationSeconds;
            Magnitude = magnitude;
            SourceWeapon = sourceWeapon;
        }

        public bool Has(WeaponEffectFlags flag) => (Flags & flag) != 0;
    }
}

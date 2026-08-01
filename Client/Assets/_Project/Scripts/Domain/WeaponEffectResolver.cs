namespace GulfRun.Domain
{
    /// <summary>
    /// Pure resolution of a weapon hit into the <see cref="PlayerStatusEffect"/>
    /// actually applied to the target — the single place the Falcon Feather
    /// "Mark" bonus (increased effectiveness of the next successful attack
    /// against a marked opponent) is implemented, so every weapon benefits
    /// from it identically instead of each weapon special-casing Mark itself.
    /// </summary>
    public static class WeaponEffectResolver
    {
        private const float MarkedMagnitudeMultiplier = 1.5f;
        private const double MarkedDurationMultiplier = 1.5d;

        public static PlayerStatusEffect Resolve(
            WeaponId sourceWeapon,
            WeaponEffectFlags flags,
            float baseMagnitude,
            double baseDurationSeconds,
            bool targetIsMarked)
        {
            float magnitude = targetIsMarked ? baseMagnitude * MarkedMagnitudeMultiplier : baseMagnitude;
            double duration = targetIsMarked ? baseDurationSeconds * MarkedDurationMultiplier : baseDurationSeconds;
            return new PlayerStatusEffect(flags, duration, magnitude, sourceWeapon);
        }
    }
}

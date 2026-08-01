namespace GulfRun.Domain
{
    /// <summary>
    /// How a weapon resolves its target(s) on activation. Purely descriptive
    /// data read from a weapon's definition — the actual resolution (who,
    /// how many) is performed by the host-authoritative
    /// <c>Features.Weapons.Authority.WeaponAuthority</c>, never by Domain.
    /// </summary>
    public enum WeaponTargetingType
    {
        /// <summary>Single target: whichever opponent is closest to the user.</summary>
        NearestOpponent,

        /// <summary>Every opponent within the effect's area (e.g. a lingering cloud or hazard).</summary>
        AreaEffect,

        /// <summary>Every opponent ahead of the user along the track (e.g. a charge).</summary>
        Forward,

        /// <summary>Applies to the user, cancelling/blocking an incoming effect (e.g. a shield).</summary>
        Defensive,

        /// <summary>Applies to the user only, with no opponent involved (e.g. a speed boost).</summary>
        SelfBuff
    }
}

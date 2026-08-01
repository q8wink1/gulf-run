using System;

namespace GulfRun.Domain
{
    /// <summary>
    /// The gameplay effect(s) a weapon applies to its target(s). A bit-flag
    /// enum (not a single value) because some weapons combine effects (e.g.
    /// Sand Storm = Slow + VisionReduced) — new combinations are a data
    /// change on a <c>WeaponDefinition</c> asset, never a new code branch,
    /// satisfying "no hardcoded values" / "easy future expansion".
    ///
    /// Every flag here is intentionally temporary/reversible — none of them
    /// remove a player from the match or end their run, satisfying "Weapons
    /// must never permanently eliminate players".
    /// </summary>
    [Flags]
    public enum WeaponEffectFlags
    {
        None = 0,

        /// <summary>Reduces movement speed for the effect duration.</summary>
        Slow = 1 << 0,

        /// <summary>Reduces the affected player's vision (presentation-layer concern).</summary>
        VisionReduced = 1 << 1,

        /// <summary>Freezes the affected player's controls for the duration (comedic pause, e.g. Arabic Coffee).</summary>
        Pause = 1 << 2,

        /// <summary>Increases movement speed for the effect duration.</summary>
        SpeedBoost = 1 << 3,

        /// <summary>Short, hard stun — no movement, no input.</summary>
        Stun = 1 << 4,

        /// <summary>Grants a shield that blocks exactly one subsequent negative effect, then is consumed.</summary>
        Shield = 1 << 5,

        /// <summary>Reduces traction/control briefly (e.g. Oil Spill).</summary>
        TractionLoss = 1 << 6,

        /// <summary>Immediately clears all active negative effects on the target.</summary>
        Cleanse = 1 << 7,

        /// <summary>Marks the target: their next confirmed hit receives a bonus (see Falcon Feather / WeaponEffectResolver).</summary>
        Mark = 1 << 8,

        /// <summary>Strong knockdown — longer, harder stun with dedicated presentation (e.g. Royal Camel Charge).</summary>
        Knockdown = 1 << 9
    }
}

using System;

namespace GulfRun.Domain
{
    /// <summary>
    /// The gameplay effect(s) a weapon (Sprint 5) or map trap (Sprint 6)
    /// applies to its target(s). A bit-flag enum (not a single value)
    /// because some sources combine effects (e.g. Sand Storm = Slow +
    /// VisionReduced, Dust Tornado = VisionReduced + LateralPush) — new
    /// combinations are a data change on a <c>WeaponDefinition</c>/
    /// <c>TrapDefinition</c> asset, never a new code branch, satisfying "no
    /// hardcoded values" / "easy future expansion". Deliberately a single
    /// shared vocabulary (rather than a parallel <c>TrapEffectFlags</c>) so
    /// both features drive the exact same
    /// <c>IPlayerStatusEffectReceiver</c>/<c>PlayerStatusEffectController</c>
    /// pipeline with zero duplicated effect-application logic — see
    /// <see cref="EffectSourceKind"/> for how a <see cref="PlayerStatusEffect"/>
    /// still records which feature actually caused it.
    ///
    /// Every flag here is intentionally temporary/reversible — none of them
    /// remove a player from the match or end their run, satisfying "Weapons
    /// must never permanently eliminate players" (Sprint 5) and traps never
    /// permanently eliminating players either (Sprint 6).
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

        /// <summary>Strong knockdown — longer, harder stun with dedicated presentation (e.g. Royal Camel Charge, Angry Camel).</summary>
        Knockdown = 1 << 9,

        /// <summary>
        /// Sprint 6: an instantaneous positional setback (e.g. Wind Gust,
        /// Dust Tornado). Applied once, immediately, by
        /// <c>PlayerMotor.ApplyLateralImpulse</c> — unlike every other flag
        /// here it is never added to a duration-based active-effect list
        /// (there is nothing to "recompute" every frame for a one-shot push).
        /// </summary>
        LateralPush = 1 << 10
    }
}

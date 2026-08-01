namespace GulfRun.Domain
{
    /// <summary>
    /// Identity of one of the 10 Sprint 5 weapons (9 Standard + 1 Legendary).
    /// Pure data — the numeric tuning per weapon (magnitude, duration, spawn
    /// weight, targeting type, ...) lives in the data-driven
    /// <c>Features.Weapons.Configuration.WeaponDefinition</c> asset, never
    /// hardcoded here, so adding an 11th weapon is a data change, not a code
    /// change (see Sprint 5 "Easy future expansion" requirement).
    /// </summary>
    public enum WeaponId
    {
        SandStorm,
        DustCloud,
        ArabicCoffee,
        DesertBoost,
        FlyingAgal,
        ProtectionShield,
        OilSpill,
        DateEnergy,
        FalconFeather,

        /// <summary>The single Legendary weapon. Extremely low spawn chance; at most one grant per match.</summary>
        RoyalCamelCharge
    }
}

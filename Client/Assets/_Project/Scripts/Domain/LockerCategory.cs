namespace GulfRun.Domain
{
    /// <summary>
    /// Locker navigation categories from the Sprint 16 brief. Mapped to
    /// <see cref="CosmeticSlot"/> (or the character grid) by
    /// <see cref="LockerCategoryMapping"/> — keeping the UI vocabulary
    /// separate from the equip-slot enum so Headwear can label <see cref="CosmeticSlot.Hat"/>
    /// without breaking Sprint 8 asset ordinals.
    /// </summary>
    public enum LockerCategory
    {
        Characters,
        Outfits,
        Headwear,
        Glasses,
        VictoryPoses,
        Emotes,
        FootstepEffects,
        RunningEffects,
        ProfileFrames,
        Titles
    }
}

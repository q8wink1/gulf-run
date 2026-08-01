namespace GulfRun.Domain
{
    /// <summary>
    /// Every equip slot the Customization system supports. Sprint 8 shipped
    /// Outfit (+ example Hat/VictoryPose/Emote). Sprint 16 appends Locker
    /// categories that need real equip slots (Footstep/Running Effects,
    /// Profile Frame, Title) — new members are always appended so existing
    /// <c>CosmeticCatalogConfig.asset</c> ordinals stay valid.
    /// </summary>
    public enum CosmeticSlot
    {
        Outfit = 0,
        Hat = 1,
        Glasses = 2,
        Shoes = 3,
        Accessory = 4,
        BackItem = 5,
        Trail = 6,
        Pet = 7,
        VictoryPose = 8,
        Emote = 9,
        FootstepEffect = 10,
        RunningEffect = 11,
        ProfileFrame = 12,
        Title = 13
    }
}

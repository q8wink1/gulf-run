namespace GulfRun.Domain
{
    /// <summary>
    /// Every equip slot the Customization system supports. Only
    /// <see cref="Outfit"/> ships with real, ownable content in Sprint 8
    /// (the free Traditional Outfits plus a handful of example premium
    /// items); every other member exists purely so the "Future Support"
    /// architecture (Hats, Glasses, Shoes, Accessories, Victory Poses,
    /// Emotes, Back Items, Trails, Pets — brief §"FUTURE SUPPORT") is real,
    /// compiled, and network-synced today, with zero code changes required
    /// to light up new items in any of these slots later — only new
    /// <c>Features.Character.Configuration.CosmeticCatalogConfig</c> entries.
    /// </summary>
    public enum CosmeticSlot
    {
        Outfit,
        Hat,
        Glasses,
        Shoes,
        Accessory,
        BackItem,
        Trail,
        Pet,
        VictoryPose,
        Emote
    }
}

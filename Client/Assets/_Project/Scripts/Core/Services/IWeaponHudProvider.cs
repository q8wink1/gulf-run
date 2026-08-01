using System;
using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Local weapon slots for Features.RaceHud. Implemented by
    /// <c>Features.Weapons.Inventory.WeaponInventoryManager</c>.
    /// </summary>
    public interface IWeaponHudProvider
    {
        IReadOnlyList<WeaponHudSlotSnapshot> LocalSlots { get; }
        event Action InventoryChanged;
    }

    public static class WeaponHudService
    {
        public static IWeaponHudProvider Current { get; set; }
    }
}

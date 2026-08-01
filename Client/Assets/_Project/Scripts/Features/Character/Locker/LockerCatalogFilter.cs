using System;
using System.Collections.Generic;
using GulfRun.Domain;
using GulfRun.Features.Character.Configuration;
using GulfRun.Features.Character.Loadout;

namespace GulfRun.Features.Character.Locker
{
    /// <summary>
    /// Pure-ish filter/search/sort for the Locker grid — keeps query rules
    /// out of the OnGUI view (SOLID: single responsibility).
    /// </summary>
    public static class LockerCatalogFilter
    {
        private static readonly List<CosmeticCatalogConfig.CosmeticEntry> Scratch = new List<CosmeticCatalogConfig.CosmeticEntry>(64);

        public static IReadOnlyList<CosmeticCatalogConfig.CosmeticEntry> Query(
            PlayerLoadoutManager manager,
            CosmeticSlot slot,
            LockerOwnershipFilter ownershipFilter,
            LockerSortMode sortMode,
            string searchText,
            GulfCountry playerCountry)
        {
            Scratch.Clear();
            if (manager == null || manager.CosmeticCatalog == null)
            {
                return Scratch;
            }

            string search = searchText != null ? searchText.Trim() : string.Empty;
            bool hasSearch = search.Length > 0;
            string searchLower = hasSearch ? search.ToLowerInvariant() : string.Empty;

            IReadOnlyList<CosmeticCatalogConfig.CosmeticEntry> bySlot = manager.CosmeticCatalog.GetBySlot(slot);
            for (int i = 0; i < bySlot.Count; i++)
            {
                CosmeticCatalogConfig.CosmeticEntry entry = bySlot[i];
                if (entry == null)
                {
                    continue;
                }

                if (entry.IsTraditionalOutfit && entry.RequiredCountry != playerCountry)
                {
                    continue;
                }

                if (!PassesOwnership(manager, entry, ownershipFilter, playerCountry))
                {
                    continue;
                }

                if (hasSearch && !MatchesSearch(entry, searchLower, slot))
                {
                    continue;
                }

                Scratch.Add(entry);
            }

            Scratch.Sort((a, b) => Compare(a, b, sortMode));
            return Scratch;
        }

        private static bool PassesOwnership(
            PlayerLoadoutManager manager,
            CosmeticCatalogConfig.CosmeticEntry entry,
            LockerOwnershipFilter filter,
            GulfCountry playerCountry)
        {
            bool owned = manager.LocalInventory.Owns(entry.Id);
            bool permanent = manager.LocalInventory.OwnsPermanently(entry.Id);
            bool temporary = manager.LocalInventory.OwnsTemporarily(entry.Id);

            switch (filter)
            {
                case LockerOwnershipFilter.All:
                    return true;
                case LockerOwnershipFilter.Owned:
                    return owned;
                case LockerOwnershipFilter.NotOwned:
                    return !owned;
                case LockerOwnershipFilter.Temporary:
                    return temporary;
                case LockerOwnershipFilter.Permanent:
                    return permanent;
                case LockerOwnershipFilter.Country:
                    return entry.CountryTagged && entry.RequiredCountry == playerCountry;
                default:
                    return true;
            }
        }

        private static bool MatchesSearch(CosmeticCatalogConfig.CosmeticEntry entry, string searchLower, CosmeticSlot slot)
        {
            if (entry.DisplayName.ToLowerInvariant().Contains(searchLower))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(entry.CollectionTag) && entry.CollectionTag.ToLowerInvariant().Contains(searchLower))
            {
                return true;
            }

            if (slot.ToString().ToLowerInvariant().Contains(searchLower))
            {
                return true;
            }

            if (entry.CountryTagged && entry.RequiredCountry.ToString().ToLowerInvariant().Contains(searchLower))
            {
                return true;
            }

            return false;
        }

        private static int Compare(CosmeticCatalogConfig.CosmeticEntry a, CosmeticCatalogConfig.CosmeticEntry b, LockerSortMode sortMode)
        {
            if (sortMode == LockerSortMode.Rarity)
            {
                int rarity = ((int)b.Rarity).CompareTo((int)a.Rarity);
                if (rarity != 0)
                {
                    return rarity;
                }
            }

            int newest = b.CatalogIndex.CompareTo(a.CatalogIndex);
            if (newest != 0)
            {
                return newest;
            }

            return string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase);
        }
    }
}

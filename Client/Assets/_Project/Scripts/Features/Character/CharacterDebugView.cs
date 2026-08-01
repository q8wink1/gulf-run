using System;
using GulfRun.Core.Managers;
using GulfRun.Domain;
using GulfRun.Features.Character.Configuration;
using GulfRun.Features.Character.Loadout;
using UnityEngine;

namespace GulfRun.Features.Character
{
    /// <summary>
    /// Debug panel required by the Sprint 8 brief: Character ID, Country ID,
    /// Current Outfit, Loaded Cosmetics. Same on-screen, dev-build-only
    /// placeholder style as <c>RaceFinishDebugView</c>/<c>TrapsDebugView</c>.
    /// </summary>
    public sealed class CharacterDebugView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private bool showOnScreenDebug = true;
        [SerializeField] private int panelX = 10;
        [SerializeField] private int panelY = 10;

        private static readonly CosmeticSlot[] AllSlots = (CosmeticSlot[])Enum.GetValues(typeof(CosmeticSlot));

        private void OnGUI()
        {
            if (!showOnScreenDebug)
            {
                return;
            }

            int y = panelY;
            const int lineHeight = 18;
            const int width = 420;

            void Line(string text)
            {
                GUI.Label(new Rect(panelX, y, width, lineHeight), text);
                y += lineHeight;
            }

            PlayerLoadoutManager manager = PlayerLoadoutManager.Instance;
            bool hasAccount = SaveManager.Instance != null && SaveManager.Instance.HasAccount;

            Line($"[Character] Has Account: {hasAccount}");

            if (manager == null || manager.LocalLoadout == null)
            {
                Line("[Character] Loadout not initialized yet.");
                return;
            }

            PlayerLoadout loadout = manager.LocalLoadout;
            CharacterDefinition character = manager.CharacterCatalog != null ? manager.CharacterCatalog.GetDefinition(loadout.Character) : null;

            Line($"Character ID: {loadout.Character} ({(character != null ? character.DisplayName : "unknown")})");
            Line($"Country ID: {loadout.Country}");

            CosmeticId outfit = loadout.GetEquipped(CosmeticSlot.Outfit);
            Line($"Current Outfit: {ResolveName(manager, outfit)}");

            Line("Loaded Cosmetics:");
            for (int i = 0; i < AllSlots.Length; i++)
            {
                CosmeticId equipped = loadout.GetEquipped(AllSlots[i]);
                Line($"  {AllSlots[i]}: {(equipped.IsNone ? "(none)" : ResolveName(manager, equipped))}");
            }

            Line($"Gems: {(EconomyManager.Instance != null ? EconomyManager.Instance.Gems : 0)}  Owned Cosmetics: {manager.LocalInventory.OwnedIds.Count}");
            Line($"Remote Loadouts Tracked: {manager.RemoteLoadouts.Count}");
        }

        private static string ResolveName(PlayerLoadoutManager manager, CosmeticId id)
        {
            if (id.IsNone)
            {
                return "(none)";
            }

            return manager.CosmeticCatalog != null && manager.CosmeticCatalog.TryGetEntry(id, out CosmeticCatalogConfig.CosmeticEntry entry)
                ? entry.DisplayName
                : id.ToString();
        }
#endif
    }
}

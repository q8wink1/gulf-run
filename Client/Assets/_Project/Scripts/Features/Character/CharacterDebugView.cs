using System;
using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Character.Configuration;
using GulfRun.Features.Character.Loadout;
using UnityEngine;

namespace GulfRun.Features.Character
{
    /// <summary>
    /// Debug panel: Character ID, Outfit ID, Animation State, Country ID,
    /// Temporary Timer. Sprint 16 keeps the Character feature panel at its
    /// established <c>panelX: 10</c> (first slot); the next free slot after
    /// MainMenu's 4060 is 4510 for any future non-Character panel.
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
            if (!showOnScreenDebug || !PersistentUiScope.AllowsPersistentDebugOverlay)
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

            Line($"[Character/Locker] Has Account: {hasAccount}");

            if (manager == null || manager.LocalLoadout == null)
            {
                Line("[Character] Loadout not initialized yet.");
                return;
            }

            PlayerLoadout loadout = manager.LocalLoadout;
            CharacterDefinition character = manager.CharacterCatalog != null ? manager.CharacterCatalog.GetDefinition(loadout.Character) : null;

            Line($"Character ID: {loadout.Character} ({(character != null ? character.DisplayName : "unknown")})");
            Line($"Country ID: {loadout.Country}");
            Line($"Animation State: {manager.PreviewAnimationState}");

            CosmeticId outfit = loadout.GetEquipped(CosmeticSlot.Outfit);
            Line($"Outfit ID: {(outfit.IsNone ? "(none)" : outfit.Value)} — {ResolveName(manager, outfit)}");

            Line("Equipped Cosmetics:");
            for (int i = 0; i < AllSlots.Length; i++)
            {
                CosmeticId equipped = loadout.GetEquipped(AllSlots[i]);
                if (equipped.IsNone)
                {
                    continue;
                }

                Line($"  {AllSlots[i]}: {equipped.Value}");
            }

            Line($"Gems: {(EconomyManager.Instance != null ? EconomyManager.Instance.Gems : 0)}  Permanent: {manager.LocalInventory.OwnedIds.Count}  Temporary: {manager.LocalInventory.TemporaryOwnedIds.Count}");

            double now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            foreach (string idValue in manager.LocalInventory.TemporaryOwnedIds)
            {
                var id = new CosmeticId(idValue);
                if (manager.LocalInventory.TryGetTemporaryExpiry(id, out double expires))
                {
                    double remaining = expires - now;
                    int total = remaining > 0 ? (int)remaining : 0;
                    Line($"Temp Timer [{idValue}]: {total / 86400}d {(total % 86400) / 3600}h {(total % 3600) / 60}m");
                }
            }

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

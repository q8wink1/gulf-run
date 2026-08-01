using System.Collections.Generic;
using GulfRun.Core.Networking;
using GulfRun.Domain;
using GulfRun.Features.Weapons.Configuration;
using GulfRun.Features.Weapons.Inventory;
using UnityEngine;

namespace GulfRun.Features.Weapons
{
    /// <summary>
    /// Debug overlay: Current Inventory, Weapon IDs, Spawn Rate, Legendary
    /// Spawn Chance, Current Weapon State — plus buttons to exercise the
    /// pickup/use flow end-to-end without a physical Item Box collision
    /// (same OnGUI-placeholder approach as MultiplayerDebugView/RunnerDebugView,
    /// no menu/HUD UI exists yet). Placed further right than
    /// MultiplayerDebugView's panel so both can be shown at once.
    /// </summary>
    public sealed class WeaponsDebugView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private bool showOnScreenDebug = true;
        [SerializeField] private int panelX = 910;
        [SerializeField] private WeaponCatalogConfig catalog;

        private int _syntheticBoxId = -1000;

        private void OnGUI()
        {
            if (!showOnScreenDebug)
            {
                return;
            }

            int y = 10;
            const int lineHeight = 18;
            const int width = 420;

            void Line(string text)
            {
                GUI.Label(new Rect(panelX, y, width, lineHeight), text);
                y += lineHeight;
            }

            IMatchTransport transport = MatchTransportService.Current;
            WeaponInventoryManager inventory = WeaponInventoryManager.Instance;
            int localId = transport != null ? transport.LocalConnectionId : -1;

            Line("[Weapons] Catalog: " + (catalog != null ? catalog.Weapons.Count : 0) + " weapons");
            Line($"Legendary Spawn Chance: {(catalog != null ? catalog.LegendarySpawnChance01 : 0f) * 100f:F1}%");

            if (catalog != null)
            {
                float totalWeight = 0f;
                foreach (WeightedOption<WeaponId> option in catalog.GetStandardWeightedOptions())
                {
                    totalWeight += option.Weight;
                }

                foreach (WeightedOption<WeaponId> option in catalog.GetStandardWeightedOptions())
                {
                    float rate = totalWeight > 0f ? option.Weight / totalWeight * 100f : 0f;
                    Line($"  [{option.Value}] Standard spawn rate: {rate:F1}%");
                }

                Line($"  [{catalog.LegendaryWeaponId}] Legendary");
            }

            y += 6;
            Line($"Current Weapon State (local conn {localId}):");

            if (inventory != null)
            {
                IReadOnlyList<WeaponId?> slots = inventory.GetSlots(localId);
                for (int i = 0; i < slots.Count; i++)
                {
                    string label = slots[i].HasValue ? slots[i].Value.ToString() : "(empty)";
                    Line($"  Slot {i + 1}: {label}");
                }

                Line($"  Inventory Full: {inventory.IsFull(localId)}  Cooldown: {(inventory.IsOnCooldown(localId) ? "Active" : "Ready")}");
            }

            y += 6;
            DrawControls(transport, inventory, ref y, width);
        }

        private void DrawControls(IMatchTransport transport, WeaponInventoryManager inventory, ref int y, int width)
        {
            const int buttonHeight = 24;
            const int buttonWidth = 260;

            if (transport == null)
            {
                return;
            }

            if (GUI.Button(new Rect(panelX, y, buttonWidth, buttonHeight), "Simulate Item Box Pickup"))
            {
                _syntheticBoxId--;
                transport.RequestWeaponPickup(new WeaponPickupRequest(_syntheticBoxId, transport.LocalConnectionId, Time.timeAsDouble));
            }

            y += buttonHeight + 4;

            if (inventory != null)
            {
                if (GUI.Button(new Rect(panelX, y, buttonWidth / 2 - 2, buttonHeight), "Use Slot 1"))
                {
                    inventory.TryUseLocalSlot(0);
                }

                if (GUI.Button(new Rect(panelX + buttonWidth / 2 + 2, y, buttonWidth / 2 - 2, buttonHeight), "Use Slot 2"))
                {
                    inventory.TryUseLocalSlot(1);
                }

                y += buttonHeight + 4;
            }

            _ = width;
        }
#endif
    }
}

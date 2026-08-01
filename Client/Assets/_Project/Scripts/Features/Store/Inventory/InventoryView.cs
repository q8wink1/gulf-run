using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Store.Inventory
{
    /// <summary>
    /// The Inventory screen from the Sprint 10 brief: "Display all owned:
    /// Characters, Skins, Outfits, Emotes, Victory Poses, Effects." Reads
    /// exclusively through <see cref="InventoryManager"/> — never a second
    /// copy of ownership state.
    /// </summary>
    public sealed class InventoryView : SceneSingleton<InventoryView>
    {
        private bool _open;
        private Vector2 _scroll;
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;

        private void OnGUI()
        {
            EnsureStyles();

            if (GUI.Button(new Rect(1230, 10, 140, 34), _open ? "Close Inventory" : "Inventory"))
            {
                _open = !_open;
            }

            if (!_open)
            {
                return;
            }

            DrawPanel();
        }

        private void DrawPanel()
        {
            const float panelWidth = 420f;
            const float panelHeight = 460f;
            float x = 1230f;
            float y = 50f;

            GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty);
            GUI.Label(new Rect(x + 14f, y + 8f, panelWidth - 28f, 24f), "INVENTORY", _titleStyle);

            InventoryManager inventory = InventoryManager.Instance;
            if (inventory == null)
            {
                return;
            }

            float rowY = y + 40f;
            GUI.Label(new Rect(x + 14f, rowY, panelWidth - 28f, 20f), "Characters: " + inventory.UnlockedCharacterCount + "/" + inventory.TotalLaunchCharacterCount + " unlocked", _labelStyle);
            rowY += 26f;

            IReadOnlyList<CosmeticId> cosmetics = inventory.GetOwnedCosmetics();
            IReadOnlyList<OwnedStoreItem> storeItems = inventory.GetOwnedStoreItems();

            Rect viewport = new Rect(x + 14f, rowY, panelWidth - 28f, y + panelHeight - rowY - 14f);
            float rowH = 20f;
            Rect content = new Rect(0f, 0f, viewport.width - 20f, (cosmetics.Count + storeItems.Count + 2) * rowH);
            _scroll = GUI.BeginScrollView(viewport, _scroll, content);

            float cy = 0f;
            GUI.Label(new Rect(0f, cy, content.width, rowH), "— Outfits / Emotes / Victory Poses —", _labelStyle);
            cy += rowH;
            for (int i = 0; i < cosmetics.Count; i++)
            {
                GUI.Label(new Rect(0f, cy, content.width, rowH), cosmetics[i].Value, _labelStyle);
                cy += rowH;
            }

            GUI.Label(new Rect(0f, cy, content.width, rowH), "— Visual Effects / Profile Frames / Other —", _labelStyle);
            cy += rowH;
            for (int i = 0; i < storeItems.Count; i++)
            {
                GUI.Label(new Rect(0f, cy, content.width, rowH), storeItems[i].ItemId.Value + " (" + storeItems[i].Section + ")", _labelStyle);
                cy += rowH;
            }

            GUI.EndScrollView();
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _titleStyle.normal.textColor = Color.white;

            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleLeft };
            _labelStyle.normal.textColor = Color.white;
        }
    }
}

using GulfRun.Core.Countries;
using GulfRun.Core.Managers;
using GulfRun.Domain;
using GulfRun.Features.Character.Configuration;
using GulfRun.Features.Character.Loadout;
using UnityEngine;

namespace GulfRun.Features.Character.Menu
{
    /// <summary>
    /// The Character Menu required by the Sprint 8 brief: Current Character,
    /// Current Country, Current Outfit, Owned/Locked Cosmetics (with Gem
    /// Price), and a Character Preview. Toggled with a corner button since
    /// no main-menu scene/UI exists yet in this project (same "OnGUI
    /// placeholder, real UI Toolkit screen later" posture as every prior
    /// sprint's UI — see docs/02-architecture/TECHNICAL_STACK.md).
    /// </summary>
    public sealed class CharacterMenuView : MonoBehaviour
    {
        [SerializeField] private CountryCatalogConfig countryCatalog;

        private bool _open;
        private GUIStyle _titleStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _labelStyle;

        private void OnGUI()
        {
            EnsureStyles();

            if (GUI.Button(new Rect(10, Screen.height - 46, 160, 34), _open ? "Close Character Menu" : "Character Menu"))
            {
                _open = !_open;
            }

            if (!_open)
            {
                return;
            }

            PlayerLoadoutManager manager = PlayerLoadoutManager.Instance;
            if (manager == null || manager.LocalLoadout == null)
            {
                GUI.Box(new Rect(10, Screen.height - 300, 420, 240), string.Empty);
                GUI.Label(new Rect(24, Screen.height - 284, 400, 24), "Create an account to unlock the Character Menu.", _labelStyle);
                return;
            }

            DrawPanel(manager);
        }

        private void DrawPanel(PlayerLoadoutManager manager)
        {
            const float panelWidth = 460f;
            const float panelHeight = 520f;
            float x = 10f;
            float y = Screen.height - panelHeight - 56f;

            GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty);
            GUI.Label(new Rect(x + 14f, y + 8f, panelWidth - 28f, 30f), "CHARACTER MENU", _titleStyle);

            float rowY = y + 44f;

            DrawCharacterPreview(manager, x + 14f, rowY);
            rowY += 130f;

            CharacterDefinition activeCharacter = manager.CharacterCatalog != null ? manager.CharacterCatalog.GetDefinition(manager.LocalLoadout.Character) : null;
            GUI.Label(new Rect(x + 14f, rowY, panelWidth - 28f, 20f), $"Current Character: {(activeCharacter != null ? activeCharacter.DisplayName : manager.LocalLoadout.Character.ToString())}", _labelStyle);
            rowY += 22f;

            if (GUI.Button(new Rect(x + 14f, rowY, 90f, 24f), "< Prev"))
            {
                CycleCharacter(manager, -1);
            }
            if (GUI.Button(new Rect(x + 110f, rowY, 90f, 24f), "Next >"))
            {
                CycleCharacter(manager, 1);
            }
            rowY += 32f;

            string countryName = countryCatalog != null && countryCatalog.TryGetEntry(manager.LocalLoadout.Country, out CountryCatalogConfig.CountryEntry entry)
                ? entry.DisplayName
                : manager.LocalLoadout.Country.ToString();
            GUI.Label(new Rect(x + 14f, rowY, panelWidth - 28f, 20f), $"Current Country: {countryName} (permanent)", _labelStyle);
            rowY += 22f;

            CosmeticId currentOutfit = manager.LocalLoadout.GetEquipped(CosmeticSlot.Outfit);
            string outfitName = ResolveDisplayName(manager, currentOutfit);
            GUI.Label(new Rect(x + 14f, rowY, panelWidth - 28f, 20f), $"Current Outfit: {outfitName}", _labelStyle);
            rowY += 22f;

            int gems = EconomyManager.Instance != null ? EconomyManager.Instance.Gems : 0;
            GUI.Label(new Rect(x + 14f, rowY, panelWidth - 28f, 20f), $"Gems: {gems}", _labelStyle);
            rowY += 26f;

            GUI.Label(new Rect(x + 14f, rowY, panelWidth - 28f, 20f), "Outfits (Owned / Locked):", _headerStyle);
            rowY += 22f;

            DrawOutfitList(manager, x + 14f, ref rowY, panelWidth - 28f);
        }

        private void DrawCharacterPreview(PlayerLoadoutManager manager, float x, float y)
        {
            CharacterDefinition definition = manager.CharacterCatalog != null ? manager.CharacterCatalog.GetDefinition(manager.LocalLoadout.Character) : null;

            Color previous = GUI.color;
            GUI.color = definition != null ? definition.PlaceholderColor : Color.gray;
            GUI.Box(new Rect(x, y, 110f, 110f), string.Empty);
            GUI.color = previous;

            GUI.Label(new Rect(x, y + 40f, 110f, 30f), definition != null ? definition.DisplayName : "—", _labelStyle);
        }

        private void DrawOutfitList(PlayerLoadoutManager manager, float x, ref float y, float width)
        {
            if (manager.CosmeticCatalog == null)
            {
                return;
            }

            CosmeticId equipped = manager.LocalLoadout.GetEquipped(CosmeticSlot.Outfit);

            foreach (CosmeticCatalogConfig.CosmeticEntry entry in manager.CosmeticCatalog.GetBySlot(CosmeticSlot.Outfit))
            {
                bool ownable = !entry.IsTraditionalOutfit || entry.RequiredCountry == manager.LocalLoadout.Country;
                if (!ownable)
                {
                    // Another country's Traditional Outfit — never ownable/wearable by this account.
                    continue;
                }

                bool owned = manager.LocalInventory.Owns(entry.Id);
                bool isEquipped = entry.Id == equipped;

                string status = isEquipped ? "EQUIPPED" : owned ? "Owned" : $"{entry.GemPrice} Gems";
                GUI.Label(new Rect(x, y, width - 170f, 20f), $"{entry.DisplayName} — {status}", _labelStyle);

                if (owned && !isEquipped)
                {
                    if (GUI.Button(new Rect(x + width - 160f, y - 2f, 80f, 22f), "Equip"))
                    {
                        manager.EquipCosmetic(CosmeticSlot.Outfit, entry.Id);
                    }
                }
                else if (!owned)
                {
                    if (GUI.Button(new Rect(x + width - 160f, y - 2f, 80f, 22f), "Unlock"))
                    {
                        manager.TryUnlockCosmetic(entry.Id);
                    }
                }

                y += 24f;
            }
        }

        private static void CycleCharacter(PlayerLoadoutManager manager, int direction)
        {
            if (manager.CharacterCatalog == null || manager.CharacterCatalog.Characters.Count == 0)
            {
                return;
            }

            var characters = manager.CharacterCatalog.Characters;
            int currentIndex = 0;
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] != null && characters[i].Id == manager.LocalLoadout.Character)
                {
                    currentIndex = i;
                    break;
                }
            }

            int nextIndex = ((currentIndex + direction) % characters.Count + characters.Count) % characters.Count;
            if (characters[nextIndex] != null)
            {
                manager.SelectCharacter(characters[nextIndex].Id);
            }
        }

        private static string ResolveDisplayName(PlayerLoadoutManager manager, CosmeticId id)
        {
            if (id.IsNone)
            {
                return "None";
            }

            return manager.CosmeticCatalog != null && manager.CosmeticCatalog.TryGetEntry(id, out CosmeticCatalogConfig.CosmeticEntry entry)
                ? entry.DisplayName
                : id.ToString();
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            _titleStyle.normal.textColor = Color.white;

            _headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            _headerStyle.normal.textColor = Color.yellow;

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft
            };
            _labelStyle.normal.textColor = Color.white;
        }
    }
}

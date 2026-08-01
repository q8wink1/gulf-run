using System;
using System.Collections.Generic;
using GulfRun.Core.Countries;
using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Character.Configuration;
using GulfRun.Features.Character.Loadout;
using GulfRun.Features.Character.UI;
using UnityEngine;

namespace GulfRun.Features.Character.Locker
{
    /// <summary>
    /// Sprint 16 Character Selection / Locker / Customization screen:
    /// Majlis showroom, cinematic camera, category grid, search/filters,
    /// rarity cards, temporary timers, equip/purchase, soft dressing-room audio.
    /// </summary>
    public sealed class LockerView : MonoBehaviour, IMenuScreenOpener
    {
        public static LockerView Instance { get; private set; }

        private static readonly LockerCategory[] Categories =
        {
            LockerCategory.Characters,
            LockerCategory.Outfits,
            LockerCategory.Headwear,
            LockerCategory.Glasses,
            LockerCategory.VictoryPoses,
            LockerCategory.Emotes,
            LockerCategory.FootstepEffects,
            LockerCategory.RunningEffects,
            LockerCategory.ProfileFrames,
            LockerCategory.Titles
        };

        private static readonly LockerOwnershipFilter[] OwnershipFilters =
        {
            LockerOwnershipFilter.All,
            LockerOwnershipFilter.Owned,
            LockerOwnershipFilter.NotOwned,
            LockerOwnershipFilter.Temporary,
            LockerOwnershipFilter.Permanent,
            LockerOwnershipFilter.Country
        };

        [SerializeField] private CountryCatalogConfig countryCatalog;
        [SerializeField] private LockerUiConfig uiConfig;
        [SerializeField] private AudioClip dressingRoomMusic;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip equipSound;
        [SerializeField] private AudioClip rewardSound;

        private bool _open;
        private LockerCategory _category = LockerCategory.Characters;
        private LockerOwnershipFilter _ownershipFilter = LockerOwnershipFilter.All;
        private LockerSortMode _sortMode = LockerSortMode.Newest;
        private string _search = string.Empty;
        private Vector2 _gridScroll;
        private Vector2 _categoryScroll;
        private bool _autoRotate = true;
        private float _rewardFlashUntil;
        private CosmeticRarity _rewardFlashRarity = CosmeticRarity.Common;
        private CosmeticId _selectedCosmetic = CosmeticId.None;
        private CharacterId _selectedCharacterPreview = CharacterId.None;
        private bool _musicPlaying;

        private readonly CharacterShowroomPresenter _showroom = new CharacterShowroomPresenter();
        private readonly CharacterPreviewAnimator _previewAnimator = new CharacterPreviewAnimator();

        private void OnEnable()
        {
            Instance = this;
            MenuScreenRouter.Register(MenuScreen.Characters, this);
            MenuScreenRouter.Register(MenuScreen.Locker, this);
        }

        private void OnDisable()
        {
            MenuScreenRouter.Unregister(MenuScreen.Characters, this);
            MenuScreenRouter.Unregister(MenuScreen.Locker, this);
            if (Instance == this)
            {
                Instance = null;
            }

            StopDressingRoomMusic();
        }

        public void OpenScreen(MenuScreen screen)
        {
            _open = true;
            _category = screen == MenuScreen.Locker ? LockerCategory.Outfits : LockerCategory.Characters;
            _showroom.Reset(uiConfig);
            _previewAnimator.Reset(uiConfig);
            PlayDressingRoomMusic();
            PlayUiSound(buttonClickSound);
        }

        public void Close()
        {
            _open = false;
            StopDressingRoomMusic();
        }

        private void Update()
        {
            if (!_open)
            {
                return;
            }

            float dt = Time.unscaledDeltaTime;
            _showroom.Tick(dt, uiConfig, _autoRotate);
            _previewAnimator.Tick(dt, uiConfig);

            PlayerLoadoutManager manager = PlayerLoadoutManager.Instance;
            if (manager != null)
            {
                manager.PreviewAnimationState = _previewAnimator.State;
            }
        }

        private void OnGUI()
        {
            float scale = uiConfig != null ? uiConfig.ResolveUiScale() : 1f;
            float toggleW = 160f * scale;
            float toggleH = 34f * scale;
            if (GUI.Button(new Rect(10f, Screen.height - toggleH - 12f, toggleW, toggleH), _open ? "Close Locker" : "Locker", CharacterTheme.PanelButton))
            {
                if (_open)
                {
                    Close();
                }
                else
                {
                    OpenScreen(MenuScreen.Locker);
                }
            }

            if (!_open)
            {
                return;
            }

            PlayerLoadoutManager manager = PlayerLoadoutManager.Instance;
            if (manager == null || manager.LocalLoadout == null)
            {
                CharacterTheme.DrawPanel(new Rect(10f, Screen.height - 280f, 420f, 200f));
                GUI.Label(new Rect(24f, Screen.height - 260f, 400f, 24f), "Create an account to open the Locker.", CharacterTheme.Label);
                return;
            }

            DrawLocker(manager, scale);
        }

        private void DrawLocker(PlayerLoadoutManager manager, float scale)
        {
            float margin = 10f * scale;
            float panelX = margin;
            float panelY = 48f * scale;
            float panelW = Screen.width - margin * 2f;
            float panelH = Screen.height - panelY - 56f * scale;

            // Animated dressing-room backdrop wash
            Color previous = GUI.color;
            float pulse = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 0.6f);
            GUI.color = new Color(0.12f + 0.04f * pulse, 0.10f, 0.08f, 0.55f);
            GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), string.Empty);
            GUI.color = previous;

            CharacterTheme.DrawPanel(new Rect(panelX, panelY, panelW, panelH));
            GUI.Label(new Rect(panelX + 14f * scale, panelY + 8f * scale, 280f * scale, 28f * scale), "LOCKER", CharacterTheme.Title);
            CharacterTheme.DrawGoldAccentLine(panelX + 14f * scale, panelY + 36f * scale, 120f * scale);

            if (GUI.Button(new Rect(panelX + panelW - 90f * scale, panelY + 8f * scale, 76f * scale, 28f * scale), "Close", CharacterTheme.GoldButton))
            {
                PlayUiSound(buttonClickSound);
                Close();
            }

            float contentY = panelY + 48f * scale;
            float leftW = Mathf.Clamp(panelW * 0.38f, 260f * scale, 420f * scale);
            float rightX = panelX + leftW + 12f * scale;
            float rightW = panelW - leftW - 24f * scale;

            DrawCountryStrip(manager, panelX + 14f * scale, contentY, leftW - 20f * scale, scale);
            contentY += 52f * scale;

            Rect showroomRect = new Rect(panelX + 14f * scale, contentY, leftW - 20f * scale, Mathf.Min(320f * scale, panelH * 0.42f));
            DrawShowroom(manager, showroomRect, scale);
            contentY = showroomRect.yMax + 8f * scale;
            DrawCameraControls(panelX + 14f * scale, contentY, leftW - 20f * scale, scale);
            contentY += 78f * scale;
            DrawAnimationControls(manager, panelX + 14f * scale, contentY, leftW - 20f * scale, scale);

            DrawCategoryTabs(rightX, panelY + 48f * scale, rightW, scale);
            float gridTop = panelY + 96f * scale;
            DrawSearchAndFilters(rightX, gridTop, rightW, scale);
            gridTop += 78f * scale;

            if (_category == LockerCategory.Characters)
            {
                DrawCharacterGrid(manager, new Rect(rightX, gridTop, rightW, panelY + panelH - gridTop - 14f * scale), scale);
            }
            else if (LockerCategoryMapping.TryGetSlot(_category, out CosmeticSlot slot))
            {
                DrawCosmeticGrid(manager, slot, new Rect(rightX, gridTop, rightW, panelY + panelH - gridTop - 14f * scale), scale);
            }
        }

        private void DrawCountryStrip(PlayerLoadoutManager manager, float x, float y, float width, float scale)
        {
            GulfCountry country = manager.LocalLoadout.Country;
            string name = country.ToString();
            Color flagColor = CharacterTheme.Sand;
            if (countryCatalog != null && countryCatalog.TryGetEntry(country, out CountryCatalogConfig.CountryEntry entry))
            {
                name = entry.DisplayName;
                flagColor = entry.PlaceholderColor;
            }

            int countryRank = -1;
            ILocalProfileProvider profile = LocalProfileProviderService.Current;
            if (profile != null && profile.HasProfile)
            {
                countryRank = profile.LocalProfile.CountryRank;
            }

            Color previous = GUI.color;
            GUI.color = flagColor;
            GUI.Box(new Rect(x, y, 36f * scale, 28f * scale), string.Empty);
            GUI.color = previous;
            GUI.Label(new Rect(x + 44f * scale, y, width - 44f * scale, 18f * scale), name + " (locked)", CharacterTheme.Header);
            GUI.Label(
                new Rect(x + 44f * scale, y + 20f * scale, width - 44f * scale, 18f * scale),
                "Country Ranking: " + (countryRank > 0 ? "#" + countryRank : "—"),
                CharacterTheme.MutedLabel);
        }

        private void DrawShowroom(PlayerLoadoutManager manager, Rect rect, float scale)
        {
            CharacterDefinition definition = manager.CharacterCatalog != null
                ? manager.CharacterCatalog.GetDefinition(manager.LocalLoadout.Character)
                : null;
            Color characterColor = definition != null ? definition.PlaceholderColor : Color.gray;
            string label = definition != null ? definition.DisplayName : manager.LocalLoadout.Character.ToString();

            Color outfitAccent = CharacterTheme.Gold;
            CosmeticId outfitId = manager.LocalLoadout.GetEquipped(CosmeticSlot.Outfit);
            if (manager.CosmeticCatalog != null && manager.CosmeticCatalog.TryGetEntry(outfitId, out CosmeticCatalogConfig.CosmeticEntry outfit))
            {
                outfitAccent = outfit.PlaceholderColor;
                label += " · " + outfit.DisplayName;
            }

            string city = CharacterShowroomPresenter.CityHintFor(manager.LocalLoadout.Country);
            _showroom.Draw(rect, characterColor, label, outfitAccent, _previewAnimator, uiConfig, manager.LocalLoadout.Country, city);
        }

        private void DrawCameraControls(float x, float y, float width, float scale)
        {
            float btnW = 70f * scale;
            float btnH = 28f * scale;
            float gap = 6f * scale;
            if (GUI.Button(new Rect(x, y, btnW, btnH), "⟵ Rot", CharacterTheme.PanelButton))
            {
                _autoRotate = false;
                _showroom.RotateBy(-22f);
                PlayUiSound(buttonClickSound);
            }

            if (GUI.Button(new Rect(x + btnW + gap, y, btnW, btnH), "Rot ⟶", CharacterTheme.PanelButton))
            {
                _autoRotate = false;
                _showroom.RotateBy(22f);
                PlayUiSound(buttonClickSound);
            }

            if (GUI.Button(new Rect(x + (btnW + gap) * 2f, y, btnW, btnH), "Zoom+", CharacterTheme.PanelButton))
            {
                _showroom.ZoomBy(uiConfig != null ? uiConfig.ZoomStep : 0.08f, uiConfig);
                PlayUiSound(buttonClickSound);
            }

            if (GUI.Button(new Rect(x + (btnW + gap) * 3f, y, btnW, btnH), "Zoom−", CharacterTheme.PanelButton))
            {
                _showroom.ZoomBy(uiConfig != null ? -uiConfig.ZoomStep : -0.08f, uiConfig);
                PlayUiSound(buttonClickSound);
            }

            if (GUI.Button(new Rect(x, y + btnH + gap, 100f * scale, btnH), _autoRotate ? "Auto: ON" : "Auto: OFF", CharacterTheme.PanelButton))
            {
                _autoRotate = !_autoRotate;
                PlayUiSound(buttonClickSound);
            }

            if (GUI.Button(new Rect(x + 110f * scale, y + btnH + gap, 100f * scale, btnH), "Focus", CharacterTheme.GoldButton))
            {
                _showroom.AutoFocus(uiConfig);
                PlayUiSound(buttonClickSound);
            }
        }

        private void DrawAnimationControls(PlayerLoadoutManager manager, float x, float y, float width, float scale)
        {
            GUI.Label(new Rect(x, y, width, 18f * scale), "Preview Anim: " + _previewAnimator.State, CharacterTheme.MutedLabel);
            if (GUI.Button(new Rect(x, y + 22f * scale, 140f * scale, 28f * scale), "Cycle Anim", CharacterTheme.PanelButton))
            {
                _previewAnimator.CycleNext(uiConfig);
                manager.PreviewAnimationState = _previewAnimator.State;
                PlayUiSound(buttonClickSound);
            }
        }

        private void DrawCategoryTabs(float x, float y, float width, float scale)
        {
            float btnW = 108f * scale;
            float btnH = 30f * scale;
            Rect view = new Rect(x, y, width, btnH + 4f * scale);
            Rect content = new Rect(0f, 0f, Categories.Length * (btnW + 6f * scale), btnH);
            _categoryScroll = GUI.BeginScrollView(view, _categoryScroll, content);
            for (int i = 0; i < Categories.Length; i++)
            {
                LockerCategory cat = Categories[i];
                bool selected = cat == _category;
                if (GUI.Button(new Rect(i * (btnW + 6f * scale), 0f, btnW, btnH), LockerCategoryMapping.DisplayName(cat), selected ? CharacterTheme.GoldButton : CharacterTheme.PanelButton))
                {
                    _category = cat;
                    _gridScroll = Vector2.zero;
                    PlayUiSound(buttonClickSound);
                }
            }

            GUI.EndScrollView();
        }

        private void DrawSearchAndFilters(float x, float y, float width, float scale)
        {
            GUI.Label(new Rect(x, y, 60f * scale, 22f * scale), "Search", CharacterTheme.MutedLabel);
            _search = GUI.TextField(new Rect(x + 60f * scale, y, width - 60f * scale, 24f * scale), _search ?? string.Empty);

            float fy = y + 30f * scale;
            float chipW = 78f * scale;
            float chipH = 24f * scale;
            for (int i = 0; i < OwnershipFilters.Length; i++)
            {
                LockerOwnershipFilter filter = OwnershipFilters[i];
                bool on = filter == _ownershipFilter;
                if (GUI.Button(new Rect(x + i * (chipW + 4f * scale), fy, chipW, chipH), filter.ToString(), on ? CharacterTheme.GoldButton : CharacterTheme.PanelButton))
                {
                    _ownershipFilter = filter;
                    PlayUiSound(buttonClickSound);
                }
            }

            float sortX = x + OwnershipFilters.Length * (chipW + 4f * scale) + 8f * scale;
            if (GUI.Button(new Rect(sortX, fy, 90f * scale, chipH), "Newest", _sortMode == LockerSortMode.Newest ? CharacterTheme.GoldButton : CharacterTheme.PanelButton))
            {
                _sortMode = LockerSortMode.Newest;
                PlayUiSound(buttonClickSound);
            }

            if (GUI.Button(new Rect(sortX + 96f * scale, fy, 90f * scale, chipH), "Rarity", _sortMode == LockerSortMode.Rarity ? CharacterTheme.GoldButton : CharacterTheme.PanelButton))
            {
                _sortMode = LockerSortMode.Rarity;
                PlayUiSound(buttonClickSound);
            }
        }

        private void DrawCharacterGrid(PlayerLoadoutManager manager, Rect viewport, float scale)
        {
            if (manager.CharacterCatalog == null)
            {
                return;
            }

            IReadOnlyList<CharacterDefinition> characters = manager.CharacterCatalog.Characters;
            float cardW = 120f * scale;
            float cardH = 140f * scale;
            int cols = Mathf.Max(1, Mathf.FloorToInt(viewport.width / (cardW + 8f * scale)));
            int rows = Mathf.CeilToInt(characters.Count / (float)cols);
            Rect content = new Rect(0f, 0f, viewport.width - 16f, rows * (cardH + 8f * scale));
            _gridScroll = GUI.BeginScrollView(viewport, _gridScroll, content);

            for (int i = 0; i < characters.Count; i++)
            {
                CharacterDefinition def = characters[i];
                if (def == null)
                {
                    continue;
                }

                int col = i % cols;
                int row = i / cols;
                Rect card = new Rect(col * (cardW + 8f * scale), row * (cardH + 8f * scale), cardW, cardH);
                bool selected = manager.LocalLoadout.Character == def.Id;
                CharacterTheme.DrawRarityCard(card, selected ? CosmeticRarity.Legendary : CosmeticRarity.Common, RarityPulse(), false);

                Color previous = GUI.color;
                GUI.color = def.PlaceholderColor;
                GUI.Box(new Rect(card.x + 16f * scale, card.y + 16f * scale, card.width - 32f * scale, 70f * scale), string.Empty);
                GUI.color = previous;
                GUI.Label(new Rect(card.x + 6f * scale, card.y + 90f * scale, card.width - 12f * scale, 20f * scale), def.DisplayName, CharacterTheme.Label);
                GUI.Label(new Rect(card.x + 6f * scale, card.y + 110f * scale, card.width - 12f * scale, 18f * scale), selected ? "SELECTED" : "Unlocked", CharacterTheme.MutedLabel);

                if (GUI.Button(card, string.Empty, GUIStyle.none))
                {
                    manager.SelectCharacter(def.Id);
                    _selectedCharacterPreview = def.Id;
                    _showroom.AutoFocus(uiConfig);
                    PlayUiSound(equipSound);
                    FlashReward(CosmeticRarity.Legendary);
                }
            }

            GUI.EndScrollView();
        }

        private void DrawCosmeticGrid(PlayerLoadoutManager manager, CosmeticSlot slot, Rect viewport, float scale)
        {
            IReadOnlyList<CosmeticCatalogConfig.CosmeticEntry> entries = LockerCatalogFilter.Query(
                manager,
                slot,
                _ownershipFilter,
                _sortMode,
                _search,
                manager.LocalLoadout.Country);

            float cardW = 150f * scale;
            float cardH = 150f * scale;
            int cols = Mathf.Max(1, Mathf.FloorToInt(viewport.width / (cardW + 8f * scale)));
            int rows = Mathf.Max(1, Mathf.CeilToInt(entries.Count / (float)cols));
            Rect content = new Rect(0f, 0f, viewport.width - 16f, rows * (cardH + 8f * scale) + 8f * scale);
            _gridScroll = GUI.BeginScrollView(viewport, _gridScroll, content);

            double now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            CosmeticId equipped = manager.LocalLoadout.GetEquipped(slot);
            float pulse = RarityPulse();
            bool flashing = Time.unscaledTime < _rewardFlashUntil;

            for (int i = 0; i < entries.Count; i++)
            {
                CosmeticCatalogConfig.CosmeticEntry entry = entries[i];
                int col = i % cols;
                int row = i / cols;
                Rect card = new Rect(col * (cardW + 8f * scale), row * (cardH + 8f * scale), cardW, cardH);
                bool isFlash = flashing && entry.Rarity == _rewardFlashRarity && entry.Id == _selectedCosmetic;
                CharacterTheme.DrawRarityCard(card, entry.Rarity, pulse, isFlash);

                Color previous = GUI.color;
                GUI.color = entry.PlaceholderColor;
                GUI.Box(new Rect(card.x + 12f * scale, card.y + 12f * scale, card.width - 24f * scale, 48f * scale), string.Empty);
                GUI.color = previous;

                GUI.Label(new Rect(card.x + 6f * scale, card.y + 64f * scale, card.width - 12f * scale, 18f * scale), entry.DisplayName, CharacterTheme.Label);
                GUI.Label(new Rect(card.x + 6f * scale, card.y + 82f * scale, card.width - 12f * scale, 16f * scale), entry.Rarity.ToString(), CharacterTheme.MutedLabel);

                bool owned = manager.LocalInventory.Owns(entry.Id);
                bool permanent = manager.LocalInventory.OwnsPermanently(entry.Id);
                bool temporary = manager.LocalInventory.OwnsTemporarily(entry.Id);
                bool isEquipped = entry.Id == equipped;

                string status;
                if (isEquipped)
                {
                    status = "EQUIPPED";
                }
                else if (temporary && manager.LocalInventory.TryGetTemporaryExpiry(entry.Id, out double expires))
                {
                    status = "TEMP " + FormatCountdown(expires - now);
                }
                else if (permanent)
                {
                    status = "Owned";
                }
                else
                {
                    status = entry.GemPrice + " Gems";
                }

                GUI.Label(new Rect(card.x + 6f * scale, card.y + 98f * scale, card.width - 12f * scale, 16f * scale), status, CharacterTheme.MutedLabel);

                float actionY = card.y + card.height - 28f * scale;
                if (owned && !isEquipped)
                {
                    if (GUI.Button(new Rect(card.x + 6f * scale, actionY, (card.width - 14f * scale) * 0.5f, 24f * scale), "Equip", CharacterTheme.GoldButton))
                    {
                        manager.EquipCosmetic(slot, entry.Id);
                        _selectedCosmetic = entry.Id;
                        FlashReward(entry.Rarity);
                        PlayUiSound(equipSound);
                    }

                    if (GUI.Button(new Rect(card.x + card.width * 0.52f, actionY, (card.width - 14f * scale) * 0.45f, 24f * scale), "Preview", CharacterTheme.PanelButton))
                    {
                        _selectedCosmetic = entry.Id;
                        _showroom.AutoFocus(uiConfig);
                        PlayUiSound(buttonClickSound);
                    }
                }
                else if (owned && isEquipped)
                {
                    if (GUI.Button(new Rect(card.x + 6f * scale, actionY, card.width - 12f * scale, 24f * scale), "Unequip", CharacterTheme.PanelButton))
                    {
                        manager.UnequipCosmetic(slot);
                        PlayUiSound(buttonClickSound);
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(card.x + 6f * scale, actionY, card.width - 12f * scale, 24f * scale), "Buy", CharacterTheme.GoldButton))
                    {
                        if (manager.TryUnlockCosmetic(entry.Id))
                        {
                            manager.EquipCosmetic(slot, entry.Id);
                            _selectedCosmetic = entry.Id;
                            FlashReward(entry.Rarity);
                            PlayUiSound(rewardSound);
                        }
                        else
                        {
                            PlayUiSound(buttonClickSound);
                        }
                    }
                }
            }

            if (entries.Count == 0)
            {
                GUI.Label(new Rect(8f, 8f, viewport.width - 24f, 24f), "No items match filters/search.", CharacterTheme.MutedLabel);
            }

            GUI.EndScrollView();
        }

        private void FlashReward(CosmeticRarity rarity)
        {
            _rewardFlashRarity = rarity;
            _rewardFlashUntil = Time.unscaledTime + (uiConfig != null ? uiConfig.RarityRewardFlashSeconds : 0.55f);
        }

        private float RarityPulse()
        {
            float hz = uiConfig != null ? uiConfig.RarityGlowPulseHz : 1.2f;
            return 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * hz * Mathf.PI * 2f);
        }

        private static string FormatCountdown(double remainingSeconds)
        {
            if (remainingSeconds <= 0d)
            {
                return "Expired";
            }

            int total = (int)remainingSeconds;
            int days = total / 86400;
            int hours = (total % 86400) / 3600;
            int minutes = (total % 3600) / 60;
            if (days > 0)
            {
                return days + "d " + hours + "h";
            }

            if (hours > 0)
            {
                return hours + "h " + minutes + "m";
            }

            return minutes + "m";
        }

        private void PlayDressingRoomMusic()
        {
            if (_musicPlaying || AudioManager.Instance == null || dressingRoomMusic == null)
            {
                return;
            }

            AudioManager.Instance.PlayMusic(dressingRoomMusic, 0.45f, true);
            _musicPlaying = true;
        }

        private void StopDressingRoomMusic()
        {
            if (!_musicPlaying || AudioManager.Instance == null)
            {
                return;
            }

            // Null clip is a no-op stop path that avoids stomping lobby music hard —
            // fade out then leave Main Menu to restore its own track on next open.
            AudioManager.Instance.FadeMusicTo(0f, 0.4f);
            _musicPlaying = false;
        }

        private void PlayUiSound(AudioClip clip)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayOneShot(clip);
            }
        }
    }
}

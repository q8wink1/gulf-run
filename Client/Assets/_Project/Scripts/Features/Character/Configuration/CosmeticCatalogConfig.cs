using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Character.Configuration
{
    /// <summary>
    /// The single source of truth for every cosmetic item across every
    /// <see cref="CosmeticSlot"/> — the eight free Traditional Outfits plus
    /// a handful of example Premium Cosmetics (Football Club Kit, National
    /// Team Kit, Ramadan Collection, ...) per the Sprint 8 brief. Entries
    /// are plain serializable data (not one <c>ScriptableObject</c> asset
    /// each, unlike <c>WeaponDefinition</c>/<c>TrapDefinition</c>) —
    /// deliberately, since "prepare support for unlimited future
    /// characters/cosmetics" means this list is expected to grow into the
    /// hundreds; one asset file per item would not scale for content
    /// authors the way <c>Features.RaceFinish.Configuration.
    /// FlagCatalogConfig</c>'s nested-entry approach already proved out in
    /// Sprint 7.
    /// </summary>
    [CreateAssetMenu(fileName = "CosmeticCatalogConfig", menuName = "GulfRun/Character/Cosmetic Catalog Config")]
    public sealed class CosmeticCatalogConfig : ScriptableObject
    {
        [Serializable]
        public sealed class CosmeticEntry
        {
            [SerializeField] private string id;
            [SerializeField] private CosmeticSlot slot;
            [SerializeField] private string displayName = string.Empty;

            [Tooltip("Gems required to unlock. Ignored (always free/owned) when isTraditionalOutfit is true.")]
            [SerializeField] private int gemPrice;

            [Tooltip("True for exactly one Outfit-slot entry per country — every country's official traditional clothing is FREE and auto-granted/auto-equipped the moment that country is locked in at Account Creation.")]
            [SerializeField] private bool isTraditionalOutfit;

            [Tooltip("Only meaningful when isTraditionalOutfit is true: which country this Traditional Outfit belongs to.")]
            [SerializeField] private GulfCountry requiredCountry;

            [Tooltip("Free-form grouping for future Shop/Inventory filtering (e.g. 'Ramadan Collection', 'National Team Kits'). Cosmetic only — never affects gameplay.")]
            [SerializeField] private string collectionTag = string.Empty;

            [Header("Presentation (final art TODO — same 'no final art yet' status as prior sprints)")]
            [SerializeField] private Sprite icon;
            [SerializeField] private Color placeholderColor = Color.white;

            public CosmeticId Id => new CosmeticId(id);
            public CosmeticSlot Slot => slot;
            public string DisplayName => string.IsNullOrEmpty(displayName) ? id : displayName;
            public int GemPrice => isTraditionalOutfit ? 0 : (gemPrice > 0 ? gemPrice : 0);
            public bool IsTraditionalOutfit => isTraditionalOutfit;
            public GulfCountry RequiredCountry => requiredCountry;
            public string CollectionTag => collectionTag;
            public Sprite Icon => icon;
            public Color PlaceholderColor => placeholderColor;
        }

        [SerializeField] private List<CosmeticEntry> entries = new List<CosmeticEntry>();

        private readonly Dictionary<string, CosmeticEntry> _byId = new Dictionary<string, CosmeticEntry>();
        private readonly List<CosmeticEntry> _slotScratch = new List<CosmeticEntry>();
        private bool _indexed;

        public IReadOnlyList<CosmeticEntry> Entries => entries;

        public bool TryGetEntry(CosmeticId id, out CosmeticEntry entry)
        {
            EnsureIndexed();
            return _byId.TryGetValue(id.Value, out entry);
        }

        /// <summary>The one free Outfit-slot cosmetic auto-granted/auto-equipped for a given country, or <see cref="CosmeticId.None"/> if none is authored yet.</summary>
        public CosmeticId GetTraditionalOutfitId(GulfCountry country)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                CosmeticEntry candidate = entries[i];
                if (candidate != null && candidate.IsTraditionalOutfit && candidate.RequiredCountry == country)
                {
                    return candidate.Id;
                }
            }

            return CosmeticId.None;
        }

        /// <summary>Every entry for one slot (e.g. every Outfit) — cached/reused list, matching <c>WeaponCatalogConfig</c>'s documented "not reentrant" usage.</summary>
        public IReadOnlyList<CosmeticEntry> GetBySlot(CosmeticSlot slot)
        {
            _slotScratch.Clear();
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i] != null && entries[i].Slot == slot)
                {
                    _slotScratch.Add(entries[i]);
                }
            }

            return _slotScratch;
        }

        private void EnsureIndexed()
        {
            if (_indexed)
            {
                return;
            }

            _byId.Clear();
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i] != null && !string.IsNullOrEmpty(entries[i].Id.Value))
                {
                    _byId[entries[i].Id.Value] = entries[i];
                }
            }

            _indexed = true;
        }

#if UNITY_EDITOR
        private void OnValidate() => _indexed = false;
#endif
    }
}

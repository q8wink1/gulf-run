using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Store.Configuration
{
    /// <summary>
    /// Every individually-purchasable Store product that isn't a Gem
    /// Package, Coin Pack, or the Battle Pass itself — the brief's "STORE
    /// ITEMS" section (Characters/Character Skins/Traditional Outfits/
    /// Sports Outfits/Football Club Kits/National Team Kits/Emotes/Victory
    /// Animations/Visual Effects/Profile Frames/Future Cosmetic Items).
    /// "Character Skins" is folded into <see cref="StoreSection.Outfits"/>
    /// (this project has no separate skin slot from Sprint 8's
    /// <c>CosmeticSlot</c> — see Sprint 10 report Remaining TODOs), and
    /// every Traditional-Outfit/Sports-Outfit/Football-Kit/National-Kit
    /// item maps onto <see cref="StoreSection.Outfits"/> too, distinguished
    /// only by <see cref="StoreItemEntry.CollectionTag"/> for Store-UI
    /// grouping — exactly the same "one slot, tag-based grouping" approach
    /// <c>Features.Character.Configuration.CosmeticCatalogConfig</c>
    /// already uses.
    /// <para>
    /// Where an entry's content already exists in Sprint 8's cosmetic
    /// system (every Outfit/Emote/Victory-Pose item here), <see cref="StoreItemEntry.LinkedCosmeticId"/>
    /// names that exact <c>CosmeticId</c> so a purchase grants the SAME
    /// item Sprint 8's direct in-Character-Menu Gem unlock already offers —
    /// the Store is a second, richer storefront in front of that catalog,
    /// not a competing definition of the item (see class remarks in the
    /// Sprint 10 report for the "two catalogs, kept in sync by hand" TODO
    /// this implies). Visual Effects and Profile Frames have no existing
    /// slot to link to yet, so those entries own their purchase entirely
    /// through the Store's own ledger (<c>IStoreBackendService</c>).
    /// </para>
    /// </summary>
    [CreateAssetMenu(fileName = "StoreItemCatalogConfig", menuName = "GulfRun/Store/Store Item Catalog Config")]
    public sealed class StoreItemCatalogConfig : ScriptableObject
    {
        [Serializable]
        public sealed class StoreItemEntry
        {
            [SerializeField] private string id;
            [SerializeField] private StoreSection section;
            [SerializeField] private string displayName = string.Empty;
            [SerializeField, TextArea] private string description = string.Empty;
            [SerializeField] private StoreCurrency currency;
            [SerializeField] private int priceAmount;
            [SerializeField] private string priceCurrencyCode = "USD";
            [SerializeField] private float realMoneyPriceAmount;

            [Tooltip("For Characters section entries: which existing (already-unlocked, Sprint 8) CharacterId this showcases.")]
            [SerializeField] private string linkedCharacterId;

            [Tooltip("For Outfits/Emotes/VictoryPoses entries: which existing Features.Character.Configuration.CosmeticCatalogConfig CosmeticId a purchase grants.")]
            [SerializeField] private string linkedCosmeticId;

            [Tooltip("0 = not on sale. Otherwise the percentage off priceAmount/realMoneyPriceAmount shown in the Store (display-only — the discounted price is not separately re-derived from this field yet, see Sprint 10 report Remaining TODOs).")]
            [SerializeField, Range(0, 100)] private int saleDiscountPercent;

            [SerializeField] private string collectionTag = string.Empty;

            [Header("Presentation (final art TODO — same 'no final art yet' status as prior sprints)")]
            [SerializeField] private Color placeholderColor = Color.white;

            public StoreItemId Id => new StoreItemId(id);
            public StoreSection Section => section;
            public string DisplayName => string.IsNullOrEmpty(displayName) ? id : displayName;
            public string Description => description;
            public StoreCurrency Currency => currency;
            public int PriceAmount => priceAmount;
            public RealMoneyPrice RealMoneyPrice => new RealMoneyPrice(priceCurrencyCode, realMoneyPriceAmount);
            public CharacterId LinkedCharacterId => new CharacterId(linkedCharacterId);
            public CosmeticId LinkedCosmeticId => new CosmeticId(linkedCosmeticId);
            public bool IsOnSale => saleDiscountPercent > 0;
            public int SaleDiscountPercent => saleDiscountPercent;
            public string CollectionTag => collectionTag;
            public Color PlaceholderColor => placeholderColor;
        }

        [SerializeField] private List<StoreItemEntry> items = new List<StoreItemEntry>();

        private readonly Dictionary<string, StoreItemEntry> _byId = new Dictionary<string, StoreItemEntry>();
        private readonly List<StoreItemEntry> _sectionScratch = new List<StoreItemEntry>();
        private bool _indexed;

        public IReadOnlyList<StoreItemEntry> Items => items;

        public bool TryGetEntry(StoreItemId id, out StoreItemEntry entry)
        {
            EnsureIndexed();
            return _byId.TryGetValue(id.Value, out entry);
        }

        /// <summary>Every entry for one section — cached/reused list, matching <c>CosmeticCatalogConfig.GetBySlot</c>'s documented "not reentrant" usage.</summary>
        public IReadOnlyList<StoreItemEntry> GetBySection(StoreSection wantedSection)
        {
            _sectionScratch.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].Section == wantedSection)
                {
                    _sectionScratch.Add(items[i]);
                }
            }

            return _sectionScratch;
        }

        private void EnsureIndexed()
        {
            if (_indexed)
            {
                return;
            }

            _byId.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && !string.IsNullOrEmpty(items[i].Id.Value))
                {
                    _byId[items[i].Id.Value] = items[i];
                }
            }

            _indexed = true;
        }

#if UNITY_EDITOR
        private void OnValidate() => _indexed = false;
#endif
    }
}

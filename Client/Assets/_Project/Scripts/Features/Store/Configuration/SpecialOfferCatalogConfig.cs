using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Store.Configuration
{
    /// <summary>
    /// Limited Offers — bundles of existing <see cref="StoreItemCatalogConfig"/>
    /// entries sold together at a single bundle price, tied to the brief's
    /// event examples (Ramadan/Eid/National Days/Summer/Winter/Anniversary/
    /// Regional Celebrations). <see cref="SpecialOfferEntry.AssociatedEventLabel"/>
    /// is a free-form string rather than a typed reference to
    /// <c>Features.Online.Configuration.CountryEventCatalogConfig</c>'s
    /// entries on purpose — Store and Online are sibling Features assemblies
    /// (neither may reference the other), so the two catalogs are linked by
    /// naming convention only today (see Sprint 10 report Remaining TODOs).
    /// A bundle's own price is authored independently of the sum of its
    /// component items' individual prices (and may even use a different
    /// <see cref="StoreCurrency"/> than its components) — a standard IAP
    /// bundle pattern, not a bug.
    /// </summary>
    [CreateAssetMenu(fileName = "SpecialOfferCatalogConfig", menuName = "GulfRun/Store/Special Offer Catalog Config")]
    public sealed class SpecialOfferCatalogConfig : ScriptableObject
    {
        [Serializable]
        public sealed class SpecialOfferEntry
        {
            [SerializeField] private string id;
            [SerializeField] private string displayName = string.Empty;
            [SerializeField, TextArea] private string description = string.Empty;
            [SerializeField] private string associatedEventLabel = string.Empty;
            [SerializeField] private List<string> bundledStoreItemIds = new List<string>();
            [SerializeField] private StoreCurrency currency;
            [SerializeField] private int priceAmount;
            [SerializeField] private string priceCurrencyCode = "USD";
            [SerializeField] private float realMoneyPriceAmount;
            [SerializeField] private bool isActive = true;

            [Header("Presentation (final art TODO — same 'no final art yet' status as prior sprints)")]
            [SerializeField] private Color placeholderColor = Color.white;

            public StoreItemId Id => new StoreItemId(id);
            public string DisplayName => string.IsNullOrEmpty(displayName) ? id : displayName;
            public string Description => description;
            public string AssociatedEventLabel => associatedEventLabel;
            public IReadOnlyList<string> BundledStoreItemIds => bundledStoreItemIds;
            public StoreCurrency Currency => currency;
            public int PriceAmount => priceAmount;
            public RealMoneyPrice RealMoneyPrice => new RealMoneyPrice(priceCurrencyCode, realMoneyPriceAmount);
            public bool IsActive => isActive;
            public Color PlaceholderColor => placeholderColor;
        }

        [SerializeField] private List<SpecialOfferEntry> offers = new List<SpecialOfferEntry>();

        public IReadOnlyList<SpecialOfferEntry> Offers => offers;

        public SpecialOfferEntry GetOffer(StoreItemId id)
        {
            for (int i = 0; i < offers.Count; i++)
            {
                if (offers[i] != null && offers[i].Id == id)
                {
                    return offers[i];
                }
            }

            return null;
        }
    }
}

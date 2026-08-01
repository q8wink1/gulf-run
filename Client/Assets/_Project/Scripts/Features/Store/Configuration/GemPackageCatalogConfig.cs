using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Store.Configuration
{
    /// <summary>
    /// The Gem Packages section from the Sprint 10 brief — "Launch with 6
    /// Gem Packages. Package sizes and pricing must be configurable"
    /// (P045/P012). Every size, bonus, and real-money price is authored
    /// data, never a hardcoded number in <c>StoreManager</c>, mirroring
    /// Sprint 9's <c>ChampionshipCatalogConfig</c>/<c>RewardCatalogConfig</c>
    /// "no balance number lives in code" pattern applied to monetization.
    /// </summary>
    [CreateAssetMenu(fileName = "GemPackageCatalogConfig", menuName = "GulfRun/Store/Gem Package Catalog Config")]
    public sealed class GemPackageCatalogConfig : ScriptableObject
    {
        [Serializable]
        public sealed class GemPackageEntry
        {
            [SerializeField] private string id;
            [SerializeField] private string displayName = string.Empty;
            [SerializeField] private int gemAmount;

            [Tooltip("Extra Gems included on top of gemAmount — the 'Bonus Gems' brief requirement.")]
            [SerializeField] private int bonusGemAmount;

            [SerializeField] private string priceCurrencyCode = "USD";
            [SerializeField] private float priceAmount;

            [Tooltip("Marks this package as a time-limited/launch offer for Store presentation (badge, Notifications) — does not change purchase logic.")]
            [SerializeField] private bool isLimitedOffer;

            [Header("Presentation (final art TODO — same 'no final art yet' status as prior sprints)")]
            [SerializeField] private Color placeholderColor = Color.white;

            public StoreItemId Id => new StoreItemId(id);
            public string DisplayName => string.IsNullOrEmpty(displayName) ? id : displayName;
            public int GemAmount => gemAmount;
            public int BonusGemAmount => bonusGemAmount;
            public int TotalGemAmount => gemAmount + bonusGemAmount;
            public RealMoneyPrice Price => new RealMoneyPrice(priceCurrencyCode, priceAmount);
            public bool IsLimitedOffer => isLimitedOffer;
            public Color PlaceholderColor => placeholderColor;
        }

        [SerializeField] private List<GemPackageEntry> packages = new List<GemPackageEntry>();

        public IReadOnlyList<GemPackageEntry> Packages => packages;

        public GemPackageEntry GetPackage(StoreItemId id)
        {
            for (int i = 0; i < packages.Count; i++)
            {
                if (packages[i] != null && packages[i].Id == id)
                {
                    return packages[i];
                }
            }

            return null;
        }
    }
}

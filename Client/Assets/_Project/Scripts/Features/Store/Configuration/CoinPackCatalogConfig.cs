using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Store.Configuration
{
    /// <summary>
    /// Real-money Coin Packs — the fourth Coins source the brief lists
    /// ("Purchasing Coin Packs using real money"), alongside Playing
    /// Matches / Winning Races (already Coin sources since Sprint 7's
    /// <c>RaceRewardApplier</c>) and Completing Missions (no mission system
    /// exists yet — see Sprint 10 report Remaining TODOs). Same fully
    /// data-driven shape as <see cref="GemPackageCatalogConfig"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "CoinPackCatalogConfig", menuName = "GulfRun/Store/Coin Pack Catalog Config")]
    public sealed class CoinPackCatalogConfig : ScriptableObject
    {
        [Serializable]
        public sealed class CoinPackEntry
        {
            [SerializeField] private string id;
            [SerializeField] private string displayName = string.Empty;
            [SerializeField] private int coinAmount;
            [SerializeField] private int bonusCoinAmount;
            [SerializeField] private string priceCurrencyCode = "USD";
            [SerializeField] private float priceAmount;
            [SerializeField] private bool isLimitedOffer;

            [Header("Presentation (final art TODO — same 'no final art yet' status as prior sprints)")]
            [SerializeField] private Color placeholderColor = Color.white;

            public StoreItemId Id => new StoreItemId(id);
            public string DisplayName => string.IsNullOrEmpty(displayName) ? id : displayName;
            public int CoinAmount => coinAmount;
            public int BonusCoinAmount => bonusCoinAmount;
            public int TotalCoinAmount => coinAmount + bonusCoinAmount;
            public RealMoneyPrice Price => new RealMoneyPrice(priceCurrencyCode, priceAmount);
            public bool IsLimitedOffer => isLimitedOffer;
            public Color PlaceholderColor => placeholderColor;
        }

        [SerializeField] private List<CoinPackEntry> packs = new List<CoinPackEntry>();

        public IReadOnlyList<CoinPackEntry> Packs => packs;

        public CoinPackEntry GetPack(StoreItemId id)
        {
            for (int i = 0; i < packs.Count; i++)
            {
                if (packs[i] != null && packs[i].Id == id)
                {
                    return packs[i];
                }
            }

            return null;
        }
    }
}

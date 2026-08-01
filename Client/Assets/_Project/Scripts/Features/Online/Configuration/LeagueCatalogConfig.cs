using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Online.Configuration
{
    /// <summary>
    /// The single source of truth for the 8 <see cref="League"/> tiers'
    /// trophy thresholds and display data — no hardcoded numbers in
    /// <c>Leagues.LeagueManager</c> itself (see <see cref="LeagueRules"/>),
    /// exactly like <c>Features.Character.Configuration.CosmeticCatalogConfig</c>
    /// keeps every gem price out of code. One entry per tier, authored in
    /// ascending <see cref="League"/> order.
    /// </summary>
    [CreateAssetMenu(fileName = "LeagueCatalogConfig", menuName = "GulfRun/Online/League Catalog Config")]
    public sealed class LeagueCatalogConfig : ScriptableObject
    {
        [Serializable]
        public sealed class LeagueTierEntry
        {
            [SerializeField] private League tier;
            [SerializeField] private string displayName = string.Empty;

            [Tooltip("Minimum trophy count to reach/hold this tier this season. Placeholder balance — see Sprint 9 report Remaining TODOs.")]
            [SerializeField] private int trophyThreshold;

            [SerializeField] private Color placeholderColor = Color.white;

            public League Tier => tier;
            public string DisplayName => string.IsNullOrEmpty(displayName) ? tier.ToString() : displayName;
            public int TrophyThreshold => trophyThreshold;
            public Color PlaceholderColor => placeholderColor;
        }

        [SerializeField] private List<LeagueTierEntry> tiers = new List<LeagueTierEntry>();

        private readonly List<int> _thresholdsScratch = new List<int>();

        public IReadOnlyList<LeagueTierEntry> Tiers => tiers;

        /// <summary>Ascending thresholds indexed exactly like <see cref="League"/>'s declaration order — see <see cref="LeagueRules.ResolveLeague"/>.</summary>
        public IReadOnlyList<int> Thresholds
        {
            get
            {
                _thresholdsScratch.Clear();
                for (int i = 0; i < tiers.Count; i++)
                {
                    _thresholdsScratch.Add(tiers[i] != null ? tiers[i].TrophyThreshold : 0);
                }

                return _thresholdsScratch;
            }
        }

        public string GetDisplayName(League league)
        {
            for (int i = 0; i < tiers.Count; i++)
            {
                if (tiers[i] != null && tiers[i].Tier == league)
                {
                    return tiers[i].DisplayName;
                }
            }

            return league.ToString();
        }
    }
}

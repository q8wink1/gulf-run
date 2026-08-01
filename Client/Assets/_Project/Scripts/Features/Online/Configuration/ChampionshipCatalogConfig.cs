using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Online.Configuration
{
    /// <summary>
    /// The 5 recurring championship cadences from the Sprint 9 brief
    /// (Weekly/Monthly/Season/Weekend/Special Event), each with its own
    /// display data and headline reward — authored data, never hardcoded
    /// in <c>Championships.ChampionshipManager</c>.
    /// </summary>
    [CreateAssetMenu(fileName = "ChampionshipCatalogConfig", menuName = "GulfRun/Online/Championship Catalog Config")]
    public sealed class ChampionshipCatalogConfig : ScriptableObject
    {
        [Serializable]
        public sealed class ChampionshipEntry
        {
            [SerializeField] private string id;
            [SerializeField] private ChampionshipType type;
            [SerializeField] private string displayName = string.Empty;
            [SerializeField, TextArea] private string description = string.Empty;
            [SerializeField] private RewardType rewardType;

            [Tooltip("Coins/Gems amount when rewardType is Coins/Gems; ignored otherwise.")]
            [SerializeField] private int rewardAmount;

            [SerializeField] private string rewardDisplayName = string.Empty;
            [SerializeField] private Color placeholderColor = Color.white;

            public string Id => id;
            public ChampionshipType Type => type;
            public string DisplayName => string.IsNullOrEmpty(displayName) ? id : displayName;
            public string Description => description;
            public RewardType RewardType => rewardType;
            public int RewardAmount => rewardAmount;
            public string RewardDisplayName => rewardDisplayName;
            public Color PlaceholderColor => placeholderColor;
        }

        [SerializeField] private List<ChampionshipEntry> championships = new List<ChampionshipEntry>();

        public IReadOnlyList<ChampionshipEntry> Championships => championships;
    }
}

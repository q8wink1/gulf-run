using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Online.Configuration
{
    /// <summary>
    /// Every named, grantable reward across all 10 <see cref="RewardType"/>
    /// categories from the Sprint 9 brief — Championships/Country Events
    /// reference entries here by id rather than embedding reward data
    /// inline, so the same "Golden Falcon Title" (say) can be the prize for
    /// more than one event without duplicating its definition.
    /// Skins/Outfits/Victory Poses/Limited Cosmetics reuse Sprint 8's
    /// <see cref="CosmeticId"/> (resolved against
    /// <c>Features.Character.Configuration.CosmeticCatalogConfig</c> by
    /// whatever UI displays them — this catalog only needs the id, since
    /// Features.Online never references Features.Character directly).
    /// </summary>
    [CreateAssetMenu(fileName = "RewardCatalogConfig", menuName = "GulfRun/Online/Reward Catalog Config")]
    public sealed class RewardCatalogConfig : ScriptableObject
    {
        [Serializable]
        public sealed class RewardEntry
        {
            [SerializeField] private string id;
            [SerializeField] private RewardType type;
            [SerializeField] private string displayName = string.Empty;

            [Tooltip("Coins/Gems quantity when type is Coins/Gems; ignored otherwise.")]
            [SerializeField] private int amount;

            [Tooltip("Cosmetic id when type is a cosmetic-shaped reward (Skin/Outfit/VictoryPose/LimitedCosmetic); ignored otherwise.")]
            [SerializeField] private string cosmeticId = string.Empty;

            [SerializeField] private Color placeholderColor = Color.white;

            public string Id => id;
            public RewardType Type => type;
            public string DisplayName => string.IsNullOrEmpty(displayName) ? id : displayName;
            public int Amount => amount;
            public CosmeticId Cosmetic => new CosmeticId(cosmeticId);
            public Color PlaceholderColor => placeholderColor;

            public RewardGrant ToGrant() => new RewardGrant(type, amount, Cosmetic, DisplayName);
        }

        [SerializeField] private List<RewardEntry> rewards = new List<RewardEntry>();

        private readonly Dictionary<string, RewardEntry> _byId = new Dictionary<string, RewardEntry>();
        private bool _indexed;

        public IReadOnlyList<RewardEntry> Rewards => rewards;

        public bool TryGetEntry(string id, out RewardEntry entry)
        {
            EnsureIndexed();
            return _byId.TryGetValue(id, out entry);
        }

        private void EnsureIndexed()
        {
            if (_indexed)
            {
                return;
            }

            _byId.Clear();
            for (int i = 0; i < rewards.Count; i++)
            {
                if (rewards[i] != null && !string.IsNullOrEmpty(rewards[i].Id))
                {
                    _byId[rewards[i].Id] = rewards[i];
                }
            }

            _indexed = true;
        }

#if UNITY_EDITOR
        private void OnValidate() => _indexed = false;
#endif
    }
}

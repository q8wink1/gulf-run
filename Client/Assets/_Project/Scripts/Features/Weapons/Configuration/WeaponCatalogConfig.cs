using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Weapons.Configuration
{
    /// <summary>
    /// The single source of truth for "which 10 weapons exist and how likely
    /// is each" — referenced by <c>WeaponAuthority</c> (the only thing that
    /// rolls Item Box pickups) and by <c>WeaponInventoryManager</c>/
    /// <c>WeaponEffectApplicator</c> for weapon-id -&gt; definition lookups.
    /// Mirrors the role <c>SpawnCategoryConfig</c> plays for Sprint 3 content
    /// and <c>NetworkSyncConfig</c> plays for Sprint 4 — one data asset per
    /// feature, no tuning values hardcoded in code.
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponCatalogConfig", menuName = "GulfRun/Weapons/Weapon Catalog Config")]
    public sealed class WeaponCatalogConfig : ScriptableObject
    {
        [SerializeField] private List<WeaponDefinition> weapons = new List<WeaponDefinition>();

        [Tooltip("Chance [0..1] that a granted pickup rolls the Legendary weapon instead of a Standard one. Ignored once the Legendary has already been granted this match (WeaponAuthority allows at most one per match).")]
        [Range(0f, 1f)]
        [SerializeField] private float legendarySpawnChance01 = 0.03f;

        private readonly Dictionary<WeaponId, WeaponDefinition> _byId = new Dictionary<WeaponId, WeaponDefinition>();
        private readonly List<WeightedOption<WeaponId>> _standardOptionsScratch = new List<WeightedOption<WeaponId>>();
        private bool _indexed;

        public float LegendarySpawnChance01 => legendarySpawnChance01;
        public IReadOnlyList<WeaponDefinition> Weapons => weapons;

        public WeaponId LegendaryWeaponId
        {
            get
            {
                EnsureIndexed();
                for (int i = 0; i < weapons.Count; i++)
                {
                    if (weapons[i] != null && weapons[i].Rarity == WeaponRarity.Legendary)
                    {
                        return weapons[i].Id;
                    }
                }

                return default;
            }
        }

        public WeaponDefinition GetDefinition(WeaponId id)
        {
            EnsureIndexed();
            return _byId.TryGetValue(id, out WeaponDefinition definition) ? definition : null;
        }

        /// <summary>Weighted options over Standard-rarity weapons only, for <see cref="WeightedSelector"/>. Cached/reused list — not reentrant, matching SpawnCategoryConfig's documented usage.</summary>
        public IReadOnlyList<WeightedOption<WeaponId>> GetStandardWeightedOptions()
        {
            _standardOptionsScratch.Clear();
            for (int i = 0; i < weapons.Count; i++)
            {
                WeaponDefinition definition = weapons[i];
                if (definition != null && definition.Rarity == WeaponRarity.Standard)
                {
                    _standardOptionsScratch.Add(new WeightedOption<WeaponId>(definition.Id, definition.StandardSpawnWeight));
                }
            }

            return _standardOptionsScratch;
        }

        private void EnsureIndexed()
        {
            if (_indexed)
            {
                return;
            }

            _byId.Clear();
            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i] != null)
                {
                    _byId[weapons[i].Id] = weapons[i];
                }
            }

            _indexed = true;
        }

#if UNITY_EDITOR
        private void OnValidate() => _indexed = false;
#endif
    }
}

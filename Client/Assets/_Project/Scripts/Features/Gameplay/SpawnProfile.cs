using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.7 — map-specific spawn tuning (Kuwait / Dubai / Doha / Muscat…).
    /// Swap this asset on <see cref="SpawnManager"/> per Gulf map. Groups are
    /// independent; no prefabs or instantiation live here yet.
    /// </summary>
    [CreateAssetMenu(
        fileName = "SpawnProfile",
        menuName = "GulfRun/Gameplay/Spawn Profile")]
    public sealed class SpawnProfile : ScriptableObject
    {
        [Tooltip("Stable id for map catalogs (e.g. Kuwait, Dubai, Doha, Muscat).")]
        [SerializeField] private string profileId = "Default";

        [SerializeField] private List<SpawnGroupSettings> groups = new List<SpawnGroupSettings>();

        public string ProfileId => profileId;
        public IReadOnlyList<SpawnGroupSettings> Groups => groups;

        /// <summary>First enabled settings for <paramref name="category"/>, or null.</summary>
        public SpawnGroupSettings GetGroup(SpawnCategory category)
        {
            if (groups == null)
            {
                return null;
            }

            for (int i = 0; i < groups.Count; i++)
            {
                SpawnGroupSettings group = groups[i];
                if (group != null && group.Category == category)
                {
                    return group;
                }
            }

            return null;
        }

        /// <summary>Ensures the six Sprint 23.7 gameplay categories exist (editor / asset bootstrap).</summary>
        public void EnsureDefaultGroups()
        {
            if (groups == null)
            {
                groups = new List<SpawnGroupSettings>();
            }

            EnsureGroup(SpawnCategory.Obstacle, enabled: true, probability: 0.5f, density: 0.65f, min: 8f, max: 24f);
            EnsureGroup(SpawnCategory.Coin, enabled: true, probability: 0.75f, density: 0.85f, min: 3f, max: 12f);
            EnsureGroup(SpawnCategory.Gem, enabled: true, probability: 0.2f, density: 0.35f, min: 18f, max: 48f);
            EnsureGroup(SpawnCategory.PowerUp, enabled: true, probability: 0.25f, density: 0.4f, min: 20f, max: 55f);
            EnsureGroup(SpawnCategory.Decoration, enabled: true, probability: 0.6f, density: 0.7f, min: 4f, max: 16f);
            EnsureGroup(SpawnCategory.Npc, enabled: false, probability: 0.15f, density: 0.25f, min: 30f, max: 80f);
        }

        private void EnsureGroup(
            SpawnCategory category,
            bool enabled,
            float probability,
            float density,
            float min,
            float max)
        {
            if (GetGroup(category) != null)
            {
                return;
            }

            groups.Add(new SpawnGroupSettings(category, enabled, probability, density, min, max));
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (groups == null)
            {
                return;
            }

            for (int i = 0; i < groups.Count; i++)
            {
                groups[i]?.ClampForEditor();
            }
        }
#endif
    }
}

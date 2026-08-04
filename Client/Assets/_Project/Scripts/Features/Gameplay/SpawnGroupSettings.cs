using System;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.7 — Inspector tuning for one independent spawn category group.
    /// Used by <see cref="SpawnProfile"/>; does not spawn objects.
    /// </summary>
    [Serializable]
    public sealed class SpawnGroupSettings
    {
        [SerializeField] private SpawnCategory category = SpawnCategory.Obstacle;

        [Tooltip("When false, markers of this category are ignored.")]
        [SerializeField] private bool enabled = true;

        [Tooltip("Chance [0..1] that an eligible marker becomes a planned slot.")]
        [Range(0f, 1f)]
        [SerializeField] private float spawnProbability = 0.55f;

        [Tooltip("Fraction [0..1] of category markers considered before probability/spacing.")]
        [Range(0f, 1f)]
        [SerializeField] private float spawnDensity = 0.7f;

        [Tooltip("Minimum world-space distance along +Z from the previous planned slot in this group.")]
        [Min(0f)]
        [SerializeField] private float minimumSpacing = 6f;

        [Tooltip("Soft maximum +Z gap; when exceeded, the next eligible marker is forced through probability.")]
        [Min(0f)]
        [SerializeField] private float maximumSpacing = 28f;

        public SpawnCategory Category => category;
        public bool Enabled => enabled;
        public float SpawnProbability => spawnProbability;
        public float SpawnDensity => spawnDensity;
        public float MinimumSpacing => minimumSpacing;
        public float MaximumSpacing => maximumSpacing;

        public SpawnGroupSettings()
        {
        }

        public SpawnGroupSettings(
            SpawnCategory category,
            bool enabled,
            float spawnProbability,
            float spawnDensity,
            float minimumSpacing,
            float maximumSpacing)
        {
            this.category = category;
            this.enabled = enabled;
            this.spawnProbability = Mathf.Clamp01(spawnProbability);
            this.spawnDensity = Mathf.Clamp01(spawnDensity);
            this.minimumSpacing = Mathf.Max(0f, minimumSpacing);
            this.maximumSpacing = Mathf.Max(0f, maximumSpacing);
        }

#if UNITY_EDITOR
        public void ClampForEditor()
        {
            spawnProbability = Mathf.Clamp01(spawnProbability);
            spawnDensity = Mathf.Clamp01(spawnDensity);
            minimumSpacing = Mathf.Max(0f, minimumSpacing);
            maximumSpacing = Mathf.Max(minimumSpacing, maximumSpacing);
        }
#endif
    }
}

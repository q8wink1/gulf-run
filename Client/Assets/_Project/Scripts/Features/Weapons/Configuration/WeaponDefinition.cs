using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Weapons.Configuration
{
    /// <summary>
    /// Data-driven tuning for exactly one <see cref="WeaponId"/> — rarity,
    /// targeting, gameplay effect, and every presentation hook (icon, sounds,
    /// particles, animation trigger) the brief requires to be unique per
    /// weapon. One asset instance per weapon (10 total); adding an 11th
    /// weapon is authoring a new asset and adding it to
    /// <see cref="WeaponCatalogConfig"/>, never a code change.
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponDefinition", menuName = "GulfRun/Weapons/Weapon Definition")]
    public sealed class WeaponDefinition : ScriptableObject
    {
        [SerializeField] private WeaponId id;
        [SerializeField] private string displayName;
        [SerializeField] private WeaponRarity rarity = WeaponRarity.Standard;
        [SerializeField] private WeaponTargetingType targetingType = WeaponTargetingType.NearestOpponent;
        [SerializeField] private WeaponEffectFlags effectFlags = WeaponEffectFlags.None;

        [Tooltip("Effect strength; meaning depends on EffectFlags (e.g. a 0-1 speed multiplier for Slow/TractionLoss, a >1 multiplier for SpeedBoost).")]
        [SerializeField] private float magnitude = 1f;

        [SerializeField] private float durationSeconds = 3f;

        [Tooltip("Relative pick weight among Standard weapons only (WeightedSelector). Ignored for the Legendary weapon — see WeaponCatalogConfig.LegendarySpawnChance01.")]
        [SerializeField] private float standardSpawnWeight = 1f;

        [Tooltip("Optional short cooldown enforced client-side after activation, purely to prevent spamming multiple weapons per frame.")]
        [SerializeField] private float cooldownSeconds = 0.5f;

        [Header("Presentation (icon/audio/particle assets TODO once available — same 'no final art yet' status as Sprints 2-4)")]
        [SerializeField] private Sprite icon;
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private AudioClip activationSound;
        [SerializeField] private AudioClip impactSound;
        [SerializeField] private AudioClip cooldownSound;
        [SerializeField] private GameObject activationParticlePrefab;
        [SerializeField] private GameObject impactParticlePrefab;
        [SerializeField] private string activationAnimatorTrigger = string.Empty;

        public WeaponId Id => id;
        public string DisplayName => string.IsNullOrEmpty(displayName) ? id.ToString() : displayName;
        public WeaponRarity Rarity => rarity;
        public WeaponTargetingType TargetingType => targetingType;
        public WeaponEffectFlags EffectFlags => effectFlags;
        public float Magnitude => magnitude;
        public float DurationSeconds => durationSeconds;
        public float StandardSpawnWeight => standardSpawnWeight;
        public float CooldownSeconds => cooldownSeconds;
        public Sprite Icon => icon;
        public AudioClip PickupSound => pickupSound;
        public AudioClip ActivationSound => activationSound;
        public AudioClip ImpactSound => impactSound;
        public AudioClip CooldownSound => cooldownSound;
        public GameObject ActivationParticlePrefab => activationParticlePrefab;
        public GameObject ImpactParticlePrefab => impactParticlePrefab;
        public string ActivationAnimatorTrigger => activationAnimatorTrigger;
    }
}

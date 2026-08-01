using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Traps.Configuration
{
    /// <summary>
    /// Data-driven tuning for exactly one <see cref="TrapId"/> — the effect
    /// it applies (reusing Sprint 5's <see cref="WeaponEffectFlags"/>
    /// vocabulary, see that type's remarks), how long it stays active in the
    /// world (~15s per the brief, but configurable per trap), its relative
    /// spawn weight, optional movement/continuous-contact behaviour, and
    /// presentation hooks. One asset instance per trap (15 total); adding a
    /// 16th trap is authoring a new asset and adding it to
    /// <see cref="TrapCatalogConfig"/>, never a code change.
    /// </summary>
    [CreateAssetMenu(fileName = "TrapDefinition", menuName = "GulfRun/Traps/Trap Definition")]
    public sealed class TrapDefinition : ScriptableObject
    {
        [SerializeField] private TrapId id;
        [SerializeField] private string displayName;
        [SerializeField] private WeaponEffectFlags effectFlags = WeaponEffectFlags.None;

        [Tooltip("Effect strength; meaning depends on EffectFlags (e.g. a 0-1 speed multiplier for Slow/TractionLoss, meters of setback for LateralPush).")]
        [SerializeField] private float magnitude = 1f;

        [Tooltip("How long the applied effect lasts on a triggered player. Ignored for LateralPush, which is instantaneous.")]
        [SerializeField] private float durationSeconds = 1.5f;

        [Tooltip("How long this trap instance remains active in the world before expiring back to the pool (\"approximately 15 seconds\" per the brief; varies slightly per trap for character).")]
        [SerializeField] private float lifetimeSeconds = 15f;

        [Tooltip("Relative pick weight for WeightedSelector — the 'trap combinations' half of Randomization.")]
        [SerializeField] private float spawnWeight = 1f;

        [Tooltip("Hot Sand: keeps re-applying the effect on a refresh timer for as long as a player stands inside, instead of once on contact.")]
        [SerializeField] private bool continuousWhileStanding;

        [SerializeField] private float continuousRefreshIntervalSeconds = 0.5f;

        [Tooltip("Angry Camel / Rolling Barrel / Goat Herd / Dust Tornado: the trap itself drifts along the run axis for its lifetime instead of sitting still.")]
        [SerializeField] private bool movesAlongTrack;

        [SerializeField] private float moveSpeedMetersPerSecond;

        [Header("Presentation (art/audio TODO once available — same 'no final art yet' status as Sprints 2-5)")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private Color debugTint = Color.white;
        [SerializeField] private Sprite icon;
        [SerializeField] private AudioClip appearSound;
        [SerializeField] private AudioClip triggerSound;
        [SerializeField] private GameObject impactParticlePrefab;
        [SerializeField] private string triggerAnimatorTrigger = string.Empty;

        public TrapId Id => id;
        public string DisplayName => string.IsNullOrEmpty(displayName) ? id.ToString() : displayName;
        public WeaponEffectFlags EffectFlags => effectFlags;
        public float Magnitude => magnitude;
        public float DurationSeconds => durationSeconds;
        public float LifetimeSeconds => lifetimeSeconds;
        public float SpawnWeight => spawnWeight;
        public bool ContinuousWhileStanding => continuousWhileStanding;
        public float ContinuousRefreshIntervalSeconds => continuousRefreshIntervalSeconds;
        public bool MovesAlongTrack => movesAlongTrack;
        public float MoveSpeedMetersPerSecond => moveSpeedMetersPerSecond;
        public GameObject Prefab => prefab;
        public Color DebugTint => debugTint;
        public Sprite Icon => icon;
        public AudioClip AppearSound => appearSound;
        public AudioClip TriggerSound => triggerSound;
        public GameObject ImpactParticlePrefab => impactParticlePrefab;
        public string TriggerAnimatorTrigger => triggerAnimatorTrigger;
    }
}

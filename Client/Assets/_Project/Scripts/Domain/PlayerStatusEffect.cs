namespace GulfRun.Domain
{
    /// <summary>
    /// A resolved, ready-to-apply gameplay effect targeting one player — the
    /// output of <see cref="WeaponEffectResolver"/> (Sprint 5 weapon hits)
    /// or constructed directly by <c>Features.Traps.Effects.TrapEffectApplicator</c>
    /// (Sprint 6 trap triggers). Pure data so the exact same resolution can
    /// run identically on a future dedicated server and on every client,
    /// regardless of which feature caused it — see <see cref="EffectSourceKind"/>.
    /// </summary>
    public readonly struct PlayerStatusEffect
    {
        public readonly WeaponEffectFlags Flags;
        public readonly double DurationSeconds;
        public readonly float Magnitude;
        public readonly EffectSourceKind SourceKind;

        /// <summary>The causing <see cref="WeaponId"/> or <see cref="TrapId"/>, cast to int per <see cref="SourceKind"/> — presentation/debug metadata only, never branched on by gameplay logic.</summary>
        public readonly int SourceId;

        public PlayerStatusEffect(WeaponEffectFlags flags, double durationSeconds, float magnitude, EffectSourceKind sourceKind, int sourceId)
        {
            Flags = flags;
            DurationSeconds = durationSeconds;
            Magnitude = magnitude;
            SourceKind = sourceKind;
            SourceId = sourceId;
        }

        public bool Has(WeaponEffectFlags flag) => (Flags & flag) != 0;
    }
}

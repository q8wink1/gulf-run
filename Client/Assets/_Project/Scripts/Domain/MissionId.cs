using System;

namespace GulfRun.Domain
{
    /// <summary>
    /// Strongly-typed identifier for one entry in the Daily Missions pool —
    /// the same plain-string-wrapper shape as <see cref="StoreItemId"/>/
    /// <see cref="CosmeticId"/> so "Mission pool must be configurable"
    /// (Sprint 11 brief) means authoring a new
    /// <c>Features.Progression.Configuration.MissionPoolCatalogConfig</c>
    /// row, never a code change.
    /// </summary>
    public readonly struct MissionId : IEquatable<MissionId>
    {
        public static readonly MissionId None = new MissionId(string.Empty);

        public readonly string Value;

        public MissionId(string value)
        {
            Value = value ?? string.Empty;
        }

        public bool IsNone => string.IsNullOrEmpty(Value);

        public bool Equals(MissionId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is MissionId other && Equals(other);

        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

        public override string ToString() => Value;

        public static bool operator ==(MissionId a, MissionId b) => a.Equals(b);

        public static bool operator !=(MissionId a, MissionId b) => !a.Equals(b);
    }
}

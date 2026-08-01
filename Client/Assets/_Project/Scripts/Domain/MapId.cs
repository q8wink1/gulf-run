using System;

namespace GulfRun.Domain
{
    /// <summary>
    /// Strongly-typed identifier for one launch map (Kuwait City, Riyadh,
    /// Dubai, Doha, Manama, Muscat, ...). A plain string wrapper for the
    /// same "unlimited future content, zero code changes" reason as
    /// <see cref="CosmeticId"/>/<see cref="CharacterId"/> — a seventh map
    /// is authored as a new row in <c>Features.Maps.Configuration.
    /// MapCatalogConfig</c>, never a new enum member.
    /// </summary>
    public readonly struct MapId : IEquatable<MapId>
    {
        public static readonly MapId None = new MapId(string.Empty);

        public readonly string Value;

        public MapId(string value)
        {
            Value = value ?? string.Empty;
        }

        public bool IsNone => string.IsNullOrEmpty(Value);

        public bool Equals(MapId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is MapId other && Equals(other);

        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

        public override string ToString() => Value;

        public static bool operator ==(MapId a, MapId b) => a.Equals(b);

        public static bool operator !=(MapId a, MapId b) => !a.Equals(b);
    }
}

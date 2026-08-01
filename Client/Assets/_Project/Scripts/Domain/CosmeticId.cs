using System;

namespace GulfRun.Domain
{
    /// <summary>
    /// Strongly-typed identifier for one cosmetic item (a Traditional Outfit,
    /// a premium Football Club Kit, a future Hat/Emote/Trail/...). A plain
    /// string wrapper for the same "unlimited future content, zero code
    /// changes" reason as <see cref="CharacterId"/> — new cosmetics are
    /// authored as data rows in <c>Features.Character.Configuration.
    /// CosmeticCatalogConfig</c>, never a new enum member.
    /// </summary>
    public readonly struct CosmeticId : IEquatable<CosmeticId>
    {
        public static readonly CosmeticId None = new CosmeticId(string.Empty);

        public readonly string Value;

        public CosmeticId(string value)
        {
            Value = value ?? string.Empty;
        }

        public bool IsNone => string.IsNullOrEmpty(Value);

        public bool Equals(CosmeticId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is CosmeticId other && Equals(other);

        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

        public override string ToString() => Value;

        public static bool operator ==(CosmeticId a, CosmeticId b) => a.Equals(b);

        public static bool operator !=(CosmeticId a, CosmeticId b) => !a.Equals(b);
    }
}

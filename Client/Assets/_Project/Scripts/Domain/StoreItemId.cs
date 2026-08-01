using System;

namespace GulfRun.Domain
{
    /// <summary>
    /// Strongly-typed identifier for one purchasable Store product — a Gem
    /// Package, Coin Pack, generic Store Item (Character/Outfit/Emote/
    /// Victory Pose/Visual Effect/Profile Frame), Special Offer, or the
    /// Battle Pass itself. Same plain string-wrapper shape as
    /// <see cref="CosmeticId"/>/<see cref="CharacterId"/> — new products are
    /// authored as data rows in one of the <c>Features.Store.Configuration</c>
    /// catalogs, never a new enum member or code change.
    /// </summary>
    public readonly struct StoreItemId : IEquatable<StoreItemId>
    {
        public static readonly StoreItemId None = new StoreItemId(string.Empty);

        public readonly string Value;

        public StoreItemId(string value)
        {
            Value = value ?? string.Empty;
        }

        public bool IsNone => string.IsNullOrEmpty(Value);

        public bool Equals(StoreItemId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is StoreItemId other && Equals(other);

        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

        public override string ToString() => Value;

        public static bool operator ==(StoreItemId a, StoreItemId b) => a.Equals(b);

        public static bool operator !=(StoreItemId a, StoreItemId b) => !a.Equals(b);
    }
}

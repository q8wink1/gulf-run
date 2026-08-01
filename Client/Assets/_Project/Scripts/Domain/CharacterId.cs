using System;

namespace GulfRun.Domain
{
    /// <summary>
    /// Strongly-typed identifier for a playable character. A plain string
    /// wrapper — not an enum like <see cref="Domain.WeaponId"/>/<c>TrapId</c> —
    /// because Sprint 8 explicitly requires "prepare support for unlimited
    /// future characters": adding character #13 must be authoring a new
    /// <c>Features.Character.Configuration.CharacterDefinition</c> asset and
    /// dropping it into the catalog, never a code change/recompile.
    /// </summary>
    public readonly struct CharacterId : IEquatable<CharacterId>
    {
        public static readonly CharacterId None = new CharacterId(string.Empty);

        public readonly string Value;

        public CharacterId(string value)
        {
            Value = value ?? string.Empty;
        }

        public bool IsNone => string.IsNullOrEmpty(Value);

        public bool Equals(CharacterId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is CharacterId other && Equals(other);

        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

        public override string ToString() => Value;

        public static bool operator ==(CharacterId a, CharacterId b) => a.Equals(b);

        public static bool operator !=(CharacterId a, CharacterId b) => !a.Equals(b);
    }
}

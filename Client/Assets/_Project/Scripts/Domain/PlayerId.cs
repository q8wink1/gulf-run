using System;

namespace GulfRun.Domain
{
    /// <summary>
    /// A permanent, backend-facing player identifier — the online-ecosystem
    /// counterpart to <see cref="CharacterId"/>/<see cref="CosmeticId"/>
    /// (Sprint 8): a string-wrapped <c>readonly struct</c>, not an enum,
    /// since the population of players is obviously unbounded. Distinct
    /// from <see cref="PlayerIdentity.PlayerId"/> (a plain <c>string</c>
    /// minted per-session by <c>LocalPlayerIdentity.CreateLocal</c> for
    /// match/connection purposes since Sprint 4): this one is minted
    /// exactly once by <c>Core.Save.IAccountRepository.CreateAccount</c>
    /// (Sprint 9 extends <see cref="PlayerAccount"/> with it) and never
    /// changes for the lifetime of the account, which is what a stable
    /// Leaderboard/Friends/Hall of Fame identity requires.
    /// </summary>
    public readonly struct PlayerId : IEquatable<PlayerId>
    {
        public static readonly PlayerId None = new PlayerId(string.Empty);

        public readonly string Value;

        public PlayerId(string value)
        {
            Value = value ?? string.Empty;
        }

        public bool IsNone => string.IsNullOrEmpty(Value);

        public bool Equals(PlayerId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is PlayerId other && Equals(other);

        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

        public override string ToString() => Value;

        public static bool operator ==(PlayerId a, PlayerId b) => a.Equals(b);

        public static bool operator !=(PlayerId a, PlayerId b) => !a.Equals(b);
    }
}

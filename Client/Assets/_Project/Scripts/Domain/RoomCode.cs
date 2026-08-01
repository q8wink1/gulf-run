using System;

namespace GulfRun.Domain
{
    /// <summary>
    /// A short, human-shareable code identifying a hosted Private Room
    /// (Sprint 13 "SOCIAL: Room Code") — the presentation-friendly value
    /// on top of the existing raw <c>joinCode</c> string
    /// <see cref="Features.Multiplayer.Session.SessionManager.JoinMatch"/>
    /// already accepts. Excludes visually-ambiguous characters (0/O, 1/I)
    /// so it reads and re-types cleanly on a small mobile screen.
    /// </summary>
    public readonly struct RoomCode : IEquatable<RoomCode>
    {
        public string Value { get; }

        public RoomCode(string value)
        {
            Value = value ?? string.Empty;
        }

        public bool IsNone => string.IsNullOrEmpty(Value);

        public static RoomCode None => new RoomCode(string.Empty);

        public bool Equals(RoomCode other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is RoomCode other && Equals(other);

        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

        public override string ToString() => IsNone ? "—" : Value;
    }

    /// <summary>Pure generator for <see cref="RoomCode"/> values — no UnityEngine dependency, same "engine-free Domain" posture as <see cref="CelebrationAnimation"/>.</summary>
    public static class RoomCodeGenerator
    {
        private const int Length = 6;

        // Uppercase letters + digits, minus 0/O and 1/I (visually ambiguous on small screens).
        private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        public static RoomCode Generate(Random random)
        {
            if (random == null)
            {
                return RoomCode.None;
            }

            char[] chars = new char[Length];
            for (int i = 0; i < Length; i++)
            {
                chars[i] = Alphabet[random.Next(Alphabet.Length)];
            }

            return new RoomCode(new string(chars));
        }
    }
}

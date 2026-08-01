using System;

namespace GulfRun.Domain
{
    /// <summary>
    /// Deterministic <see cref="IRandomSource"/> backed by <see cref="System.Random"/>
    /// (not UnityEngine.Random, whose global mutable state is unsuitable for
    /// per-system seeding/testability). Same seed always produces the same
    /// sequence, satisfying Sprint 3's "Random seed support" requirement.
    /// </summary>
    public sealed class SeededRandom : IRandomSource
    {
        private readonly Random _random;

        public int Seed { get; }

        public SeededRandom(int seed)
        {
            Seed = seed;
            _random = new Random(seed);
        }

        /// <summary>Creates a non-deterministic instance seeded from the current time.</summary>
        public static SeededRandom FromTime() => new SeededRandom(Environment.TickCount);

        public float NextFloat01() => (float)_random.NextDouble();

        public int NextInt(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);
    }
}

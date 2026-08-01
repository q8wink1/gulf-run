namespace GulfRun.Domain
{
    /// <summary>
    /// Abstraction over a random number source. Exists so spawn/weighted-pick
    /// logic depends on an interface (dependency inversion) rather than a
    /// concrete RNG, enabling deterministic seeded runs and unit testing
    /// without any UnityEngine dependency.
    /// </summary>
    public interface IRandomSource
    {
        /// <summary>Returns a value in [0, 1).</summary>
        float NextFloat01();

        /// <summary>Returns a value in [minInclusive, maxExclusive).</summary>
        int NextInt(int minInclusive, int maxExclusive);
    }
}

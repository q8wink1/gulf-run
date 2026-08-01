namespace GulfRun.Domain
{
    /// <summary>
    /// A single (value, weight) pair for weighted-random selection. Generic and
    /// engine-agnostic so it can carry chunk prefabs, spawnable prefabs, or any
    /// future biome/seasonal-event variant without duplicating selection logic.
    /// </summary>
    public readonly struct WeightedOption<T>
    {
        public readonly T Value;
        public readonly float Weight;

        public WeightedOption(T value, float weight)
        {
            Value = value;
            Weight = weight;
        }
    }
}

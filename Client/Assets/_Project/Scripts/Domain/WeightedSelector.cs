using System.Collections.Generic;

namespace GulfRun.Domain
{
    /// <summary>
    /// Pure weighted-random selection over any <see cref="WeightedOption{T}"/>
    /// list. Shared by chunk-prefab selection and per-category spawnable
    /// selection so the weighting algorithm exists exactly once.
    /// </summary>
    public static class WeightedSelector
    {
        public static bool TrySelect<T>(IReadOnlyList<WeightedOption<T>> options, IRandomSource random, out T result)
        {
            result = default;

            if (options == null || options.Count == 0 || random == null)
            {
                return false;
            }

            float total = 0f;
            for (int i = 0; i < options.Count; i++)
            {
                float weight = options[i].Weight;
                total += weight > 0f ? weight : 0f;
            }

            if (total <= 0f)
            {
                return false;
            }

            float roll = random.NextFloat01() * total;
            float cumulative = 0f;
            for (int i = 0; i < options.Count; i++)
            {
                float weight = options[i].Weight;
                cumulative += weight > 0f ? weight : 0f;
                if (roll <= cumulative)
                {
                    result = options[i].Value;
                    return true;
                }
            }

            // Floating-point rounding safety net: fall back to the last option.
            result = options[options.Count - 1].Value;
            return true;
        }
    }
}

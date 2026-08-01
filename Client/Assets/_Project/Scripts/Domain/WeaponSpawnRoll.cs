namespace GulfRun.Domain
{
    /// <summary>
    /// Pure Item Box roll math: whether this grant should be the Legendary
    /// weapon, given its (extremely low) spawn chance and whether one has
    /// already been granted this match ("normally only one Legendary weapon
    /// may appear during a match"). Standard-weapon selection itself reuses
    /// the existing generic <see cref="WeightedSelector"/> — no duplicated
    /// weighted-pick algorithm.
    /// </summary>
    public static class WeaponSpawnRoll
    {
        public static bool ShouldRollLegendary(IRandomSource random, float legendarySpawnChance01, bool legendaryAlreadyGrantedThisMatch)
        {
            if (legendaryAlreadyGrantedThisMatch || random == null || legendarySpawnChance01 <= 0f)
            {
                return false;
            }

            return random.NextFloat01() < legendarySpawnChance01;
        }
    }
}

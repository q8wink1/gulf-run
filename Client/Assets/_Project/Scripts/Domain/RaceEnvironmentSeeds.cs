namespace GulfRun.Domain
{
    /// <summary>
    /// The two per-match random seeds the Sprint 12 debug overlay must show
    /// ("Trap Seed, Item Box Seed") and that
    /// <c>Features.Traps.Authority.TrapAuthority</c> /
    /// <c>Features.EndlessRunner.Spawning.ChunkContentSpawner</c> re-seed
    /// themselves from at the start of every match — the concrete mechanism
    /// behind "Trap locations change every match. Weapon boxes change every
    /// match. Spawn positions are randomized."
    /// </summary>
    public readonly struct RaceEnvironmentSeeds
    {
        public readonly int TrapSeed;
        public readonly int ItemBoxSeed;

        public RaceEnvironmentSeeds(int trapSeed, int itemBoxSeed)
        {
            TrapSeed = trapSeed;
            ItemBoxSeed = itemBoxSeed;
        }
    }
}

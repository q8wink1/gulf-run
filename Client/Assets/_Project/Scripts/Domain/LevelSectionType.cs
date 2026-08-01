namespace GulfRun.Domain
{
    /// <summary>
    /// The Sprint 12 "Level Structure" vocabulary every launch map is built
    /// from. Purely descriptive metadata carried on a chunk prefab (see
    /// <c>Features.EndlessRunner.WorldGeneration.Chunk.SectionType</c>) —
    /// adding a new section variant later is a new tagged chunk prefab plus
    /// a weighted <c>WorldGenerationConfig</c> entry, never a code change
    /// here or in <c>WorldGenerator</c>.
    /// </summary>
    public enum LevelSectionType
    {
        FlatSection,
        SmallHill,
        Slope,
        Bridge,
        WoodPlatform,
        StonePlatform,
        JumpPlatform,
        ShortTunnel,
        OpenArea,
        SmallDrop,
        SmallClimb
    }
}

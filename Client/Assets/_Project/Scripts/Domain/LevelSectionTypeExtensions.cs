namespace GulfRun.Domain
{
    /// <summary>
    /// Small classification helper so debug/UI/gameplay code never needs its
    /// own switch statement just to ask "is this one of the three platform
    /// section types" (Sprint 12 brief: "PLATFORMS: Platforms are part of
    /// gameplay ... never block the race unfairly").
    /// </summary>
    public static class LevelSectionTypeExtensions
    {
        public static bool IsPlatform(this LevelSectionType type) =>
            type == LevelSectionType.WoodPlatform ||
            type == LevelSectionType.StonePlatform ||
            type == LevelSectionType.JumpPlatform;
    }
}

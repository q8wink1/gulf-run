using System;

namespace GulfRun.Domain
{
    /// <summary>
    /// Which Sprint 12 "BACKGROUND" animated elements a given map supports
    /// (brief: "Animated clouds. Flying birds. Palm trees moving gently.
    /// Flags waving. City lights at night. Moving traffic in background.
    /// Sea waves where appropriate."). A [Flags] enum so
    /// <c>Features.Maps.Configuration.MapCatalogConfig</c> can author
    /// "where appropriate" per city as plain data, with zero per-map
    /// branching in code.
    /// </summary>
    [Flags]
    public enum BackgroundElementFlags
    {
        None = 0,
        Clouds = 1 << 0,
        Birds = 1 << 1,
        PalmTrees = 1 << 2,
        WavingFlags = 1 << 3,
        CityLightsAtNight = 1 << 4,
        Traffic = 1 << 5,
        SeaWaves = 1 << 6
    }
}

namespace GulfRun.Domain
{
    /// <summary>
    /// Purely visual weather chosen once per race (Sprint 12 brief:
    /// "WEATHER: Visual only"). Rain/Fog/Sandstorm are the brief's explicit
    /// "Future support" entries — present here so
    /// <c>Features.Maps.Configuration.WeatherCatalogConfig</c> can carry a
    /// catalog row (at zero selection weight, today) for each without a
    /// future enum/code change.
    /// </summary>
    public enum WeatherType
    {
        Sunny,
        Cloudy,
        LightWind,
        DustySky,

        /// <summary>Future support (Sprint 12 brief) — zero-weighted in the launch catalog.</summary>
        Rain,

        /// <summary>Future support (Sprint 12 brief) — zero-weighted in the launch catalog.</summary>
        Fog,

        /// <summary>Future support (Sprint 12 brief) — zero-weighted in the launch catalog.</summary>
        Sandstorm
    }
}

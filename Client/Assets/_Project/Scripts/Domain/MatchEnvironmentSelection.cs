namespace GulfRun.Domain
{
    /// <summary>
    /// One fully-resolved "before every race" environment: which map, what
    /// time of day, what weather, and this match's randomized seeds. Built
    /// exactly once per match by <c>Features.Maps.MapEnvironmentManager</c>
    /// and exposed read-only to every other feature through
    /// <c>Core.Services.IMapContextProvider</c>.
    /// </summary>
    public readonly struct MatchEnvironmentSelection
    {
        public readonly MapId Map;
        public readonly TimeOfDay TimeOfDay;
        public readonly WeatherType Weather;
        public readonly RaceEnvironmentSeeds Seeds;

        public MatchEnvironmentSelection(MapId map, TimeOfDay timeOfDay, WeatherType weather, RaceEnvironmentSeeds seeds)
        {
            Map = map;
            TimeOfDay = timeOfDay;
            Weather = weather;
            Seeds = seeds;
        }
    }
}

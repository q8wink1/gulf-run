namespace GulfRun.Domain
{
    /// <summary>
    /// Unique identifier for each of the 15 Sprint 6 map hazards. Traps
    /// belong to the map, not to any player (see <c>TrapDefinition</c>) —
    /// unlike <see cref="WeaponId"/>, nothing ever "owns" one of these.
    /// </summary>
    public enum TrapId
    {
        SandPit,
        FishingNet,
        AngryCamel,
        RollingBarrel,
        FallingPalm,
        LooseRocks,
        BrokenCart,
        ScorpionArea,
        HotSand,
        CollapsedBridge,
        GoatHerd,
        WaterBarrel,
        ConstructionBarrier,
        WindGust,
        DustTornado
    }
}

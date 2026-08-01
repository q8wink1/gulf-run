namespace GulfRun.Domain
{
    /// <summary>
    /// Pure trap spawn-position math: a random point somewhere between
    /// <paramref name="minAheadMeters"/> and <paramref name="maxAheadMeters"/>
    /// ahead of <paramref name="originX"/> (the local player's current run
    /// distance — see <c>ILocalPlayerStateProvider</c>), at a fixed ground
    /// height. This is what makes "trap positions must never be identical
    /// every match" and "randomize spawn positions" true by construction —
    /// every roll depends on the supplied <see cref="IRandomSource"/>, never
    /// a fixed layout.
    /// </summary>
    public static class TrapPositionRoll
    {
        public static NetVector2 NextPosition(IRandomSource random, float originX, float minAheadMeters, float maxAheadMeters, float groundY)
        {
            float low = minAheadMeters <= maxAheadMeters ? minAheadMeters : maxAheadMeters;
            float high = minAheadMeters <= maxAheadMeters ? maxAheadMeters : minAheadMeters;
            float ahead = random != null ? low + random.NextFloat01() * (high - low) : low;
            return new NetVector2(originX + ahead, groundY);
        }
    }
}

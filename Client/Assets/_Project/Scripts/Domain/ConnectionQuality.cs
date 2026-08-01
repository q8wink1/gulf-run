namespace GulfRun.Domain
{
    /// <summary>
    /// Player-Card-friendly bucketing of a participant's link health (Sprint
    /// 15 "PLAYER CARDS: Connection Quality" / "NETWORK: Latency indicator"),
    /// on top of the existing raw <see cref="ConnectionState"/> +
    /// millisecond ping already tracked by
    /// <c>Features.Multiplayer.Connection.ConnectionManager</c>. A UI only
    /// ever needs "how good is this link right now", not the raw number —
    /// see <see cref="ConnectionQualityResolver"/> for the pure mapping.
    /// </summary>
    public enum ConnectionQuality
    {
        Excellent,
        Good,
        Fair,
        Poor,

        /// <summary>Timed out, reconnecting, or disconnected — no meaningful ping to show.</summary>
        Disconnected
    }

    /// <summary>Pure ping-bucketing rules — no UnityEngine dependency, same "engine-free Domain" posture as <see cref="MatchmakingEtaEstimator"/>.</summary>
    public static class ConnectionQualityResolver
    {
        private const float ExcellentMaxMilliseconds = 60f;
        private const float GoodMaxMilliseconds = 120f;
        private const float FairMaxMilliseconds = 220f;

        /// <summary>Resolves a bucketed quality from live connection state + measured ping. Any non-<see cref="ConnectionState.Connected"/> state always reads as <see cref="ConnectionQuality.Disconnected"/> regardless of the last known ping.</summary>
        public static ConnectionQuality Resolve(ConnectionState state, float pingMilliseconds)
        {
            if (state != ConnectionState.Connected)
            {
                return ConnectionQuality.Disconnected;
            }

            if (pingMilliseconds <= ExcellentMaxMilliseconds)
            {
                return ConnectionQuality.Excellent;
            }

            if (pingMilliseconds <= GoodMaxMilliseconds)
            {
                return ConnectionQuality.Good;
            }

            return pingMilliseconds <= FairMaxMilliseconds ? ConnectionQuality.Fair : ConnectionQuality.Poor;
        }
    }
}

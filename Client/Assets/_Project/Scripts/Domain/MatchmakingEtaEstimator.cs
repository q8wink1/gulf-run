namespace GulfRun.Domain
{
    /// <summary>
    /// Pure "Estimated matchmaking time" heuristic for the Sprint 13 Main
    /// Menu bottom bar. No real matchmaking queue/backend exists yet (the
    /// default <see cref="Features.Multiplayer.Configuration.NetworkSyncConfig"/>
    /// + <c>LocalLoopbackTransport</c> combo resolves a match locally and
    /// instantly — see Sprint 4/5 reports) — this gives players an honest,
    /// non-zero "still waiting for players" readout instead of either a
    /// permanently misleading "0s" or an invented fake countdown, and
    /// naturally reaches 0 the moment enough players have actually joined.
    /// </summary>
    public static class MatchmakingEtaEstimator
    {
        /// <summary>Placeholder seconds-per-missing-player, pending a real matchmaking service (see class remarks).</summary>
        private const int SecondsPerMissingPlayer = 8;

        public static int EstimateSecondsRemaining(int currentPlayers, int requiredPlayers)
        {
            int missing = requiredPlayers - currentPlayers;
            return missing > 0 ? missing * SecondsPerMissingPlayer : 0;
        }
    }
}

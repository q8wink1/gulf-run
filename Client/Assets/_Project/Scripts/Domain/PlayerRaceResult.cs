namespace GulfRun.Domain
{
    /// <summary>
    /// One player's outcome for a single race: how it ended, when (relative
    /// to the shared race-start moment), how many coins they had, and how far
    /// they got. <see cref="FinishPosition"/> is intentionally left at -1
    /// ("not yet placed") when this is broadcast live as each player resolves
    /// (see <c>IMatchTransport.PlayerRaceResultReported</c>) — a player who is
    /// eliminated while others are still racing must not be assigned a
    /// permanent rank until the whole race is over, since finishers always
    /// rank above eliminated players regardless of resolution order. Only
    /// <see cref="RaceRanking.ComputeFinalPositions"/> ever produces a
    /// non-negative <see cref="FinishPosition"/>.
    /// </summary>
    public readonly struct PlayerRaceResult
    {
        public readonly int ConnectionId;
        public readonly FinishReason Reason;

        /// <summary>Seconds from the shared race start to this player's finish/elimination — the "Finish Time" / "Race Duration" the brief asks to record.</summary>
        public readonly double FinishTimeSeconds;
        public readonly int CoinsCollected;
        public readonly float DistanceMetersReached;

        /// <summary>1-based order in which this player resolved (finished or was eliminated) relative to everyone else, known immediately — used for live "who's finished so far" UI before the final ranking exists.</summary>
        public readonly int ResolutionOrder;

        /// <summary>1-based final rank; -1 until <see cref="RaceRanking.ComputeFinalPositions"/> has run.</summary>
        public readonly int FinishPosition;

        public PlayerRaceResult(
            int connectionId,
            FinishReason reason,
            double finishTimeSeconds,
            int coinsCollected,
            float distanceMetersReached,
            int resolutionOrder,
            int finishPosition)
        {
            ConnectionId = connectionId;
            Reason = reason;
            FinishTimeSeconds = finishTimeSeconds;
            CoinsCollected = coinsCollected;
            DistanceMetersReached = distanceMetersReached;
            ResolutionOrder = resolutionOrder;
            FinishPosition = finishPosition;
        }

        public PlayerRaceResult WithFinishPosition(int finishPosition) =>
            new PlayerRaceResult(ConnectionId, Reason, FinishTimeSeconds, CoinsCollected, DistanceMetersReached, ResolutionOrder, finishPosition);
    }
}

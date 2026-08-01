namespace GulfRun.Domain
{
    /// <summary>
    /// The local player's own outcome of one completed race — the payload
    /// <c>Core.Services.PlayerStatEventService.LocalMatchCompleted</c>
    /// carries from <c>Features.RaceFinish.Standings.RaceStandingsTracker</c>
    /// (which already resolves every one of these fields for Sprint 7's
    /// Reward Screen) to the Sprint 9 Statistics/League systems, without
    /// either feature depending on the other.
    /// </summary>
    public readonly struct PlayerMatchOutcome
    {
        public readonly int FinishPosition;
        public readonly FinishReason Reason;
        public readonly float FinishTimeSeconds;
        public readonly float DistanceMetersReached;
        public readonly int CoinsCollected;

        public PlayerMatchOutcome(int finishPosition, FinishReason reason, float finishTimeSeconds, float distanceMetersReached, int coinsCollected)
        {
            FinishPosition = finishPosition;
            Reason = reason;
            FinishTimeSeconds = finishTimeSeconds;
            DistanceMetersReached = distanceMetersReached;
            CoinsCollected = coinsCollected;
        }
    }
}

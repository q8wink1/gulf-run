namespace GulfRun.Domain
{
    /// <summary>How a player's race concluded — see <see cref="PlayerRaceResult"/>.</summary>
    public enum FinishReason
    {
        /// <summary>Crossed the finish line under their own progress.</summary>
        Completed,

        /// <summary>Fell too far behind and was automatically eliminated (see <see cref="RaceElimination"/>), or was still racing when the race's safety-net timeout elapsed.</summary>
        Eliminated
    }
}

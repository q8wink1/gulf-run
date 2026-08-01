namespace GulfRun.Core.Services
{
    /// <summary>
    /// Read-only view of the local player's live race progress, published by
    /// the EndlessRunner feature's <c>GameLoopController</c> and consumed by
    /// the RaceFinish feature to report progress for finish-line/elimination
    /// checks — same decoupling pattern as <see cref="IGameStateProvider"/>
    /// and <see cref="IRunSpeedProvider"/>, so neither feature ever
    /// references the other's assembly (see FOLDER_ARCHITECTURE.md §4).
    /// </summary>
    public interface IRaceProgressProvider
    {
        double DistanceMeters { get; }
        int CoinsCollected { get; }
    }
}

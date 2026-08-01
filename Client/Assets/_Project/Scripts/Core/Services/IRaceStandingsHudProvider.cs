using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Live race place + minimap markers for Features.RaceHud. Implemented by
    /// <c>Features.RaceFinish.Standings.RaceStandingsTracker</c>.
    /// </summary>
    public interface IRaceStandingsHudProvider
    {
        int LocalPlace { get; }
        float LocalProgress01 { get; }
        float TrackLengthMeters { get; }
        bool LocalHasFinished { get; }
        int? LocalFinalPlace { get; }
        IReadOnlyList<RaceProgressMarker> Markers { get; }
        RaceEndPhase CeremonyPhase { get; }
    }

    public static class RaceStandingsHudService
    {
        public static IRaceStandingsHudProvider Current { get; set; }
    }
}

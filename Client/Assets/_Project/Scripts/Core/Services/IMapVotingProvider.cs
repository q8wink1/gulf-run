using System;
using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Map Voting seam: three catalog maps, live vote counts, countdown,
    /// winner resolution. Implemented by Features.Multiplayer.MapVotingSession.
    /// </summary>
    public interface IMapVotingProvider
    {
        bool IsVotingActive { get; }
        float SecondsRemaining { get; }
        IReadOnlyList<MapId> CandidateMaps { get; }
        MapId LocalVote { get; }
        MapId WinningMap { get; }
        bool HasResolvedWinner { get; }

        event Action VotingStateChanged;

        int GetVoteCount(MapId mapId);
        void CastLocalVote(MapId mapId);
        void CastRemoteVote(int connectionId, MapId mapId);
        void BeginVoting(IReadOnlyList<MapId> candidates, float durationSeconds);
        void Clear();
    }

    /// <summary>Static locator for <see cref="IMapVotingProvider"/>.</summary>
    public static class MapVotingService
    {
        public static IMapVotingProvider Current { get; set; }
    }
}

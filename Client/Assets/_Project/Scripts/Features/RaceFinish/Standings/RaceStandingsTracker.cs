using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.RaceFinish.Configuration;
using UnityEngine;

namespace GulfRun.Features.RaceFinish.Standings
{
    /// <summary>
    /// Single client-side source of truth for "what do we currently know
    /// about this race's results" — every UI/debug consumer (Podium
    /// Ceremony, Reward Screen, Race Finish Debug View, Sprint 15 Race HUD)
    /// reads from here instead of each independently subscribing to the same
    /// <see cref="IMatchTransport"/> events.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RaceStandingsTracker : SceneSingleton<RaceStandingsTracker>, IRaceStandingsHudProvider
    {
        [SerializeField] private RaceFinishConfig config;

        private readonly Dictionary<int, PlayerRaceResult> _liveResults = new Dictionary<int, PlayerRaceResult>();
        private readonly Dictionary<int, EliminationStatusEvent> _eliminationStatus = new Dictionary<int, EliminationStatusEvent>();
        private readonly Dictionary<int, RaceRewardBreakdown> _rewards = new Dictionary<int, RaceRewardBreakdown>();
        private readonly Dictionary<int, float> _distances = new Dictionary<int, float>();
        private readonly HashSet<int> _finishedIds = new HashSet<int>();
        private readonly List<RaceProgressMarker> _markers = new List<RaceProgressMarker>(8);

        /// <summary>Every player resolved so far this race, live (FinishPosition is -1 until <see cref="FinalResults"/> is set).</summary>
        public IReadOnlyDictionary<int, PlayerRaceResult> LiveResults => _liveResults;

        /// <summary>Null until the race has fully ended; then the complete, final 1..N ranking.</summary>
        public IReadOnlyList<PlayerRaceResult> FinalResults { get; private set; }

        /// <summary>The host-broadcast, synchronized ceremony phase — identical for every client. Debug/analytics should read this; ceremony views should read <see cref="LocalDisplayPhase"/> instead (see its doc comment).</summary>
        public RaceEndPhase CurrentPhase { get; private set; } = RaceEndPhase.None;

        private int _localSkipRank;

        public RaceEndPhase LocalDisplayPhase => CeremonySkipProgression.PhaseOfRank(_localSkipRank);

        public void RequestLocalSkip() => _localSkipRank = CeremonySkipProgression.AdvanceRank(_localSkipRank);

        int IRaceStandingsHudProvider.LocalPlace
        {
            get
            {
                IMatchTransport transport = MatchTransportService.Current;
                int localId = transport != null ? transport.LocalConnectionId : 0;
                SyncLocalDistance(localId);
                return RaceLiveRanking.ComputeLocalPlace(localId, _distances, _finishedIds);
            }
        }

        float IRaceStandingsHudProvider.LocalProgress01
        {
            get
            {
                float track = TrackLength;
                if (track <= 0f)
                {
                    return 0f;
                }

                IMatchTransport transport = MatchTransportService.Current;
                int localId = transport != null ? transport.LocalConnectionId : 0;
                SyncLocalDistance(localId);
                return _distances.TryGetValue(localId, out float d) ? Mathf.Clamp01(d / track) : 0f;
            }
        }

        float IRaceStandingsHudProvider.TrackLengthMeters => TrackLength;

        bool IRaceStandingsHudProvider.LocalHasFinished
        {
            get
            {
                IMatchTransport transport = MatchTransportService.Current;
                return transport != null && _finishedIds.Contains(transport.LocalConnectionId);
            }
        }

        int? IRaceStandingsHudProvider.LocalFinalPlace
        {
            get
            {
                IMatchTransport transport = MatchTransportService.Current;
                if (transport == null || !_liveResults.TryGetValue(transport.LocalConnectionId, out PlayerRaceResult result))
                {
                    return null;
                }

                return result.FinishPosition >= 1 ? result.FinishPosition : (int?)null;
            }
        }

        IReadOnlyList<RaceProgressMarker> IRaceStandingsHudProvider.Markers
        {
            get
            {
                RebuildMarkers();
                return _markers;
            }
        }

        RaceEndPhase IRaceStandingsHudProvider.CeremonyPhase => CurrentPhase;

        private float TrackLength => config != null ? config.TrackLengthMeters : 550f;

        private void OnEnable()
        {
            RaceStandingsHudService.Current = this;
            IMatchTransport transport = MatchTransportService.Current;
            transport.PlayerRaceResultReported += HandlePlayerRaceResultReported;
            transport.RaceResultsFinalized += HandleRaceResultsFinalized;
            transport.EliminationStatusChanged += HandleEliminationStatusChanged;
            transport.RaceRewardCalculated += HandleRaceRewardCalculated;
            transport.RaceEndPhaseChanged += HandleRaceEndPhaseChanged;
            transport.MatchStateChanged += HandleMatchStateChanged;
            transport.RaceProgressReported += HandleRaceProgressReported;
        }

        private void OnDisable()
        {
            if (ReferenceEquals(RaceStandingsHudService.Current, this))
            {
                RaceStandingsHudService.Current = null;
            }

            IMatchTransport transport = MatchTransportService.Current;
            if (transport == null)
            {
                return;
            }

            transport.PlayerRaceResultReported -= HandlePlayerRaceResultReported;
            transport.RaceResultsFinalized -= HandleRaceResultsFinalized;
            transport.EliminationStatusChanged -= HandleEliminationStatusChanged;
            transport.RaceRewardCalculated -= HandleRaceRewardCalculated;
            transport.RaceEndPhaseChanged -= HandleRaceEndPhaseChanged;
            transport.MatchStateChanged -= HandleMatchStateChanged;
            transport.RaceProgressReported -= HandleRaceProgressReported;
        }

        public bool TryGetEliminationStatus(int connectionId, out EliminationStatusEvent status) =>
            _eliminationStatus.TryGetValue(connectionId, out status);

        public bool TryGetReward(int connectionId, out RaceRewardBreakdown reward) =>
            _rewards.TryGetValue(connectionId, out reward);

        private void HandleRaceProgressReported(RaceProgressReport report)
        {
            _distances[report.ConnectionId] = report.DistanceMeters;
        }

        private void HandlePlayerRaceResultReported(PlayerRaceResult result)
        {
            _liveResults[result.ConnectionId] = result;
            _finishedIds.Add(result.ConnectionId);
            _distances[result.ConnectionId] = result.DistanceMetersReached;

            IMatchTransport transport = MatchTransportService.Current;
            if (transport != null && result.ConnectionId == transport.LocalConnectionId)
            {
                CharacterAnimationCueService.RaiseLocalCue(CharacterAnimationResolver.FromFinishReason(result.Reason));
            }
        }

        private void HandleRaceResultsFinalized(PlayerRaceResult[] results)
        {
            FinalResults = results;
            IMatchTransport transport = MatchTransportService.Current;
            for (int i = 0; i < results.Length; i++)
            {
                PlayerRaceResult result = results[i];
                _liveResults[result.ConnectionId] = result;
                _finishedIds.Add(result.ConnectionId);
                _distances[result.ConnectionId] = result.DistanceMetersReached;

                if (transport != null && result.ConnectionId == transport.LocalConnectionId)
                {
                    var outcome = new PlayerMatchOutcome(result.FinishPosition, result.Reason, (float)result.FinishTimeSeconds, result.DistanceMetersReached, result.CoinsCollected);
                    PlayerStatEventService.RaiseLocalMatchCompleted(outcome);
                }
            }
        }

        private void HandleEliminationStatusChanged(EliminationStatusEvent status) => _eliminationStatus[status.ConnectionId] = status;

        private void HandleRaceRewardCalculated(RaceRewardBreakdown reward) => _rewards[reward.ConnectionId] = reward;

        private void HandleRaceEndPhaseChanged(RaceEndPhase phase)
        {
            CurrentPhase = phase;
            _localSkipRank = CeremonySkipProgression.SyncRank(_localSkipRank, CeremonySkipProgression.RankOf(phase));
        }

        private void HandleMatchStateChanged(MatchState state)
        {
            if (state != MatchState.Running)
            {
                return;
            }

            _liveResults.Clear();
            _eliminationStatus.Clear();
            _rewards.Clear();
            _distances.Clear();
            _finishedIds.Clear();
            _markers.Clear();
            FinalResults = null;
            CurrentPhase = RaceEndPhase.None;
            _localSkipRank = 0;
        }

        private void SyncLocalDistance(int localId)
        {
            IRaceProgressProvider progress = RaceProgressService.Current;
            if (progress != null)
            {
                _distances[localId] = (float)progress.DistanceMeters;
            }
        }

        private void RebuildMarkers()
        {
            _markers.Clear();
            float track = TrackLength;
            if (track <= 0f)
            {
                return;
            }

            IMatchTransport transport = MatchTransportService.Current;
            int localId = transport != null ? transport.LocalConnectionId : -1;
            SyncLocalDistance(localId);

            foreach (KeyValuePair<int, float> pair in _distances)
            {
                _markers.Add(new RaceProgressMarker(
                    pair.Key,
                    pair.Value / track,
                    pair.Key == localId,
                    _finishedIds.Contains(pair.Key)));
            }
        }
    }
}

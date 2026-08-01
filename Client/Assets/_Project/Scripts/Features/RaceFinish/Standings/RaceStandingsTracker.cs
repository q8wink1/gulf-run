using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.RaceFinish.Standings
{
    /// <summary>
    /// Single client-side source of truth for "what do we currently know
    /// about this race's results" — every UI/debug consumer (Podium
    /// Ceremony, Reward Screen, Race Finish Debug View) reads from here
    /// instead of each independently subscribing to the same
    /// <see cref="IMatchTransport"/> events, avoiding duplicated
    /// subscription/bookkeeping logic across three separate views.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RaceStandingsTracker : SceneSingleton<RaceStandingsTracker>
    {
        private readonly Dictionary<int, PlayerRaceResult> _liveResults = new Dictionary<int, PlayerRaceResult>();
        private readonly Dictionary<int, EliminationStatusEvent> _eliminationStatus = new Dictionary<int, EliminationStatusEvent>();
        private readonly Dictionary<int, RaceRewardBreakdown> _rewards = new Dictionary<int, RaceRewardBreakdown>();

        /// <summary>Every player resolved so far this race, live (FinishPosition is -1 until <see cref="FinalResults"/> is set).</summary>
        public IReadOnlyDictionary<int, PlayerRaceResult> LiveResults => _liveResults;

        /// <summary>Null until the race has fully ended; then the complete, final 1..N ranking.</summary>
        public IReadOnlyList<PlayerRaceResult> FinalResults { get; private set; }

        public RaceEndPhase CurrentPhase { get; private set; } = RaceEndPhase.None;

        private void OnEnable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            transport.PlayerRaceResultReported += HandlePlayerRaceResultReported;
            transport.RaceResultsFinalized += HandleRaceResultsFinalized;
            transport.EliminationStatusChanged += HandleEliminationStatusChanged;
            transport.RaceRewardCalculated += HandleRaceRewardCalculated;
            transport.RaceEndPhaseChanged += HandleRaceEndPhaseChanged;
            transport.MatchStateChanged += HandleMatchStateChanged;
        }

        private void OnDisable()
        {
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
        }

        public bool TryGetEliminationStatus(int connectionId, out EliminationStatusEvent status) =>
            _eliminationStatus.TryGetValue(connectionId, out status);

        public bool TryGetReward(int connectionId, out RaceRewardBreakdown reward) =>
            _rewards.TryGetValue(connectionId, out reward);

        private void HandlePlayerRaceResultReported(PlayerRaceResult result) => _liveResults[result.ConnectionId] = result;

        private void HandleRaceResultsFinalized(PlayerRaceResult[] results)
        {
            FinalResults = results;
            for (int i = 0; i < results.Length; i++)
            {
                _liveResults[results[i].ConnectionId] = results[i];
            }
        }

        private void HandleEliminationStatusChanged(EliminationStatusEvent status) => _eliminationStatus[status.ConnectionId] = status;

        private void HandleRaceRewardCalculated(RaceRewardBreakdown reward) => _rewards[reward.ConnectionId] = reward;

        private void HandleRaceEndPhaseChanged(RaceEndPhase phase) => CurrentPhase = phase;

        private void HandleMatchStateChanged(MatchState state)
        {
            if (state != MatchState.Running)
            {
                return;
            }

            _liveResults.Clear();
            _eliminationStatus.Clear();
            _rewards.Clear();
            FinalResults = null;
            CurrentPhase = RaceEndPhase.None;
        }
    }
}

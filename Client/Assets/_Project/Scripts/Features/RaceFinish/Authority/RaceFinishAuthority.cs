using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Domain;
using GulfRun.Features.RaceFinish.Configuration;
using UnityEngine;

namespace GulfRun.Features.RaceFinish.Authority
{
    /// <summary>
    /// Host-authoritative decision-maker for the entire Race Finish flow:
    /// detects finish-line crossings and elimination-gap warnings/countdowns
    /// from each participant's periodic <see cref="RaceProgressReport"/>,
    /// resolves every player exactly once (finished or eliminated), computes
    /// the final ranking and reward once the whole race is over, and then
    /// drives the host-owned Podium/Reward ceremony clock all the way back to
    /// <see cref="MatchState.Waiting"/> — the same "clients report/request,
    /// host confirms and broadcasts" role <c>WeaponAuthority</c>/
    /// <c>TrapAuthority</c> already play for their systems, and the same
    /// "every gameplay-facing system reacts only to what this class
    /// broadcasts" guarantee that gives P011 RES-001/002 ("results cannot be
    /// modified by players; the official result is server-generated") a real
    /// implementation today, ahead of a future dedicated server.
    ///
    /// Persistent (match-spanning) — placed alongside the Connection/Lobby/
    /// Match/Session/Weapon/Trap authorities in Boot.unity.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RaceFinishAuthority : Singleton<RaceFinishAuthority>
    {
        [SerializeField] private RaceFinishConfig config;

        private IMatchTransport _transport;
        private MatchState _currentMatchState = MatchState.Waiting;
        private RaceEndPhase _currentPhase = RaceEndPhase.None;

        private double _raceStartTimeSeconds;
        private double _phaseElapsedSeconds;
        private bool _skipRequestedThisPhase;
        private int _resolutionOrderCounter;

        private readonly HashSet<int> _active = new HashSet<int>();
        private readonly Dictionary<int, PlayerRaceResult> _resolved = new Dictionary<int, PlayerRaceResult>();
        private readonly Dictionary<int, RaceProgressReport> _lastKnownProgress = new Dictionary<int, RaceProgressReport>();
        private readonly Dictionary<int, double> _eliminationWarningStartSeconds = new Dictionary<int, double>();
        private readonly Dictionary<int, int> _lastBroadcastWarningSeconds = new Dictionary<int, int>();
        private readonly HashSet<int> _skippedThisPhase = new HashSet<int>();
        private readonly List<int> _scratchIds = new List<int>();

        protected override void OnInitialize()
        {
        }

        private void OnEnable()
        {
            _transport = MatchTransportService.Current;
            _transport.MatchStateChanged += HandleMatchStateChanged;
            _transport.RaceProgressReported += HandleRaceProgressReported;
            _transport.SkipRaceEndPhaseRequested += HandleSkipRequested;
        }

        private void OnDisable()
        {
            if (_transport == null)
            {
                return;
            }

            _transport.MatchStateChanged -= HandleMatchStateChanged;
            _transport.RaceProgressReported -= HandleRaceProgressReported;
            _transport.SkipRaceEndPhaseRequested -= HandleSkipRequested;
        }

        private void Update()
        {
            if (_transport == null || !_transport.IsHost || config == null)
            {
                return;
            }

            if (_currentMatchState == MatchState.Running)
            {
                TickRace();
            }
            else if (_currentMatchState == MatchState.Finished && _currentPhase != RaceEndPhase.None)
            {
                TickCeremony();
            }
        }

        private void HandleMatchStateChanged(MatchState newState)
        {
            _currentMatchState = newState;

            if (newState == MatchState.Running)
            {
                BeginNewRace();
            }
        }

        private void BeginNewRace()
        {
            _raceStartTimeSeconds = Time.timeAsDouble;
            _resolutionOrderCounter = 0;
            _currentPhase = RaceEndPhase.None;
            _phaseElapsedSeconds = 0d;
            _skipRequestedThisPhase = false;

            _active.Clear();
            _resolved.Clear();
            _lastKnownProgress.Clear();
            _eliminationWarningStartSeconds.Clear();
            _lastBroadcastWarningSeconds.Clear();
            _skippedThisPhase.Clear();

            if (_transport == null)
            {
                return;
            }

            foreach (MatchParticipant participant in _transport.Participants)
            {
                _active.Add(participant.Identity.ConnectionId);
            }
        }

        private void HandleRaceProgressReported(RaceProgressReport report)
        {
            _lastKnownProgress[report.ConnectionId] = report;

            if (_transport == null || !_transport.IsHost || _currentMatchState != MatchState.Running)
            {
                return;
            }

            if (_active.Contains(report.ConnectionId) && report.DistanceMeters >= config.TrackLengthMeters)
            {
                ResolvePlayer(report.ConnectionId, FinishReason.Completed, report, Time.timeAsDouble);
            }
        }

        private void TickRace()
        {
            double now = Time.timeAsDouble;

            if (now - _raceStartTimeSeconds >= config.MaxRaceDurationSeconds)
            {
                EliminateAllRemaining(now);
                return;
            }

            float leaderDistance = ComputeLeaderDistance();

            _scratchIds.Clear();
            _scratchIds.AddRange(_active);

            for (int i = 0; i < _scratchIds.Count; i++)
            {
                int connectionId = _scratchIds[i];
                if (_lastKnownProgress.TryGetValue(connectionId, out RaceProgressReport progress))
                {
                    TickElimination(connectionId, progress, leaderDistance, now);
                }
            }
        }

        private float ComputeLeaderDistance()
        {
            float leader = 0f;
            foreach (RaceProgressReport progress in _lastKnownProgress.Values)
            {
                if (progress.DistanceMeters > leader)
                {
                    leader = progress.DistanceMeters;
                }
            }

            return leader;
        }

        private void TickElimination(int connectionId, RaceProgressReport progress, float leaderDistance, double now)
        {
            bool warning = _eliminationWarningStartSeconds.TryGetValue(connectionId, out double warningStart);
            bool shouldWarn = RaceElimination.ShouldWarn(leaderDistance, progress.DistanceMeters, config.EliminationWarningGapMeters);
            bool shouldClear = RaceElimination.ShouldClearWarning(leaderDistance, progress.DistanceMeters, config.EliminationRecoveryGapMeters);

            if (!warning)
            {
                if (shouldWarn)
                {
                    _eliminationWarningStartSeconds[connectionId] = now;
                    BroadcastElimination(connectionId, EliminationStatus.Warning, Mathf.CeilToInt(config.EliminationCountdownSeconds), now);
                }

                return;
            }

            if (shouldClear)
            {
                _eliminationWarningStartSeconds.Remove(connectionId);
                _lastBroadcastWarningSeconds.Remove(connectionId);
                BroadcastElimination(connectionId, EliminationStatus.Safe, 0, now);
                return;
            }

            double elapsed = now - warningStart;
            int wholeSeconds = CountdownMath.WholeSecondsRemaining(elapsed, config.EliminationCountdownSeconds);

            if (elapsed >= config.EliminationCountdownSeconds)
            {
                _eliminationWarningStartSeconds.Remove(connectionId);
                _lastBroadcastWarningSeconds.Remove(connectionId);
                ResolvePlayer(connectionId, FinishReason.Eliminated, progress, now);
                return;
            }

            if (!_lastBroadcastWarningSeconds.TryGetValue(connectionId, out int lastSeconds) || lastSeconds != wholeSeconds)
            {
                _lastBroadcastWarningSeconds[connectionId] = wholeSeconds;
                BroadcastElimination(connectionId, EliminationStatus.Warning, wholeSeconds, now);
            }
        }

        private void BroadcastElimination(int connectionId, EliminationStatus status, int secondsRemaining, double now) =>
            _transport.BroadcastEliminationStatus(new EliminationStatusEvent(connectionId, status, secondsRemaining, now));

        private void EliminateAllRemaining(double now)
        {
            _scratchIds.Clear();
            _scratchIds.AddRange(_active);

            for (int i = 0; i < _scratchIds.Count; i++)
            {
                int connectionId = _scratchIds[i];
                RaceProgressReport progress = _lastKnownProgress.TryGetValue(connectionId, out RaceProgressReport known)
                    ? known
                    : new RaceProgressReport(connectionId, 0f, 0, now);

                ResolvePlayer(connectionId, FinishReason.Eliminated, progress, now);
            }
        }

        private void ResolvePlayer(int connectionId, FinishReason reason, RaceProgressReport progress, double now)
        {
            if (!_active.Remove(connectionId))
            {
                return;
            }

            _eliminationWarningStartSeconds.Remove(connectionId);
            _lastBroadcastWarningSeconds.Remove(connectionId);
            _resolutionOrderCounter++;

            var result = new PlayerRaceResult(
                connectionId,
                reason,
                now - _raceStartTimeSeconds,
                progress.CoinsCollected,
                progress.DistanceMeters,
                _resolutionOrderCounter,
                finishPosition: -1);

            _resolved[connectionId] = result;
            _transport.BroadcastPlayerRaceResult(result);

            if (_active.Count == 0)
            {
                FinalizeRace();
            }
        }

        private void FinalizeRace()
        {
            List<PlayerRaceResult> ranked = RaceRanking.ComputeFinalPositions(_resolved.Values);
            _transport.BroadcastRaceResultsFinalized(ranked.ToArray());

            for (int i = 0; i < ranked.Count; i++)
            {
                PlayerRaceResult result = ranked[i];
                RaceRewardBreakdown reward = RaceRewardCalculator.Calculate(
                    result.ConnectionId,
                    result.FinishPosition,
                    result.CoinsCollected,
                    config.CoinRewardMultiplier,
                    config.BonusCoinsByPosition,
                    config.RankPointsByPosition,
                    config.ExperienceByPosition,
                    config.ParticipationExperience);

                _transport.BroadcastRaceReward(reward);
            }

            _transport.BroadcastMatchState(MatchState.Finished);
            BeginPhase(RaceEndPhase.Podium);
        }

        private void TickCeremony()
        {
            _phaseElapsedSeconds += Time.deltaTime;
            float duration = _currentPhase == RaceEndPhase.Podium ? config.PodiumCeremonySeconds : config.RewardScreenSeconds;

            if (_skipRequestedThisPhase || _phaseElapsedSeconds >= duration)
            {
                AdvancePhase();
            }
        }

        private void AdvancePhase()
        {
            if (_currentPhase == RaceEndPhase.Podium)
            {
                BeginPhase(RaceEndPhase.Reward);
                return;
            }

            if (_currentPhase == RaceEndPhase.Reward)
            {
                _currentPhase = RaceEndPhase.None;
                _transport.BroadcastRaceEndPhase(RaceEndPhase.None);

                // Lobby Return: broadcasting Waiting is the entire mechanism —
                // LobbyManager's roster is untouched by a MatchState change
                // (it only clears on an explicit leave/disconnect), so every
                // connected player is already "back in the same lobby".
                _transport.BroadcastMatchState(MatchState.Waiting);
            }
        }

        private void BeginPhase(RaceEndPhase phase)
        {
            _currentPhase = phase;
            _phaseElapsedSeconds = 0d;
            _skipRequestedThisPhase = false;
            _skippedThisPhase.Clear();
            _transport.BroadcastRaceEndPhase(phase);
        }

        /// <summary>
        /// Sprint 7 addendum: "players may skip the ceremony individually;
        /// skipping does not interrupt other players." An individual skip
        /// only ever hides the ceremony for that one client locally (see
        /// <c>Standings.RaceStandingsTracker.LocalDisplayPhase</c>) — the
        /// host's shared clock (which every *other* client's presentation
        /// still follows) only ever advances early once EVERY currently
        /// connected participant has independently chosen to skip, which by
        /// definition interrupts nobody.
        /// </summary>
        private void HandleSkipRequested(int connectionId)
        {
            if (_currentMatchState != MatchState.Finished || _currentPhase == RaceEndPhase.None)
            {
                return;
            }

            _skippedThisPhase.Add(connectionId);

            foreach (MatchParticipant participant in _transport.Participants)
            {
                if (!_skippedThisPhase.Contains(participant.Identity.ConnectionId))
                {
                    return;
                }
            }

            _skipRequestedThisPhase = true;
        }
    }
}

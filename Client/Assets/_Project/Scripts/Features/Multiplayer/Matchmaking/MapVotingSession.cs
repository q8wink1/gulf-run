using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Services;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Multiplayer.Matchmaking
{
    /// <summary>
    /// Hosts the Map Voting session state (candidates, votes, countdown, winner).
    /// UI lives in Features.Matchmaking and never references this type directly.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MapVotingSession : Singleton<MapVotingSession>, IMapVotingProvider
    {
        private readonly List<MapId> _candidates = new List<MapId>(3);
        private readonly Dictionary<string, int> _votesByMap = new Dictionary<string, int>();
        private readonly Dictionary<int, string> _voteByConnection = new Dictionary<int, string>();
        private readonly System.Random _random = new System.Random();

        private float _secondsRemaining;
        private bool _active;
        private bool _resolved;
        private MapId _localVote = MapId.None;
        private MapId _winningMap = MapId.None;

        public bool IsVotingActive => _active;
        public float SecondsRemaining => Mathf.Max(0f, _secondsRemaining);
        public IReadOnlyList<MapId> CandidateMaps => _candidates;
        public MapId LocalVote => _localVote;
        public MapId WinningMap => _winningMap;
        public bool HasResolvedWinner => _resolved;

        public event Action VotingStateChanged;

        protected override void OnInitialize()
        {
            MapVotingService.Current = this;
        }

        private void OnDisable()
        {
            if (ReferenceEquals(MapVotingService.Current, this))
            {
                MapVotingService.Current = null;
            }
        }

        private void Update()
        {
            if (!_active || _resolved)
            {
                return;
            }

            _secondsRemaining -= Time.deltaTime;
            if (_secondsRemaining > 0f)
            {
                VotingStateChanged?.Invoke();
                return;
            }

            ResolveWinner();
        }

        public void BeginVoting(IReadOnlyList<MapId> candidates, float durationSeconds)
        {
            ClearInternal();
            if (candidates == null || candidates.Count == 0)
            {
                return;
            }

            for (int i = 0; i < candidates.Count; i++)
            {
                MapId map = candidates[i];
                if (map.Equals(MapId.None))
                {
                    continue;
                }

                _candidates.Add(map);
                _votesByMap[map.Value] = 0;
            }

            if (_candidates.Count == 0)
            {
                return;
            }

            _active = true;
            _resolved = false;
            _secondsRemaining = Mathf.Max(1f, durationSeconds);
            VotingStateChanged?.Invoke();
        }

        public int GetVoteCount(MapId mapId)
        {
            return _votesByMap.TryGetValue(mapId.Value, out int count) ? count : 0;
        }

        public void CastLocalVote(MapId mapId) => CastVote(connectionId: -1, mapId, isLocal: true);

        /// <summary>Mock remote vote from a simulated lobby participant.</summary>
        public void CastRemoteVote(int connectionId, MapId mapId) => CastVote(connectionId, mapId, isLocal: false);

        public void Clear()
        {
            ClearInternal();
            VotingStateChanged?.Invoke();
        }

        private void CastVote(int connectionId, MapId mapId, bool isLocal)
        {
            if (!_active || _resolved || mapId.Equals(MapId.None) || !_votesByMap.ContainsKey(mapId.Value))
            {
                return;
            }

            int lookupId = isLocal ? -1 : connectionId;
            if (_voteByConnection.TryGetValue(lookupId, out string prior) && _votesByMap.ContainsKey(prior))
            {
                _votesByMap[prior] = Mathf.Max(0, _votesByMap[prior] - 1);
            }

            _voteByConnection[lookupId] = mapId.Value;
            _votesByMap[mapId.Value] = _votesByMap[mapId.Value] + 1;

            if (isLocal)
            {
                _localVote = mapId;
            }

            VotingStateChanged?.Invoke();
        }

        private void ResolveWinner()
        {
            _secondsRemaining = 0f;
            _resolved = true;
            _active = false;

            int best = -1;
            var tied = new List<MapId>();
            for (int i = 0; i < _candidates.Count; i++)
            {
                MapId map = _candidates[i];
                int votes = GetVoteCount(map);
                if (votes > best)
                {
                    best = votes;
                    tied.Clear();
                    tied.Add(map);
                }
                else if (votes == best)
                {
                    tied.Add(map);
                }
            }

            if (tied.Count == 0)
            {
                _winningMap = MapId.None;
            }
            else if (tied.Count == 1)
            {
                _winningMap = tied[0];
            }
            else
            {
                _winningMap = tied[_random.Next(0, tied.Count)];
            }

            VotingStateChanged?.Invoke();
        }

        private void ClearInternal()
        {
            _candidates.Clear();
            _votesByMap.Clear();
            _voteByConnection.Clear();
            _localVote = MapId.None;
            _winningMap = MapId.None;
            _secondsRemaining = 0f;
            _active = false;
            _resolved = false;
        }
    }
}

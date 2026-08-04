using System.Collections.Generic;
using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Matchmaking.UI;
using UnityEngine;

namespace GulfRun.Features.Matchmaking.MapVoting
{
    /// <summary>
    /// Map Voting screen: three random Gulf maps, live vote counts, countdown,
    /// tie → random among tied. On winner: apply map, then Loading → Gameplay.
    /// </summary>
    public sealed class MapVotingView : MonoBehaviour
    {
        [SerializeField, Range(0.5f, 4f)] private float postWinnerHoldSeconds = 1.4f;

        private readonly List<int> _remoteVotersScratch = new List<int>(4);
        private readonly HashSet<int> _remotesWhoVoted = new HashSet<int>();
        private float _remoteVoteTimer = 0.8f;
        private bool _finishing;
        private float _finishAt = -1f;
        private bool _loadedLoading;

        private void Start()
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            IMapVotingProvider voting = MapVotingService.Current;
            if (lobby == null || !lobby.IsInMatch)
            {
                SceneManager.Instance?.LoadLobby();
                return;
            }

            if (voting != null && !voting.IsVotingActive && !voting.HasResolvedWinner &&
                MapContextService.Current != null)
            {
                voting.BeginVoting(MapContextService.Current.PickRandomMaps(3), 12f);
            }
        }

        private void Update()
        {
            IMapVotingProvider voting = MapVotingService.Current;
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (voting == null)
            {
                return;
            }

            if (!_finishing && voting.IsVotingActive)
            {
                SimulateRemoteVotes(voting, lobby);
            }

            if (!_finishing && voting.HasResolvedWinner)
            {
                _finishing = true;
                _finishAt = Time.unscaledTime + postWinnerHoldSeconds;
                MapContextService.Current?.ApplyForcedMap(voting.WinningMap);
            }

            if (_finishing && !_loadedLoading && Time.unscaledTime >= _finishAt)
            {
                _loadedLoading = true;
                if (SceneManager.Instance != null)
                {
                    SceneManager.Instance.LoadLoading();
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.LoadingSceneName);
                }
            }
        }

        private void OnGUI()
        {
            IMapVotingProvider voting = MapVotingService.Current;
            if (voting == null)
            {
                return;
            }

            PreRaceLobbyTheme.DrawPanel(new Rect(40f, 36f, Screen.width - 80f, 72f));
            GUI.Label(new Rect(56f, 48f, Screen.width - 120f, 28f), "Map Voting", PreRaceLobbyTheme.Title);
            string timer = voting.HasResolvedWinner
                ? "Winner: " + ResolveName(voting.WinningMap)
                : "Time left: " + Mathf.CeilToInt(voting.SecondsRemaining) + "s — vote for a map";
            GUI.Label(new Rect(56f, 76f, Screen.width - 120f, 22f), timer, PreRaceLobbyTheme.Muted);

            IReadOnlyList<MapId> maps = voting.CandidateMaps;
            if (maps == null || maps.Count == 0)
            {
                return;
            }

            float cardW = Mathf.Min(320f, (Screen.width - 96f) / maps.Count);
            float total = maps.Count * cardW + (maps.Count - 1) * 16f;
            float startX = (Screen.width - total) * 0.5f;
            float y = Screen.height * 0.28f;

            for (int i = 0; i < maps.Count; i++)
            {
                MapId map = maps[i];
                Rect card = new Rect(startX + i * (cardW + 16f), y, cardW, 260f);
                PreRaceLobbyTheme.DrawPanel(card);

                GUI.Label(new Rect(card.x + 12f, card.y + 18f, card.width - 24f, 36f), ResolveName(map), PreRaceLobbyTheme.Header);
                GUI.Label(new Rect(card.x + 12f, card.y + 64f, card.width - 24f, 28f),
                    "Votes: " + voting.GetVoteCount(map), PreRaceLobbyTheme.Label);

                bool selected = voting.LocalVote == map;
                Color previous = GUI.color;
                GUI.color = selected ? PreRaceLobbyTheme.Gold : PreRaceLobbyTheme.SandDark;
                Rect voteBtn = new Rect(card.x + 24f, card.y + card.height - 64f, card.width - 48f, 44f);
                if (!voting.HasResolvedWinner && GUI.Button(voteBtn, selected ? "Voted" : "Vote", PreRaceLobbyTheme.GoldButton))
                {
                    voting.CastLocalVote(map);
                }

                GUI.color = previous;
            }
        }

        private void SimulateRemoteVotes(IMapVotingProvider voting, IMatchLobbySummaryProvider lobby)
        {
            if (lobby == null || voting.CandidateMaps == null || voting.CandidateMaps.Count == 0)
            {
                return;
            }

            _remoteVoteTimer -= Time.deltaTime;
            if (_remoteVoteTimer > 0f)
            {
                return;
            }

            _remoteVoteTimer = 0.85f + Random.value * 0.75f;
            _remoteVotersScratch.Clear();
            foreach (MatchParticipant p in lobby.Participants)
            {
                int id = p.Identity.ConnectionId;
                if (id == lobby.LocalConnectionId || _remotesWhoVoted.Contains(id))
                {
                    continue;
                }

                _remoteVotersScratch.Add(id);
            }

            if (_remoteVotersScratch.Count == 0)
            {
                return;
            }

            int voter = _remoteVotersScratch[Random.Range(0, _remoteVotersScratch.Count)];
            MapId pick = voting.CandidateMaps[Random.Range(0, voting.CandidateMaps.Count)];
            voting.CastRemoteVote(voter, pick);
            _remotesWhoVoted.Add(voter);
        }

        private static string ResolveName(MapId map)
        {
            return MapContextService.Current != null
                ? MapContextService.Current.ResolveMapDisplayName(map)
                : map.Value;
        }
    }
}

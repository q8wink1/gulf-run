using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Matchmaking.Lobby
{
    /// <summary>Sprint 14 Matchmaking join/leave SFX driven by roster deltas.</summary>
    public sealed class LobbyAudioDirector : MonoBehaviour
    {
        [SerializeField] private AudioClip playerJoinedSound;
        [SerializeField] private AudioClip playerLeftSound;

        private int _lastCount = -1;

        private void OnEnable()
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby != null)
            {
                lobby.LobbyStateChanged += HandleLobbyChanged;
                _lastCount = lobby.LobbyPlayerCount;
            }
        }

        private void OnDisable()
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby != null)
            {
                lobby.LobbyStateChanged -= HandleLobbyChanged;
            }
        }

        private void HandleLobbyChanged()
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby == null)
            {
                return;
            }

            int count = lobby.LobbyPlayerCount;
            if (_lastCount >= 0)
            {
                if (count > _lastCount)
                {
                    AudioManager.Instance?.PlayOneShot(playerJoinedSound);
                }
                else if (count < _lastCount)
                {
                    AudioManager.Instance?.PlayOneShot(playerLeftSound);
                }
            }

            _lastCount = count;
        }
    }
}

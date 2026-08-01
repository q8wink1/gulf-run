using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using UnityEngine;

namespace GulfRun.Features.Matchmaking.Lobby
{
    /// <summary>
    /// Sprint 14 Matchmaking composition root for <c>Lobby.unity</c>: soft
    /// Gulf lobby music fade-in, and a safety return to Main Menu if the
    /// player somehow lands here without an active match.
    /// </summary>
    public sealed class LobbyBootstrapper : MonoBehaviour
    {
        [SerializeField] private AudioClip lobbyMusic;
        [SerializeField, Range(0f, 1f)] private float lobbyMusicVolume = 0.55f;
        [SerializeField, Range(0f, 3f)] private float fadeInSeconds = 0.6f;

        private void Start()
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby == null || !lobby.IsInMatch)
            {
                SceneManager.Instance?.LoadMainMenu();
                return;
            }

            if (AudioManager.Instance != null && lobbyMusic != null)
            {
                AudioManager.Instance.PlayMusic(lobbyMusic, 0f);
                AudioManager.Instance.FadeMusicTo(lobbyMusicVolume, fadeInSeconds);
            }
        }
    }
}

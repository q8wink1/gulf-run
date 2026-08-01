using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using UnityEngine;

namespace GulfRun.Features.MainMenu
{
    /// <summary>
    /// Debug overlay: FPS, Current Lobby, Current Background, Player ID,
    /// Network Status — the Sprint 13 brief's exact "DEBUG: Display" list.
    /// Same OnGUI-placeholder approach as every other Sprint's
    /// *DebugView; next free panel slot after Maps' 3610.
    /// </summary>
    public sealed class MainMenuDebugView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private bool showOnScreenDebug = true;
        [SerializeField] private int panelX = 4060;

        private float _fpsSmoothed;

        private void Update()
        {
            float instantFps = Time.unscaledDeltaTime > 0f ? 1f / Time.unscaledDeltaTime : 0f;
            _fpsSmoothed = Mathf.Lerp(_fpsSmoothed, instantFps, 0.1f);
        }

        private void OnGUI()
        {
            if (!showOnScreenDebug)
            {
                return;
            }

            int y = 10;
            const int lineHeight = 18;
            const int width = 420;

            void Line(string text)
            {
                GUI.Label(new Rect(panelX, y, width, lineHeight), text);
                y += lineHeight;
            }

            Line("[MainMenu] Sprint 13");
            Line("FPS: " + Mathf.CeilToInt(_fpsSmoothed));

            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            string lobbyText = lobby == null ? "unavailable" : lobby.IsInMatch ? lobby.LobbyPlayerCount + "/" + lobby.RequiredPlayerCount + " players" : lobby.IsMatchmaking ? "matchmaking..." : "not in a lobby";
            Line("Current Lobby: " + lobbyText);

            IMapContextProvider mapContext = MapContextService.Current;
            string backgroundText = mapContext != null && mapContext.HasResolvedEnvironment
                ? mapContext.ResolveMapDisplayName(mapContext.Current.Map) + " (" + mapContext.Current.TimeOfDay + ")"
                : "not resolved yet";
            Line("Current Background: " + backgroundText);

            ILocalProfileProvider profile = LocalProfileProviderService.Current;
            Line("Player ID: " + (profile != null && profile.HasProfile ? profile.LocalProfile.PlayerId.Value : "—"));

            IMatchTransport transport = MatchTransportService.Current;
            Line("Network Status: " + (transport != null && transport.IsActive ? "Connected" : "Offline"));
        }
#endif
    }
}

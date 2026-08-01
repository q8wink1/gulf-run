using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Matchmaking.UI;
using UnityEngine;

namespace GulfRun.Features.Matchmaking.Lobby
{
    /// <summary>Sprint 14 Matchmaking Ready System toggle + Leave Room.</summary>
    public sealed class ReadyControlsView : MonoBehaviour
    {
        [SerializeField] private AudioClip readySound;

        private LobbyButtonPressAnimator _readyAnim;
        private LobbyButtonPressAnimator _leaveAnim;

        private void OnGUI()
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby == null || !lobby.IsInMatch)
            {
                return;
            }

            float width = 280f;
            float x = (Screen.width - width) * 0.5f;
            float y = Screen.height - 96f;

            bool ready = lobby.LocalReadyState == PlayerReadyState.Ready;
            Rect readyRect = _readyAnim.Apply(new Rect(x, y, 160f, 40f), 3f);
            Color previous = GUI.color;
            GUI.color = ready ? PreRaceLobbyTheme.Success : PreRaceLobbyTheme.Gold;
            if (GUI.Button(readyRect, ready ? "Ready ✓" : "Ready Up", PreRaceLobbyTheme.GoldButton))
            {
                _readyAnim.NotifyPressed();
                PlayerReadyState next = ready ? PlayerReadyState.NotReady : PlayerReadyState.Ready;
                lobby.SetLocalReady(next);
                if (next == PlayerReadyState.Ready)
                {
                    AudioManager.Instance?.PlayOneShot(readySound);
                }
            }

            GUI.color = previous;

            Rect leaveRect = _leaveAnim.Apply(new Rect(x + 172f, y, 108f, 40f), 2f);
            if (GUI.Button(leaveRect, "Leave", PreRaceLobbyTheme.PanelButton))
            {
                _leaveAnim.NotifyPressed();
                lobby.CancelOrLeaveMatch();
                SceneManager.Instance?.LoadMainMenu();
            }
        }
    }
}

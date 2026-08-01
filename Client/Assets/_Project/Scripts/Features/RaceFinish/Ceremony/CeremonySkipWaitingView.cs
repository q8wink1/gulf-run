using GulfRun.Domain;
using GulfRun.Features.RaceFinish.Standings;
using UnityEngine;

namespace GulfRun.Features.RaceFinish.Ceremony
{
    /// <summary>
    /// Tiny placeholder shown only to a player who has individually skipped
    /// all the way through the ceremony
    /// (<see cref="RaceStandingsTracker.LocalDisplayPhase"/> == <see cref="RaceEndPhase.None"/>)
    /// while the host's synchronized clock
    /// (<see cref="RaceStandingsTracker.CurrentPhase"/>) is still
    /// mid-ceremony for everyone else — confirms the skip took effect
    /// without implying anything was interrupted for other players (Sprint
    /// 7 addendum: "skipping the ceremony does not interrupt other
    /// players"). Disappears the instant the host's own Lobby Return fires
    /// for real (both phases become <see cref="RaceEndPhase.None"/>).
    /// </summary>
    public sealed class CeremonySkipWaitingView : MonoBehaviour
    {
        private GUIStyle _style;

        private void OnGUI()
        {
            RaceStandingsTracker standings = RaceStandingsTracker.Instance;
            if (standings == null || standings.LocalDisplayPhase != RaceEndPhase.None || standings.CurrentPhase == RaceEndPhase.None)
            {
                return;
            }

            EnsureStyle();
            GUI.Label(new Rect(0, Screen.height - 80f, Screen.width, 30f), "Ceremony skipped — returning to lobby shortly...", _style);
        }

        private void EnsureStyle()
        {
            if (_style != null)
            {
                return;
            }

            _style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter
            };
            _style.normal.textColor = Color.gray;
        }
    }
}

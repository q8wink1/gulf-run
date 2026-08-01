using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.GameLoop
{
    /// <summary>
    /// Legacy OnGUI countdown placeholder. Sprint 15's
    /// <c>Features.RaceHud.UI.CountdownHudView</c> owns the production
    /// presentation; this component stays for scenes that have not yet
    /// wired RaceHud (disabled on <c>Gameplay.unity</c>'s RunnerHUD).
    /// </summary>
    public sealed class CountdownView : MonoBehaviour
    {
        [SerializeField] private int fontSize = 96;

        private GUIStyle _style;

        private void OnGUI()
        {
            CountdownController countdown = CountdownController.Instance;
            GameLoopController loop = GameLoopController.Instance;
            if (countdown == null || loop == null || loop.State != GameLoopState.Countdown)
            {
                return;
            }

            EnsureStyle();
            Rect area = new Rect(0, Screen.height * 0.5f - fontSize, Screen.width, fontSize * 1.5f);
            GUI.Label(area, countdown.DisplayText, _style);
        }

        private void EnsureStyle()
        {
            if (_style != null)
            {
                return;
            }

            _style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = fontSize,
                fontStyle = FontStyle.Bold
            };
            _style.normal.textColor = Color.white;
        }
    }
}

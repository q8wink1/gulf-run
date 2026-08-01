using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.GameLoop
{
    /// <summary>
    /// Minimal OnGUI presentation of the race-start countdown ("3", "2",
    /// "1", "GO!"), large and centered on screen. This is a functional
    /// placeholder — it works in real builds, not just the Editor — until a
    /// Canvas + TextMeshPro HUD is authored once the Unity Editor is
    /// available (see Sprint 3 report open items). Contains no gameplay
    /// logic: it only reads <see cref="CountdownController"/>'s public
    /// display state, keeping UI cleanly separated from simulation per the
    /// project's code-quality requirements.
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

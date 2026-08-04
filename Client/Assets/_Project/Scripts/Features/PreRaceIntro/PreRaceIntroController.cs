using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.PreRaceIntro
{
    /// <summary>
    /// Pre-Race Intro UI (Sprint 23.1 / 23.2). Presentation placeholders only —
    /// no player movement, controls, obstacles, or networking.
    /// Sprint 23.2 auto-starts <see cref="RaceCountdownController"/> after a brief hold.
    /// Optional Continue stub skips the intro hold for Editor flow testing.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PreRaceIntroController : MonoBehaviour
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private RaceCountdownController countdown;

        private void Awake()
        {
            if (continueButton == null)
            {
                return;
            }

            continueButton.onClick.AddListener(OnContinueClicked);
        }

        private void OnDestroy()
        {
            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(OnContinueClicked);
            }
        }

        private void OnContinueClicked()
        {
            if (countdown != null)
            {
                countdown.SkipHoldAndStart();
            }
        }
    }
}

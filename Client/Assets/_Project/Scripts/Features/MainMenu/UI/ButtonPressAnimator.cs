using UnityEngine;

namespace GulfRun.Features.MainMenu.UI
{
    /// <summary>
    /// Per-button "small click animation" state (brief: PLAY BUTTON "Small
    /// click animation", ANIMATIONS "Button feedback") — a tiny reusable
    /// value type instead of copy-pasted press-timer fields on every
    /// button-owning view. Call <see cref="NotifyPressed"/> the frame a
    /// button is clicked, then <see cref="EvaluateScale01"/> every OnGUI to
    /// get a 0..1 "how compressed right now" value to shrink the button's
    /// draw rect by.
    /// </summary>
    public struct ButtonPressAnimator
    {
        private const float PressDurationSeconds = 0.12f;

        private double _pressedAtSeconds;
        private bool _hasBeenPressed;

        public void NotifyPressed() => (_pressedAtSeconds, _hasBeenPressed) = (Time.timeAsDouble, true);

        /// <summary>0 = resting size, 1 = fully compressed (the instant of the click).</summary>
        public float EvaluateScale01()
        {
            if (!_hasBeenPressed)
            {
                return 0f;
            }

            float elapsed = (float)(Time.timeAsDouble - _pressedAtSeconds);
            if (elapsed >= PressDurationSeconds)
            {
                return 0f;
            }

            float t = elapsed / PressDurationSeconds;
            return 1f - t;
        }

        /// <summary>Shrinks <paramref name="rect"/> toward its own center by up to <paramref name="maxInset"/> pixels, driven by the current press animation.</summary>
        public Rect Apply(Rect rect, float maxInset)
        {
            float inset = EvaluateScale01() * maxInset;
            return new Rect(rect.x + inset, rect.y + inset, rect.width - inset * 2f, rect.height - inset * 2f);
        }
    }
}

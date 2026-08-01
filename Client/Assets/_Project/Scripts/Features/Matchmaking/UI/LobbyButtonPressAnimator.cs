using UnityEngine;

namespace GulfRun.Features.Matchmaking.UI
{
    /// <summary>Tiny click-compress animator for Pre-Race Lobby buttons (same shape as Main Menu's ButtonPressAnimator, duplicated because Features cannot reference Features).</summary>
    public struct LobbyButtonPressAnimator
    {
        private const float PressDurationSeconds = 0.12f;
        private double _pressedAtSeconds;
        private bool _hasBeenPressed;

        public void NotifyPressed() => (_pressedAtSeconds, _hasBeenPressed) = (Time.timeAsDouble, true);

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

            return 1f - elapsed / PressDurationSeconds;
        }

        public Rect Apply(Rect rect, float maxInset)
        {
            float inset = EvaluateScale01() * maxInset;
            return new Rect(rect.x + inset, rect.y + inset, rect.width - inset * 2f, rect.height - inset * 2f);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.WinningMapRevealScreen
{
    /// <summary>
    /// UI-only reveal prep (Sprint 22.4): card scale-up, soft glow, dim overlay,
    /// optional canvas zoom. No winner logic, networking, or loading.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WinningMapRevealAnimation : MonoBehaviour
    {
        private static readonly Vector3 CardStartScale = new Vector3(0.82f, 0.82f, 1f);
        private static readonly Vector3 CardEndScale = Vector3.one;
        private static readonly Vector3 CanvasStartScale = new Vector3(0.97f, 0.97f, 1f);
        private static readonly Vector3 CanvasEndScale = Vector3.one;

        [SerializeField] private RectTransform winningCard;
        [SerializeField] private Image dimOverlay;
        [SerializeField] private Image glowImage;
        [SerializeField] private RectTransform canvasRoot;
        [SerializeField] private float duration = 0.85f;
        [SerializeField] private float dimTargetAlpha = 0.42f;
        [SerializeField] private float glowTargetAlpha = 0.55f;

        private float _elapsed;
        private bool _playing;
        private Color _dimBase;
        private Color _glowBase;

        private void OnEnable()
        {
            _elapsed = 0f;
            _playing = true;

            if (winningCard != null)
            {
                winningCard.localScale = CardStartScale;
            }

            if (canvasRoot != null)
            {
                canvasRoot.localScale = CanvasStartScale;
            }

            if (dimOverlay != null)
            {
                _dimBase = dimOverlay.color;
                Color c = _dimBase;
                c.a = 0f;
                dimOverlay.color = c;
            }

            if (glowImage != null)
            {
                _glowBase = glowImage.color;
                Color c = _glowBase;
                c.a = 0f;
                glowImage.color = c;
            }
        }

        private void Update()
        {
            if (!_playing)
            {
                return;
            }

            float safeDuration = duration > 0.01f ? duration : 0.01f;
            _elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(_elapsed / safeDuration);
            float eased = 1f - ((1f - t) * (1f - t));

            if (winningCard != null)
            {
                winningCard.localScale = Vector3.LerpUnclamped(CardStartScale, CardEndScale, eased);
            }

            if (canvasRoot != null)
            {
                canvasRoot.localScale = Vector3.LerpUnclamped(CanvasStartScale, CanvasEndScale, eased);
            }

            if (dimOverlay != null)
            {
                Color c = _dimBase;
                c.a = Mathf.Lerp(0f, dimTargetAlpha, eased);
                dimOverlay.color = c;
            }

            if (glowImage != null)
            {
                Color c = _glowBase;
                c.a = Mathf.Lerp(0f, glowTargetAlpha, eased);
                glowImage.color = c;
            }

            if (t >= 1f)
            {
                _playing = false;
            }
        }
    }
}

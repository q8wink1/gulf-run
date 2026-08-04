using UnityEngine;

namespace GulfRun.Features.PreRaceIntro
{
    /// <summary>
    /// UI-only camera/background pan placeholder (Sprint 23.1).
    /// Lerps an oversized background RectTransform — no gameplay camera logic.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PreRaceIntroPanAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform panTarget;
        [SerializeField] private Vector2 panFrom = new Vector2(-80f, 0f);
        [SerializeField] private Vector2 panTo = new Vector2(80f, 12f);
        [SerializeField] private float cycleSeconds = 14f;

        private void Update()
        {
            if (panTarget == null || cycleSeconds <= 0.01f)
            {
                return;
            }

            float pingPong = Mathf.PingPong(Time.unscaledTime / cycleSeconds, 1f);
            float eased = pingPong * pingPong * (3f - (2f * pingPong));
            panTarget.anchoredPosition = Vector2.LerpUnclamped(panFrom, panTo, eased);
        }
    }
}

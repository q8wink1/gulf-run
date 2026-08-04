using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GulfRun.Features.MapVotingScreen
{
    /// <summary>
    /// UI-only map card hover / selected polish (Sprint 22.2).
    /// Scale + shadow only — no voting, networking, or matchmaking.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class MapCardVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private static readonly Vector3 IdleScale = Vector3.one;
        private static readonly Vector3 HoverScale = new Vector3(1.03f, 1.03f, 1f);
        private static readonly Vector3 SelectedScale = new Vector3(1.05f, 1.05f, 1f);

        private static readonly Vector2 IdleShadowDistance = new Vector2(0f, -6f);
        private static readonly Vector2 HoverShadowDistance = new Vector2(0f, -12f);
        private static readonly Vector2 SelectedShadowDistance = new Vector2(0f, -14f);

        private static readonly Color IdleShadowColor = new Color(0f, 0f, 0f, 0.42f);
        private static readonly Color HoverShadowColor = new Color(0f, 0f, 0f, 0.58f);
        private static readonly Color SelectedShadowColor = new Color(1f, 0.84f, 0.40f, 0.62f);

        [SerializeField] private Shadow cardShadow;

        private bool _selected;
        private bool _hovered;

        private void Awake()
        {
            if (cardShadow == null)
            {
                cardShadow = GetComponent<Shadow>();
            }

            ApplyVisual();
        }

        public void SetSelected(bool selected)
        {
            _selected = selected;
            ApplyVisual();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
            ApplyVisual();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
            ApplyVisual();
        }

        private void ApplyVisual()
        {
            if (_selected)
            {
                transform.localScale = SelectedScale;
                ApplyShadow(SelectedShadowColor, SelectedShadowDistance);
                return;
            }

            if (_hovered)
            {
                transform.localScale = HoverScale;
                ApplyShadow(HoverShadowColor, HoverShadowDistance);
                return;
            }

            transform.localScale = IdleScale;
            ApplyShadow(IdleShadowColor, IdleShadowDistance);
        }

        private void ApplyShadow(Color color, Vector2 distance)
        {
            if (cardShadow == null)
            {
                return;
            }

            cardShadow.effectColor = color;
            cardShadow.effectDistance = distance;
        }
    }
}

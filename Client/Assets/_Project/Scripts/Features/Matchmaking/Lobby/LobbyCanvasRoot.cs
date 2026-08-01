using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.Matchmaking.Lobby
{
    /// <summary>
    /// Production Lobby UGUI root. Sprint 14 Lobby content is still OnGUI on
    /// <c>LobbyUI</c>; this Canvas owns the required CanvasScaler contract
    /// (Scale With Screen Size, 1920×1080, match 0.5) for layout validation
    /// and any future RectTransform children — without redesigning the Lobby.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    public sealed class LobbyCanvasRoot : MonoBehaviour
    {
        public const float ReferenceWidth = 1920f;
        public const float ReferenceHeight = 1080f;
        public const float Match = 0.5f;

        private void Awake() => EnsureLayout();

        private void OnValidate() => EnsureLayout();

        public void EnsureLayout()
        {
            EnsureScaler();
            EnsureFullScreenRoot();
        }

        public void EnsureScaler()
        {
            CanvasScaler scaler = GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                return;
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = Match;
        }

        private void EnsureFullScreenRoot()
        {
            RectTransform rect = transform as RectTransform;
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            // Overlay Canvas drives scale at runtime; keep identity in the asset.
            if (rect.localScale == Vector3.zero)
            {
                rect.localScale = Vector3.one;
            }
        }
    }
}

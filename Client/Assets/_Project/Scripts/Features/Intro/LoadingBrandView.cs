using GulfRun.Core.Branding;
using UnityEngine;

namespace GulfRun.Features.Intro
{
    /// <summary>
    /// Sprint 14 "BRANDING: Use this official logo everywhere ... Loading
    /// Screen": a small, static (non-animated) placement of the shared
    /// <see cref="GulfRunBrandMark"/> plus a simple progress placeholder
    /// for the <c>Loading.unity</c> scene. The Loading scene itself is not
    /// wired into any real async/Addressables transition flow yet — that
    /// remains the tracked TODO carried since Sprint 13's report — this
    /// component only satisfies the branding requirement so the mark is
    /// consistently present the moment that flow lands.
    /// </summary>
    public sealed class LoadingBrandView : MonoBehaviour
    {
        private GUIStyle _labelStyle;

        private void OnGUI()
        {
            EnsureStyles();

            Color previous = GUI.color;
            GUI.color = new Color(0.04f, 0.05f, 0.08f, 1f);
            GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), string.Empty);
            GUI.color = previous;

            float size = Mathf.Min(Screen.width, Screen.height) * 0.22f;
            Rect logoRect = new Rect((Screen.width - size) * 0.5f, Screen.height * 0.36f, size, size);
            GulfRunBrandMark.Draw(logoRect);

            GUI.Label(new Rect(0f, logoRect.y + logoRect.height + 20f, Screen.width, 30f), "Loading...", _labelStyle);
        }

        private void EnsureStyles()
        {
            if (_labelStyle != null)
            {
                return;
            }

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter
            };
            _labelStyle.normal.textColor = new Color(0.87f, 0.78f, 0.62f, 1f);
        }
    }
}

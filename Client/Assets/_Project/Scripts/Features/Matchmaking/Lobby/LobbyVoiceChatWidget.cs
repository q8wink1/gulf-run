using GulfRun.Core.Managers;
using GulfRun.Domain;
using GulfRun.Features.Matchmaking.UI;
using UnityEngine;

namespace GulfRun.Features.Matchmaking.Lobby
{
    /// <summary>Sprint 14 Matchmaking Voice Chat widget (UI-only over <see cref="SettingsManager.VoiceChatMode"/>).</summary>
    public sealed class LobbyVoiceChatWidget : MonoBehaviour
    {
        private LobbyButtonPressAnimator _micAnim;

        private void OnGUI()
        {
            const float width = 150f;
            const float height = 36f;
            float x = Screen.width - width - 16f;
            float y = Screen.height - height - 16f;

            SettingsManager settings = SettingsManager.Instance;
            VoiceChatMode mode = settings != null ? settings.VoiceChatMode : VoiceChatMode.Muted;

            PreRaceLobbyTheme.DrawPanel(new Rect(x, y, width, height));
            string icon = mode == VoiceChatMode.Muted ? "🎤✕ Mute" : mode == VoiceChatMode.PushToTalk ? "🎤 PTT" : "🎤 Live";
            Rect buttonRect = _micAnim.Apply(new Rect(x + 6f, y + 4f, width - 12f, height - 8f), 2f);
            if (GUI.Button(buttonRect, icon, PreRaceLobbyTheme.PanelButton))
            {
                _micAnim.NotifyPressed();
                settings?.CycleVoiceChatMode();
            }
        }
    }
}

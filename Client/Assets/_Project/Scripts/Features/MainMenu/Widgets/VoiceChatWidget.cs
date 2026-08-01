using GulfRun.Core.Managers;
using GulfRun.Domain;
using GulfRun.Features.MainMenu.UI;
using UnityEngine;

namespace GulfRun.Features.MainMenu.Widgets
{
    /// <summary>
    /// Sprint 13 "VOICE CHAT": microphone icon, mute, push-to-talk, voice
    /// settings — a UI-only control surface over
    /// <see cref="SettingsManager.VoiceChatMode"/>. No real microphone
    /// capture/transport exists anywhere in the project yet (see Sprint 13
    /// report Remaining TODOs) — clicking the mic icon only changes local
    /// UI/settings state, the same "UI wired, backend still a TODO" honesty
    /// this project already applies to Friend Invites/Special Offers.
    /// </summary>
    public sealed class VoiceChatWidget : MonoBehaviour
    {
        private ButtonPressAnimator _micAnim;

        private void OnGUI()
        {
            const float width = 150f;
            const float height = 36f;
            float x = 16f;
            float y = Screen.height - height - 16f;

            SettingsManager settings = SettingsManager.Instance;
            VoiceChatMode mode = settings != null ? settings.VoiceChatMode : VoiceChatMode.Muted;

            MainMenuTheme.DrawPanel(new Rect(x, y, width, height));

            string icon = mode == VoiceChatMode.Muted ? "🎤✕" : mode == VoiceChatMode.PushToTalk ? "🎤 PTT" : "🎤 Live";

            Rect buttonRect = _micAnim.Apply(new Rect(x + 6f, y + 4f, width - 12f, height - 8f), 2f);
            if (GUI.Button(buttonRect, icon, MainMenuTheme.PanelButton))
            {
                _micAnim.NotifyPressed();
                settings?.CycleVoiceChatMode();
            }
        }
    }
}

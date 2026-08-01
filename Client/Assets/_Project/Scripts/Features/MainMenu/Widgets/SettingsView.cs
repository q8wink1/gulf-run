using GulfRun.Core;
using GulfRun.Core.Managers;
using GulfRun.Features.MainMenu.UI;
using UnityEngine;

namespace GulfRun.Features.MainMenu.Widgets
{
    /// <summary>
    /// Sprint 13 Settings panel (Top Bar gear icon): Master/Music/SFX/
    /// Ambient volume sliders plus the Voice Chat mode, all backed by
    /// <see cref="SettingsManager"/>. A <see cref="SceneSingleton{T}"/> so
    /// <see cref="TopBar.TopBarView"/> can open it with a direct
    /// <c>SettingsView.Instance?.Open()</c> call — no
    /// <see cref="Core.Services.MenuScreenRouter"/> indirection is needed
    /// here since both live in this same Features.MainMenu assembly.
    /// </summary>
    public sealed class SettingsView : SceneSingleton<SettingsView>
    {
        private bool _open;

        public void Open() => _open = true;

        public void Close() => _open = false;

        private void OnGUI()
        {
            if (!_open)
            {
                return;
            }

            const float width = 380f;
            const float height = 280f;
            float x = (Screen.width - width) * 0.5f;
            float y = (Screen.height - height) * 0.5f;

            MainMenuTheme.DrawPanel(new Rect(x, y, width, height));
            GUI.Label(new Rect(x + 14f, y + 10f, width - 28f, 26f), "SETTINGS", MainMenuTheme.Title);

            if (GUI.Button(new Rect(x + width - 34f, y + 8f, 24f, 24f), "X"))
            {
                Close();
                return;
            }

            SettingsManager settings = SettingsManager.Instance;
            if (settings == null)
            {
                GUI.Label(new Rect(x + 14f, y + 44f, width - 28f, 22f), "Settings are not available yet.", MainMenuTheme.MutedLabel);
                return;
            }

            float rowY = y + 48f;
            rowY = DrawVolumeSlider(x + 14f, rowY, width - 28f, "Master Volume", settings.MasterVolume, settings.SetMasterVolume);
            rowY = DrawVolumeSlider(x + 14f, rowY, width - 28f, "Music Volume", settings.MusicVolume, settings.SetMusicVolume);
            rowY = DrawVolumeSlider(x + 14f, rowY, width - 28f, "SFX Volume", settings.SfxVolume, settings.SetSfxVolume);
            rowY = DrawVolumeSlider(x + 14f, rowY, width - 28f, "Ambient Volume", settings.AmbientVolume, settings.SetAmbientVolume);

            GUI.Label(new Rect(x + 14f, rowY + 6f, width - 28f, 20f), "Voice Chat Mode: " + settings.VoiceChatMode, MainMenuTheme.Label);
            if (GUI.Button(new Rect(x + 14f, rowY + 30f, 160f, 26f), "Cycle Mode", MainMenuTheme.PanelButton))
            {
                settings.CycleVoiceChatMode();
            }
        }

        private static float DrawVolumeSlider(float x, float y, float width, string label, float value01, System.Action<float> onChanged)
        {
            GUI.Label(new Rect(x, y, width, 18f), label + ": " + Mathf.CeilToInt(value01 * 100f) + "%", MainMenuTheme.Label);
            float updated = GUI.HorizontalSlider(new Rect(x, y + 20f, width, 18f), value01, 0f, 1f);
            if (!Mathf.Approximately(updated, value01))
            {
                onChanged(updated);
            }

            return y + 44f;
        }
    }
}

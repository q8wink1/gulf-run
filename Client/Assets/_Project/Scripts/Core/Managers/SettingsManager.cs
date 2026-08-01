using System;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Composition root for the Sprint 13 Main Menu's Settings panel +
    /// Voice Chat widget: owns the four audio category volumes (forwarded
    /// live to <see cref="AudioManager"/>) and the local Voice Chat mode
    /// (Muted/Open Mic/Push-to-Talk — see <see cref="VoiceChatMode"/>).
    /// Deliberately thin and in-memory only for this sprint — no real
    /// microphone capture exists yet (see Sprint 13 report Remaining
    /// TODOs), and settings do not yet persist across app restarts (no
    /// PlayerPrefs/save-file wiring exists in the project yet either).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SettingsManager : Singleton<SettingsManager>
    {
        private float _masterVolume = 1f;
        private float _musicVolume = 0.8f;
        private float _sfxVolume = 1f;
        private float _ambientVolume = 0.7f;
        private VoiceChatMode _voiceChatMode = VoiceChatMode.Muted;

        public float MasterVolume => _masterVolume;
        public float MusicVolume => _musicVolume;
        public float SfxVolume => _sfxVolume;
        public float AmbientVolume => _ambientVolume;
        public VoiceChatMode VoiceChatMode => _voiceChatMode;

        /// <summary>Raised after any setting changes, so the Settings panel/Voice Chat widget can refresh their displayed sliders/toggle state.</summary>
        public event Action SettingsChanged;

        protected override void OnInitialize()
        {
            ApplyAllVolumesToAudioManager();
        }

        public void SetMasterVolume(float volume01)
        {
            _masterVolume = Clamp01(volume01);
            AudioManager.Instance?.SetMasterVolume(_masterVolume);
            SettingsChanged?.Invoke();
        }

        public void SetMusicVolume(float volume01)
        {
            _musicVolume = Clamp01(volume01);
            AudioManager.Instance?.SetMusicVolume(_musicVolume);
            SettingsChanged?.Invoke();
        }

        public void SetSfxVolume(float volume01)
        {
            _sfxVolume = Clamp01(volume01);
            AudioManager.Instance?.SetSfxVolume(_sfxVolume);
            SettingsChanged?.Invoke();
        }

        public void SetAmbientVolume(float volume01)
        {
            _ambientVolume = Clamp01(volume01);
            AudioManager.Instance?.SetAmbientVolume(_ambientVolume);
            SettingsChanged?.Invoke();
        }

        /// <summary>Sprint 13 (Voice Chat widget: mic icon tap). Cycles Muted → Open Mic → Push-to-Talk → Muted.</summary>
        public void CycleVoiceChatMode()
        {
            _voiceChatMode = _voiceChatMode switch
            {
                VoiceChatMode.Muted => VoiceChatMode.OpenMic,
                VoiceChatMode.OpenMic => VoiceChatMode.PushToTalk,
                _ => VoiceChatMode.Muted
            };

            SettingsChanged?.Invoke();
        }

        public void SetVoiceChatMode(VoiceChatMode mode)
        {
            _voiceChatMode = mode;
            SettingsChanged?.Invoke();
        }

        private void ApplyAllVolumesToAudioManager()
        {
            if (AudioManager.Instance == null)
            {
                return;
            }

            AudioManager.Instance.SetMasterVolume(_masterVolume);
            AudioManager.Instance.SetMusicVolume(_musicVolume);
            AudioManager.Instance.SetSfxVolume(_sfxVolume);
            AudioManager.Instance.SetAmbientVolume(_ambientVolume);
        }

        private static float Clamp01(float value) => value < 0f ? 0f : value > 1f ? 1f : value;
    }
}

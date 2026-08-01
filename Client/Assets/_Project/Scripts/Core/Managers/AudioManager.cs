using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Central entry point for playback of UI, gameplay, character, environment,
    /// weapon and voice audio, and for music playback coordination.
    /// References: P035 (Audio System), P036 (Music System), P034 (Settings).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AudioManager : Singleton<AudioManager>
    {
        private AudioSource _sfxSource;

        protected override void OnInitialize()
        {
            // TODO(Sprint 2+): Wire up AudioMixer groups and category volumes
            // (Master, Music, SFX, Voice Chat) per the Settings System (P034).
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
        }

        /// <summary>
        /// Minimal one-shot SFX playback (e.g. weapon pickup/activation/
        /// impact/cooldown sounds — see Sprint 5). A no-op if
        /// <paramref name="clip"/> is null, so callers never need to guard
        /// against still-unassigned placeholder clips themselves.
        /// </summary>
        public void PlayOneShot(AudioClip clip, float volume = 1f)
        {
            if (clip == null || _sfxSource == null)
            {
                return;
            }

            _sfxSource.PlayOneShot(clip, volume);
        }
    }
}

using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Central entry point for playback of UI, gameplay, character, environment,
    /// weapon and voice audio, and for music playback coordination.
    /// References: P035 (Audio System), P036 (Music System), P034 (Settings).
    /// Sprint 13 (Main Menu Settings panel) adds Master/Music/SFX/Ambient
    /// category volumes — every source's live <c>.volume</c> is always
    /// <c>requestedVolume * categoryVolume * masterVolume</c>, so a Settings
    /// slider change takes effect immediately on whatever is already
    /// playing, with zero per-call volume math for the many existing
    /// <see cref="PlayOneShot"/>/<see cref="PlayMusic"/>/<see cref="PlayAmbient"/> call sites.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AudioManager : Singleton<AudioManager>
    {
        private AudioSource _sfxSource;
        private AudioSource _musicSource;
        private AudioSource _ambientSource;

        private float _masterVolume = 1f;
        private float _musicVolume = 1f;
        private float _sfxVolume = 1f;
        private float _ambientVolume = 1f;

        private float _musicRequestedVolume = 1f;
        private float _ambientRequestedVolume = 1f;

        public float MasterVolume => _masterVolume;
        public float MusicVolume => _musicVolume;
        public float SfxVolume => _sfxVolume;
        public float AmbientVolume => _ambientVolume;

        protected override void OnInitialize()
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;

            // Sprint 7: separate looping source for Victory Ceremony music,
            // kept independent of one-shot SFX so a music track is never cut
            // short by an unrelated PlayOneShot call.
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;

            // Sprint 12: separate looping source for per-city ambient audio
            // (birds/wind/sea/city ambience, day and night variations) —
            // independent of Music so a city ambience swap never interrupts
            // a Victory Ceremony track, and independent of SFX so it is
            // never cut short by a one-shot pickup/impact sound.
            _ambientSource = gameObject.AddComponent<AudioSource>();
            _ambientSource.playOnAwake = false;
        }

        /// <summary>Sprint 13 (Settings panel "Master Volume"). Scales every category's already-playing audio immediately.</summary>
        public void SetMasterVolume(float volume01)
        {
            _masterVolume = Clamp01(volume01);
            ApplyLoopingVolumes();
        }

        /// <summary>Sprint 13 (Settings panel "Music Volume").</summary>
        public void SetMusicVolume(float volume01)
        {
            _musicVolume = Clamp01(volume01);
            ApplyLoopingVolumes();
        }

        /// <summary>Sprint 13 (Settings panel "SFX Volume"). Affects only future <see cref="PlayOneShot"/> calls — one-shots already playing are short-lived by design.</summary>
        public void SetSfxVolume(float volume01)
        {
            _sfxVolume = Clamp01(volume01);
        }

        /// <summary>Sprint 13 (Settings panel "Ambient Volume").</summary>
        public void SetAmbientVolume(float volume01)
        {
            _ambientVolume = Clamp01(volume01);
            ApplyLoopingVolumes();
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

            _sfxSource.PlayOneShot(clip, volume * _sfxVolume * _masterVolume);
        }

        /// <summary>Starts (or restarts) looping music playback — e.g. the Victory Ceremony track (Sprint 7). A no-op if <paramref name="clip"/> is null.</summary>
        public void PlayMusic(AudioClip clip, float volume = 1f, bool loop = true)
        {
            if (clip == null || _musicSource == null)
            {
                return;
            }

            _musicRequestedVolume = volume;
            _musicSource.clip = clip;
            _musicSource.volume = EffectiveVolume(volume, _musicVolume);
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        /// <summary>Stops any currently playing music. A no-op if nothing is playing.</summary>
        public void StopMusic()
        {
            _musicSource?.Stop();
        }

        /// <summary>
        /// Starts (or restarts) looping ambient environment audio — e.g. the
        /// active map's day/night city ambience (Sprint 12). Passing a null
        /// clip stops the current ambience instead of no-op'ing, so swapping
        /// maps/time-of-day can cleanly silence ambience with no clip
        /// authored yet.
        /// </summary>
        public void PlayAmbient(AudioClip clip, float volume = 1f, bool loop = true)
        {
            if (_ambientSource == null)
            {
                return;
            }

            if (clip == null)
            {
                _ambientSource.Stop();
                return;
            }

            _ambientRequestedVolume = volume;
            _ambientSource.clip = clip;
            _ambientSource.volume = EffectiveVolume(volume, _ambientVolume);
            _ambientSource.loop = loop;
            _ambientSource.Play();
        }

        /// <summary>Stops any currently playing ambient audio. A no-op if nothing is playing.</summary>
        public void StopAmbient()
        {
            _ambientSource?.Stop();
        }

        private void ApplyLoopingVolumes()
        {
            if (_musicSource != null)
            {
                _musicSource.volume = EffectiveVolume(_musicRequestedVolume, _musicVolume);
            }

            if (_ambientSource != null)
            {
                _ambientSource.volume = EffectiveVolume(_ambientRequestedVolume, _ambientVolume);
            }
        }

        private float EffectiveVolume(float requestedVolume, float categoryVolume) => requestedVolume * categoryVolume * _masterVolume;

        private static float Clamp01(float value) => value < 0f ? 0f : value > 1f ? 1f : value;
    }
}

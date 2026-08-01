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
        private AudioSource _musicSource;
        private AudioSource _ambientSource;

        protected override void OnInitialize()
        {
            // TODO(Sprint 2+): Wire up AudioMixer groups and category volumes
            // (Master, Music, SFX, Voice Chat) per the Settings System (P034).
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

        /// <summary>Starts (or restarts) looping music playback — e.g. the Victory Ceremony track (Sprint 7). A no-op if <paramref name="clip"/> is null.</summary>
        public void PlayMusic(AudioClip clip, float volume = 1f, bool loop = true)
        {
            if (clip == null || _musicSource == null)
            {
                return;
            }

            _musicSource.clip = clip;
            _musicSource.volume = volume;
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

            _ambientSource.clip = clip;
            _ambientSource.volume = volume;
            _ambientSource.loop = loop;
            _ambientSource.Play();
        }

        /// <summary>Stops any currently playing ambient audio. A no-op if nothing is playing.</summary>
        public void StopAmbient()
        {
            _ambientSource?.Stop();
        }
    }
}

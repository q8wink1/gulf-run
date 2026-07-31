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
        protected override void OnInitialize()
        {
            // TODO(Sprint 2+): Wire up AudioMixer groups and category volumes
            // (Master, Music, SFX, Voice Chat) per the Settings System (P034).
        }
    }
}

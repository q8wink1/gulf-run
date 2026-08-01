namespace GulfRun.Domain
{
    /// <summary>
    /// Local Voice Chat input mode (Sprint 13 "VOICE CHAT: Microphone icon.
    /// Voice settings. Mute. Push-to-talk."). No real microphone capture or
    /// voice network channel exists yet (see <c>Design/GDD/P016-VOICE-CHAT-SYSTEM-v1.0.md</c>) —
    /// this is the honest, local-only UI state machine a future real
    /// capture/transport pipeline plugs into, the same "real system, no
    /// backend yet" posture every other Sprint 13 placeholder follows.
    /// </summary>
    public enum VoiceChatMode
    {
        Muted,
        OpenMic,
        PushToTalk
    }
}

namespace GulfRun.Domain
{
    /// <summary>
    /// Device-agnostic snapshot of player intent for a single simulation step.
    /// Kept separate from raw device polling so the same intent shape can later
    /// be captured, buffered and sent over the network for multiplayer input
    /// synchronization without touching movement/animation code.
    /// </summary>
    public readonly struct PlayerInputIntent
    {
        public readonly bool JumpRequested;

        public PlayerInputIntent(bool jumpRequested)
        {
            JumpRequested = jumpRequested;
        }

        public static PlayerInputIntent None => new PlayerInputIntent(jumpRequested: false);
    }
}

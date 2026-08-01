namespace GulfRun.Domain
{
    /// <summary>
    /// Identifies one player across a match: a stable <see cref="PlayerId"/>,
    /// a <see cref="DisplayName"/> for UI, the transport-assigned
    /// <see cref="ConnectionId"/> for this session, and a reserved
    /// <see cref="ProfileId"/> slot for a future account/profile system
    /// (empty string until that system exists).
    /// </summary>
    public readonly struct PlayerIdentity
    {
        public readonly string PlayerId;
        public readonly string DisplayName;
        public readonly int ConnectionId;
        public readonly string ProfileId;

        public PlayerIdentity(string playerId, string displayName, int connectionId, string profileId)
        {
            PlayerId = playerId;
            DisplayName = displayName;
            ConnectionId = connectionId;
            ProfileId = profileId;
        }

        public PlayerIdentity WithConnectionId(int connectionId) =>
            new PlayerIdentity(PlayerId, DisplayName, connectionId, ProfileId);
    }
}

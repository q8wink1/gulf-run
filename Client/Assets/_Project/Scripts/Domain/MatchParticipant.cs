namespace GulfRun.Domain
{
    /// <summary>
    /// One roster entry in the Lobby: an identity plus its Host/Ready/
    /// Connection status. Immutable; use the <c>With*</c> helpers to derive
    /// an updated copy instead of mutating in place.
    /// </summary>
    public readonly struct MatchParticipant
    {
        public readonly PlayerIdentity Identity;
        public readonly bool IsHost;
        public readonly PlayerReadyState Ready;
        public readonly ConnectionState Connection;

        public MatchParticipant(PlayerIdentity identity, bool isHost, PlayerReadyState ready, ConnectionState connection)
        {
            Identity = identity;
            IsHost = isHost;
            Ready = ready;
            Connection = connection;
        }

        public MatchParticipant WithReady(PlayerReadyState ready) =>
            new MatchParticipant(Identity, IsHost, ready, Connection);

        public MatchParticipant WithConnection(ConnectionState connection) =>
            new MatchParticipant(Identity, IsHost, Ready, connection);

        /// <summary>Sprint 15 (Network "Host migration ready"): produces an updated copy with a new Host flag, used by <c>LobbyManager.PromoteToHost</c> when the current host leaves.</summary>
        public MatchParticipant WithHost(bool isHost) =>
            new MatchParticipant(Identity, isHost, Ready, Connection);
    }
}

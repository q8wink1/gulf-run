namespace GulfRun.Core.Networking
{
    /// <summary>
    /// Minimal service locator for the active <see cref="IMatchTransport"/> —
    /// same pattern as <see cref="Services.RunSpeedService"/> and
    /// <see cref="Services.GameStateService"/>. The getter self-initializes
    /// with a <see cref="LocalLoopbackTransport"/> if nothing has been
    /// assigned yet, so every consumer (Connection/Lobby/Match/Session
    /// managers) can safely use <see cref="Current"/> the instant it runs,
    /// with no Awake/OnEnable ordering dependency on whatever component
    /// would otherwise be responsible for registering a transport first.
    /// </summary>
    public static class MatchTransportService
    {
        private static IMatchTransport _current;

        public static IMatchTransport Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new LocalLoopbackTransport();
                }

                return _current;
            }
            set => _current = value;
        }
    }
}

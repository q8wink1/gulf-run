namespace GulfRun.Core.Backend
{
    /// <summary>
    /// Minimal service locator for the active <see cref="IOnlineBackendService"/> —
    /// identical pattern to <see cref="Networking.MatchTransportService"/>.
    /// Self-initializes with <see cref="LocalOnlineBackendService"/> so every
    /// Features.Online manager can safely use <see cref="Current"/> the
    /// instant it runs; swap in a real cloud-backed implementation later by
    /// assigning <see cref="Current"/> once at startup, with zero changes
    /// to any caller.
    /// </summary>
    public static class OnlineBackendService
    {
        private static IOnlineBackendService _current;

        public static IOnlineBackendService Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new LocalOnlineBackendService();
                }

                return _current;
            }
            set => _current = value;
        }
    }
}

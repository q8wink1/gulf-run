namespace GulfRun.Core.Backend
{
    /// <summary>Minimal service locator for the active <see cref="IStoreBackendService"/> — identical pattern to <see cref="OnlineBackendService"/>. Self-initializes with <see cref="LocalStoreBackendService"/> so every Features.Store manager can safely use <see cref="Current"/> the instant it runs.</summary>
    public static class StoreBackendService
    {
        private static IStoreBackendService _current;

        public static IStoreBackendService Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new LocalStoreBackendService();
                }

                return _current;
            }
            set => _current = value;
        }
    }
}

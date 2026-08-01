namespace GulfRun.Core.Backend
{
    /// <summary>Minimal service locator for the active <see cref="IProgressionBackendService"/> — identical pattern to <see cref="StoreBackendService"/>. Self-initializes with <see cref="LocalProgressionBackendService"/> so every Features.Progression manager can safely use <see cref="Current"/> the instant it runs.</summary>
    public static class ProgressionBackendService
    {
        private static IProgressionBackendService _current;

        public static IProgressionBackendService Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new LocalProgressionBackendService();
                }

                return _current;
            }
            set => _current = value;
        }
    }
}

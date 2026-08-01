namespace GulfRun.Core.Services
{
    /// <summary>
    /// Local network diagnostics for Race HUD debug (ping). Implemented by
    /// <c>Features.Multiplayer.Connection.NetworkDiagnosticsBridge</c>.
    /// </summary>
    public interface INetworkDiagnosticsProvider
    {
        float LocalPingMilliseconds { get; }
    }

    public static class NetworkDiagnosticsService
    {
        public static INetworkDiagnosticsProvider Current { get; set; }
    }
}

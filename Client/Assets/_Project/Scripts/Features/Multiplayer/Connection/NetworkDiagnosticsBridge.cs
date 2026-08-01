using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using UnityEngine;

namespace GulfRun.Features.Multiplayer.Connection
{
    /// <summary>
    /// Publishes local ping for Race HUD debug via
    /// <see cref="INetworkDiagnosticsProvider"/> without Features.RaceHud
    /// referencing Features.Multiplayer.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class NetworkDiagnosticsBridge : SceneSingleton<NetworkDiagnosticsBridge>, INetworkDiagnosticsProvider
    {
        public float LocalPingMilliseconds
        {
            get
            {
                ConnectionManager connection = ConnectionManager.Instance;
                IMatchTransport transport = MatchTransportService.Current;
                if (connection == null || transport == null)
                {
                    return 0f;
                }

                return connection.PingSecondsFor(transport.LocalConnectionId) * 1000f;
            }
        }

        private void OnEnable() => NetworkDiagnosticsService.Current = this;

        private void OnDisable()
        {
            if (ReferenceEquals(NetworkDiagnosticsService.Current, this))
            {
                NetworkDiagnosticsService.Current = null;
            }
        }
    }
}

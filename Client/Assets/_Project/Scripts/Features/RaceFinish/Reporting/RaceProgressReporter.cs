using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.RaceFinish.Configuration;
using UnityEngine;

namespace GulfRun.Features.RaceFinish.Reporting
{
    /// <summary>
    /// Periodically publishes the local player's live race progress
    /// (distance/coins) to the network at a configurable rate — the same
    /// "read state via an interface, publish over the transport" shape
    /// <c>NetworkPlayerSync</c> already uses for position sync. Reads local
    /// progress via <see cref="IRaceProgressProvider"/> only, so this never
    /// references the EndlessRunner feature directly. A no-op whenever no
    /// match is active (e.g. single-player endless mode), so it never
    /// interferes with that mode.
    /// </summary>
    public sealed class RaceProgressReporter : MonoBehaviour
    {
        [SerializeField] private RaceFinishConfig config;

        private float _sendTimer;

        private void Update()
        {
            IMatchTransport transport = MatchTransportService.Current;
            IRaceProgressProvider progress = RaceProgressService.Current;

            if (transport == null || !transport.IsActive || progress == null)
            {
                return;
            }

            float interval = config != null ? config.ProgressReportIntervalSeconds : 0.5f;
            _sendTimer += Time.deltaTime;
            if (_sendTimer < interval)
            {
                return;
            }

            _sendTimer = 0f;

            var report = new RaceProgressReport(
                transport.LocalConnectionId,
                (float)progress.DistanceMeters,
                progress.CoinsCollected,
                Time.timeAsDouble);

            transport.ReportRaceProgress(report);
        }
    }
}

using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Domain;
using GulfRun.Features.Multiplayer.Configuration;
using UnityEngine;

namespace GulfRun.Features.Multiplayer.Sync
{
    /// <summary>
    /// Buffers the last two received <see cref="NetworkPlayerSnapshot"/>s per
    /// remote connection and resolves a smooth, jitter-resistant render pose
    /// via the pure <see cref="NetworkInterpolator"/> — the Network
    /// Interpolation + prediction-preparation requirement. Scene-scoped
    /// (resets every Gameplay reload). Does not spawn or move any visual
    /// player avatar itself (no remote-player prefab exists yet — final
    /// gameplay wiring is out of scope for this foundation sprint); a future
    /// PlayerVisual/remote-avatar component is the natural consumer of
    /// <see cref="TryGetInterpolatedSnapshot"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RemotePlayerSyncHub : SceneSingleton<RemotePlayerSyncHub>
    {
        [SerializeField] private NetworkSyncConfig config;

        private readonly Dictionary<int, NetworkPlayerSnapshot> _previous = new Dictionary<int, NetworkPlayerSnapshot>();
        private readonly Dictionary<int, NetworkPlayerSnapshot> _latest = new Dictionary<int, NetworkPlayerSnapshot>();

        public IReadOnlyCollection<int> TrackedConnectionIds => _latest.Keys;

        private void OnEnable()
        {
            MatchTransportService.Current.SnapshotReceived += HandleSnapshotReceived;
        }

        private void OnDisable()
        {
            MatchTransportService.Current.SnapshotReceived -= HandleSnapshotReceived;
        }

        /// <summary>Render time (seconds) to resolve remote poses at, delayed by <see cref="NetworkSyncConfig.InterpolationDelaySeconds"/> to keep a smooth interpolation buffer.</summary>
        public double ResolveRenderTime(double localTimeSeconds)
        {
            float delay = config != null ? config.InterpolationDelaySeconds : 0.1f;
            return localTimeSeconds - delay;
        }

        public bool TryGetInterpolatedSnapshot(int connectionId, double renderTimeSeconds, out NetworkPlayerSnapshot result)
        {
            if (!_latest.TryGetValue(connectionId, out NetworkPlayerSnapshot to))
            {
                result = default;
                return false;
            }

            NetworkPlayerSnapshot from = _previous.TryGetValue(connectionId, out NetworkPlayerSnapshot previous) ? previous : to;
            float maxExtrapolation = config != null ? config.MaxExtrapolationSeconds : 0.25f;
            result = NetworkInterpolator.Resolve(from, to, renderTimeSeconds, maxExtrapolation);
            return true;
        }

        public void Clear()
        {
            _previous.Clear();
            _latest.Clear();
        }

        private void HandleSnapshotReceived(NetworkPlayerSnapshot snapshot)
        {
            if (_latest.TryGetValue(snapshot.ConnectionId, out NetworkPlayerSnapshot currentLatest))
            {
                _previous[snapshot.ConnectionId] = currentLatest;
            }

            _latest[snapshot.ConnectionId] = snapshot;
        }
    }
}

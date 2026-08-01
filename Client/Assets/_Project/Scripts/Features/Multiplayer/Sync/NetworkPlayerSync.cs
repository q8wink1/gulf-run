using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Multiplayer.Configuration;
using UnityEngine;

namespace GulfRun.Features.Multiplayer.Sync
{
    /// <summary>
    /// Periodically publishes the local player's position/rotation/animation
    /// state to the network at a configurable, bandwidth-conscious rate
    /// (<see cref="NetworkSyncConfig.SnapshotSendRateHz"/> — well below 60Hz
    /// by default, satisfying "Minimize bandwidth"). Reads the local state
    /// via <see cref="ILocalPlayerStateProvider"/> only, so this never
    /// references the PlayerController feature directly. Add this component
    /// alongside a local Player instance once one exists in a
    /// multiplayer-enabled scene (no Player.prefab is instantiated in any
    /// scene yet — see Sprint 2/3 reports).
    /// </summary>
    public sealed class NetworkPlayerSync : MonoBehaviour
    {
        [SerializeField] private NetworkSyncConfig config;

        private float _sendTimer;

        private void Update()
        {
            IMatchTransport transport = MatchTransportService.Current;
            ILocalPlayerStateProvider localState = LocalPlayerStateService.Current;

            if (!transport.IsActive || localState == null)
            {
                return;
            }

            float interval = config != null ? config.SnapshotSendIntervalSeconds : 1f / 15f;
            _sendTimer += Time.deltaTime;
            if (_sendTimer < interval)
            {
                return;
            }

            _sendTimer = 0f;

            var snapshot = new NetworkPlayerSnapshot(
                transport.LocalConnectionId,
                new NetVector2(localState.Position.x, localState.Position.y),
                localState.RotationDegrees,
                localState.AnimationState,
                Time.timeAsDouble);

            transport.SendSnapshot(snapshot);
        }
    }
}

using UnityEngine;

namespace GulfRun.Features.Multiplayer.Configuration
{
    /// <summary>
    /// All tunable values for the Multiplayer Foundation in one
    /// ScriptableObject — no hardcoded match size, timings, or rates
    /// anywhere in the Connection/Lobby/Match/Session/Spawn/Sync code.
    /// </summary>
    [CreateAssetMenu(fileName = "NetworkSyncConfig", menuName = "GulfRun/Multiplayer/Network Sync Config")]
    public sealed class NetworkSyncConfig : ScriptableObject
    {
        [Header("Match")]
        [Tooltip("Maximum number of players in a single match.")]
        [SerializeField] private int maxPlayers = 4;
        [Tooltip("Minimum number of players (all of whom must be Ready) required to start the countdown.")]
        [SerializeField] private int minimumPlayersToStart = 2;

        [Header("Countdown")]
        [Tooltip("Sprint 15: the Pre-Race Lobby's Auto Start countdown length in seconds, ticked by MatchManager the instant every required player is Ready (brief: '5 4 3 2 1 GO' -> 5). Was a still-unconnected-to-any-UI 3s race-start placeholder before Sprint 15's Lobby scene became this value's first real consumer.")]
        [SerializeField] private float countdownDurationSeconds = 5f;

        [Header("Synchronization")]
        [Tooltip("How many player-state snapshots are sent per second. Lower = less bandwidth.")]
        [SerializeField] private float snapshotSendRateHz = 15f;
        [Tooltip("Render-time delay (seconds) used to buffer remote snapshots for smooth interpolation.")]
        [SerializeField] private float interpolationDelaySeconds = 0.1f;
        [Tooltip("Maximum time (seconds) to extrapolate beyond the last received snapshot before holding in place.")]
        [SerializeField] private float maxExtrapolationSeconds = 0.25f;

        [Header("Connection")]
        [Tooltip("Seconds without any data from a participant before they are flagged as Timed Out.")]
        [SerializeField] private float connectionTimeoutSeconds = 10f;

        public int MaxPlayers => maxPlayers;
        public int MinimumPlayersToStart => minimumPlayersToStart;
        public float CountdownDurationSeconds => countdownDurationSeconds;
        public float SnapshotSendRateHz => snapshotSendRateHz;
        public float SnapshotSendIntervalSeconds => snapshotSendRateHz > 0f ? 1f / snapshotSendRateHz : 0f;
        public float InterpolationDelaySeconds => interpolationDelaySeconds;
        public float MaxExtrapolationSeconds => maxExtrapolationSeconds;
        public float ConnectionTimeoutSeconds => connectionTimeoutSeconds;
    }
}

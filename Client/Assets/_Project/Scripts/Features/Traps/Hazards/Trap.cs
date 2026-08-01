using GulfRun.Core.Networking;
using GulfRun.Core.Pooling;
using GulfRun.Domain;
using GulfRun.Features.Traps.Configuration;
using UnityEngine;

namespace GulfRun.Features.Traps.Hazards
{
    /// <summary>
    /// A single active trap instance. Spawned/pooled/recycled entirely by
    /// <c>TrapSpawnController</c> reacting to host-authoritative network
    /// events — this component only knows how to (optionally) drift along
    /// the track for its lifetime and report when a player touches it. It
    /// never decides gameplay outcomes itself: it always asks
    /// <c>TrapAuthority</c> to validate the trigger, exactly like
    /// <c>Features.Weapons.ItemBoxes.ItemBox</c> never grants a weapon
    /// itself.
    ///
    /// Unlike ItemBox, this does NOT disable its collider after one contact —
    /// "every player can be affected equally, no immunity", so the same
    /// instance may legitimately trigger again if a player re-enters it
    /// (careless play should not be free). Only <see cref="TrapDefinition.ContinuousWhileStanding"/>
    /// (Hot Sand) additionally re-reports on a throttled timer while a
    /// player remains inside; every other trap fires once per contact via
    /// Unity's own OnTriggerEnter2D semantics, so no extra debounce
    /// bookkeeping is needed.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class Trap : MonoBehaviour, IPoolable
    {
        [SerializeField] private SpriteRenderer visual;

        private Collider2D _collider;
        private TrapDefinition _definition;
        private int _trapInstanceId = -1;
        private TrapId _trapId;
        private Vector2 _moveVelocity;
        private float _nextContinuousReportTime;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        /// <summary>Called by TrapSpawnController immediately after the pool activates this instance, to attach the host-minted network identity and per-type tuning.</summary>
        public void Configure(int trapInstanceId, TrapId trapId, TrapDefinition definition)
        {
            _trapInstanceId = trapInstanceId;
            _trapId = trapId;
            _definition = definition;
            _moveVelocity = definition != null && definition.MovesAlongTrack
                ? new Vector2(definition.MoveSpeedMetersPerSecond, 0f)
                : Vector2.zero;

            if (visual != null && definition != null)
            {
                visual.color = definition.DebugTint;
            }
        }

        public void OnSpawned()
        {
            if (_collider != null)
            {
                _collider.enabled = true;
            }

            _nextContinuousReportTime = 0f;
        }

        public void OnDespawned()
        {
            _definition = null;
            _trapInstanceId = -1;
        }

        private void Update()
        {
            if (_moveVelocity != Vector2.zero)
            {
                transform.position += (Vector3)(_moveVelocity * Time.deltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                ReportTrigger();
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_definition == null || !_definition.ContinuousWhileStanding || !other.CompareTag("Player"))
            {
                return;
            }

            if (Time.time < _nextContinuousReportTime)
            {
                return;
            }

            _nextContinuousReportTime = Time.time + _definition.ContinuousRefreshIntervalSeconds;
            ReportTrigger();
        }

        private void ReportTrigger()
        {
            if (_trapInstanceId < 0)
            {
                return;
            }

            IMatchTransport transport = MatchTransportService.Current;
            if (transport == null)
            {
                return;
            }

            // Only the local player has a physically simulated collider today
            // (no networked remote avatar exists yet — same Sprint 4/5
            // limitation Features.Weapons.ItemBoxes.ItemBox inherits), so the
            // reported target is always the local connection until real
            // remote avatars are physically spawned.
            transport.ReportTrapTrigger(new TrapTriggerEvent(_trapInstanceId, _trapId, transport.LocalConnectionId, Time.timeAsDouble));
        }
    }
}

using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.CameraSystem
{
    /// <summary>
    /// Smooth side-scrolling follow camera with optional world-space bounds,
    /// Sprint 15 look-ahead, gentle vertical bob, and impact-only shake.
    /// Takes a plain <see cref="Transform"/> target rather than referencing
    /// the PlayerController feature directly (features must not reference
    /// other features — see FOLDER_ARCHITECTURE.md §4).
    /// </summary>
    public sealed class SideScrollCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private CameraFollowConfig config;

        private Vector3 _velocity;
        private float _impactShakeRemaining;
        private float _impactShakeStrength;
        private Vector3 _lastTargetPosition;

        public Transform Target
        {
            get => target;
            set => target = value;
        }

        /// <summary>Brief impact shake — the only shake source (landing/weapon hits).</summary>
        public void TriggerImpactShake(float strength01 = 1f)
        {
            if (config == null)
            {
                return;
            }

            _impactShakeStrength = Mathf.Clamp01(strength01) * config.ImpactShakeMaxOffset;
            _impactShakeRemaining = config.ImpactShakeDecaySeconds;
        }

        private void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }

            if (target != null)
            {
                _lastTargetPosition = target.position;
            }
        }

        private void LateUpdate()
        {
            if (target == null || config == null)
            {
                return;
            }

            float deltaX = target.position.x - _lastTargetPosition.x;
            _lastTargetPosition = target.position;

            Vector3 desired = target.position + config.Offset;
            if (deltaX > 0.001f)
            {
                desired.x += config.LookAheadMeters;
            }

            if (!config.FollowY)
            {
                desired.y = transform.position.y;
            }
            else
            {
                desired.y += CelebrationAnimation.EvaluateOffset(Time.time, config.VerticalBobAmplitude, config.VerticalBobFrequencyHz);
            }

            if (config.UseBoundsX)
            {
                desired.x = Mathf.Clamp(desired.x, config.MinX, config.MaxX);
            }

            if (config.UseBoundsY)
            {
                desired.y = Mathf.Clamp(desired.y, config.MinY, config.MaxY);
            }

            if (_impactShakeRemaining > 0f)
            {
                float t = _impactShakeRemaining / Mathf.Max(0.01f, config.ImpactShakeDecaySeconds);
                float shake = _impactShakeStrength * t;
                desired.x += CelebrationAnimation.EvaluateOffset(Time.time * 17f, shake, 9f);
                desired.y += CelebrationAnimation.EvaluateOffset(Time.time * 23f, shake * 0.6f, 11f);
                _impactShakeRemaining -= Time.deltaTime;
            }

            transform.position = Vector3.SmoothDamp(
                transform.position,
                desired,
                ref _velocity,
                config.SmoothTime);
        }
    }
}

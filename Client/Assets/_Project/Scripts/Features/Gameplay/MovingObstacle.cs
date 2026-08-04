using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.9 — moving obstacle foundation. Stores motion tuning only;
    /// no Update motion or collision consequences this sprint.
    /// </summary>
    public sealed class MovingObstacle : Obstacle
    {
        [Header("Motion (future)")]
        [Tooltip("Meters per second along local move axis when motion is enabled later.")]
        [SerializeField] private float moveSpeed = 2f;

        [Tooltip("Local-space direction used by a future motion tick.")]
        [SerializeField] private Vector3 moveAxis = Vector3.right;

        [Tooltip("Half-extent of a future ping-pong path (meters).")]
        [SerializeField] private float moveRange = 1.1f;

        public override ObstacleType Type => ObstacleType.Moving;
        public float MoveSpeed => moveSpeed;
        public Vector3 MoveAxis => moveAxis;
        public float MoveRange => moveRange;

#if UNITY_EDITOR
        protected override Color GizmoColor => new Color(1f, 0.55f, 0.15f, 0.9f);

        protected override void DrawExtraGizmosSelected()
        {
            Vector3 axis = moveAxis.sqrMagnitude > 0.0001f ? moveAxis.normalized : Vector3.right;
            Vector3 worldAxis = transform.TransformDirection(axis);
            Vector3 origin = transform.position + Vector3.up * (ResolveHeight() * 0.5f);
            Gizmos.color = new Color(1f, 0.7f, 0.2f, 0.85f);
            Gizmos.DrawLine(origin - worldAxis * moveRange, origin + worldAxis * moveRange);
        }
#endif
    }
}

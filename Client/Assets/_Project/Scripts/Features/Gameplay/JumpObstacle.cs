using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>Sprint 23.9 — jump-over obstacle foundation (no clearance checks yet).</summary>
    public sealed class JumpObstacle : Obstacle
    {
        public override ObstacleType Type => ObstacleType.Jump;

#if UNITY_EDITOR
        protected override Color GizmoColor => new Color(0.35f, 0.75f, 1f, 0.9f);
#endif
    }
}

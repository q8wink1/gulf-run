using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>Sprint 23.9 — fixed barrier obstacle (foundation only).</summary>
    public sealed class StaticObstacle : Obstacle
    {
        public override ObstacleType Type => ObstacleType.Static;

#if UNITY_EDITOR
        protected override Color GizmoColor => new Color(1f, 0.3f, 0.25f, 0.9f);
#endif
    }
}

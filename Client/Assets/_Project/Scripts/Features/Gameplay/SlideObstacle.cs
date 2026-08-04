using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>Sprint 23.9 — slide-under obstacle foundation (no clearance checks yet).</summary>
    public sealed class SlideObstacle : Obstacle
    {
        public override ObstacleType Type => ObstacleType.Slide;

#if UNITY_EDITOR
        protected override Color GizmoColor => new Color(0.55f, 0.35f, 1f, 0.9f);
#endif
    }
}

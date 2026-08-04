using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.6 / 23.10 / 23.12 — fixed placeholder slot on a <see cref="TrackSegment"/>
    /// for a spawnable. Markers are authoring data; SpawnManager plans and may
    /// pool-Get obstacles / collectibles at these poses. Lane comes from the
    /// serialized field or from local X (aligned with runner lane spacing).
    /// </summary>
    public sealed class TrackSpawnMarker : MonoBehaviour
    {
        private const float DefaultLaneSpacing = 2.2f;

        [SerializeField] private SpawnCategory category = SpawnCategory.Obstacle;

        [Header("Lane (Sprint 23.10)")]
        [SerializeField] private RunnerLane lane = RunnerLane.Center;

        [Tooltip("When true, ResolveLane picks nearest Left/Center/Right from local X.")]
        [SerializeField] private bool resolveLaneFromTransformX = true;

        [SerializeField] private float laneSpacing = DefaultLaneSpacing;
        [SerializeField] private float laneCenterX;

        public SpawnCategory Category => category;
        public RunnerLane Lane => lane;
        public bool ResolveLaneFromTransformX => resolveLaneFromTransformX;

        /// <summary>
        /// Lane used for obstacle placement. Prefers local-X inference when enabled,
        /// otherwise the Inspector <see cref="Lane"/> field.
        /// </summary>
        public RunnerLane ResolveLane()
        {
            if (!resolveLaneFromTransformX)
            {
                return lane;
            }

            float spacing = laneSpacing > 0.1f ? laneSpacing : DefaultLaneSpacing;
            float x = transform.localPosition.x;
            float leftX = laneCenterX - spacing;
            float rightX = laneCenterX + spacing;
            float distLeft = Mathf.Abs(x - leftX);
            float distCenter = Mathf.Abs(x - laneCenterX);
            float distRight = Mathf.Abs(x - rightX);

            if (distLeft <= distCenter && distLeft <= distRight)
            {
                return RunnerLane.Left;
            }

            if (distRight <= distCenter && distRight <= distLeft)
            {
                return RunnerLane.Right;
            }

            return RunnerLane.Center;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            laneSpacing = Mathf.Max(0.1f, laneSpacing);
            if (!resolveLaneFromTransformX)
            {
                return;
            }

            lane = ResolveLane();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = CategoryColor(category);
            Gizmos.DrawWireSphere(transform.position, 0.35f);
            if (category == SpawnCategory.Obstacle)
            {
                RunnerLane resolved = ResolveLane();
                Vector3 tick = transform.position;
                tick.x = RunnerLaneMath.LaneX(resolved, laneCenterX, laneSpacing > 0.1f ? laneSpacing : DefaultLaneSpacing);
                Gizmos.DrawWireCube(tick, new Vector3(0.25f, 0.15f, 0.25f));
            }
        }

        private static Color CategoryColor(SpawnCategory kind)
        {
            switch (kind)
            {
                case SpawnCategory.Obstacle: return new Color(1f, 0.25f, 0.25f, 0.9f);
                case SpawnCategory.Coin: return new Color(1f, 0.85f, 0.2f, 0.9f);
                case SpawnCategory.Gem: return new Color(0.35f, 0.75f, 1f, 0.9f);
                case SpawnCategory.PowerUp: return new Color(0.2f, 0.85f, 1f, 0.9f);
                case SpawnCategory.Decoration: return new Color(0.35f, 0.9f, 0.4f, 0.9f);
                case SpawnCategory.Npc: return new Color(0.85f, 0.45f, 1f, 0.9f);
                default: return Color.white;
            }
        }
#endif
    }
}

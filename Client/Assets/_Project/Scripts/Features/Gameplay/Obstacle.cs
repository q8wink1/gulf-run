using GulfRun.Core.Pooling;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.9 — base obstacle behaviour. Owns collider, visual, type, and lane.
    /// No collision consequences, damage, or spawn execution. Pool-ready via <see cref="IPoolable"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider))]
    public abstract class Obstacle : MonoBehaviour, IObstaclePlacementTarget, IPoolable
    {
        private const float DefaultLaneSpacing = 2.2f;

        [Header("Data")]
        [SerializeField] private ObstacleData data;

        [Header("Placement (Inspector)")]
        [SerializeField] private RunnerLane lane = RunnerLane.Center;
        [SerializeField] private Vector3 placementEulerAngles;
        [SerializeField] private Vector3 placementScale = Vector3.one;
        [SerializeField] private bool obstacleEnabled = true;

        [Header("Lane Fit")]
        [Tooltip("Matches RunnerMovementConfig defaults so authored obstacles align with lanes.")]
        [SerializeField] private float laneSpacing = DefaultLaneSpacing;
        [SerializeField] private float laneCenterX;

        [Header("Components")]
        [SerializeField] private Collider obstacleCollider;
        [SerializeField] private Transform visualModel;

        public abstract ObstacleType Type { get; }

        public ObstacleData Data => data;
        public RunnerLane Lane => lane;
        public Collider ObstacleCollider => obstacleCollider;
        public Transform VisualModel => visualModel;
        public bool IsObstacleEnabled => obstacleEnabled;
        public Vector3 PlacementEulerAngles => placementEulerAngles;
        public Vector3 PlacementScale => placementScale;

        protected virtual void Awake()
        {
            EnsureComponents();
            ApplyInspectorPlacement(snapLaneX: true);
            ApplyEnabledState();
        }

        protected virtual void OnValidate()
        {
            EnsureComponents();
            if (placementScale.x < 0.05f || placementScale.y < 0.05f || placementScale.z < 0.05f)
            {
                placementScale = new Vector3(
                    Mathf.Max(0.05f, placementScale.x),
                    Mathf.Max(0.05f, placementScale.y),
                    Mathf.Max(0.05f, placementScale.z));
            }

            laneSpacing = Mathf.Max(0.1f, laneSpacing);
            ApplyInspectorPlacement(snapLaneX: true);
            ApplyEnabledState();
        }

        /// <summary>Assigns or swaps authoring data (type comes from subclass / data).</summary>
        public void BindData(ObstacleData obstacleData)
        {
            data = obstacleData;
        }

        public void SetLane(RunnerLane newLane)
        {
            lane = newLane;
            ApplyLaneX();
        }

        public void SetPlacementRotation(Vector3 eulerAngles)
        {
            placementEulerAngles = eulerAngles;
            transform.localRotation = Quaternion.Euler(placementEulerAngles);
        }

        public void SetPlacementScale(Vector3 scale)
        {
            placementScale = scale;
            transform.localScale = placementScale;
        }

        public void SetObstacleEnabled(bool enabled)
        {
            obstacleEnabled = enabled;
            ApplyEnabledState();
        }

        /// <summary>
        /// Configures this instance from a <see cref="SpawnManager"/> plan.
        /// Does not Instantiate — callers already own the instance (future pool Get).
        /// </summary>
        public void ApplyPlannedSlot(in PlannedSpawnSlot slot, RunnerLane plannedLane)
        {
            lane = plannedLane;
            transform.SetPositionAndRotation(slot.WorldPosition, slot.WorldRotation);
            ApplyLaneX();
            transform.localScale = placementScale;
            ApplyEnabledState();
        }

        /// <summary>Applies Inspector rotation / scale / lane X without touching world Z.</summary>
        public void ApplyInspectorPlacement(bool snapLaneX = true)
        {
            transform.localRotation = Quaternion.Euler(placementEulerAngles);
            transform.localScale = placementScale;
            if (snapLaneX)
            {
                ApplyLaneX();
            }
        }

        public virtual void OnSpawned()
        {
            ApplyEnabledState();
        }

        public virtual void OnDespawned()
        {
            // Keep components wired; pool deactivates the GameObject.
        }

        protected void EnsureComponents()
        {
            if (obstacleCollider == null)
            {
                obstacleCollider = GetComponent<Collider>();
            }

            if (visualModel == null)
            {
                Transform child = transform.Find("Visual");
                if (child != null)
                {
                    visualModel = child;
                }
            }
        }

        protected void ApplyLaneX()
        {
            Vector3 p = transform.position;
            p.x = RunnerLaneMath.LaneX(lane, laneCenterX, laneSpacing);
            transform.position = p;
        }

        protected void ApplyEnabledState()
        {
            if (obstacleCollider != null)
            {
                obstacleCollider.enabled = obstacleEnabled;
            }

            if (visualModel != null)
            {
                visualModel.gameObject.SetActive(obstacleEnabled);
            }
        }

        protected float ResolveWidth()
        {
            return data != null ? Mathf.Max(0.05f, data.Width) : 1.2f;
        }

        protected float ResolveHeight()
        {
            return data != null ? Mathf.Max(0.05f, data.Height) : 1.5f;
        }

#if UNITY_EDITOR
        protected virtual Color GizmoColor => new Color(1f, 0.35f, 0.2f, 0.85f);

        private void OnDrawGizmos()
        {
            DrawObstacleGizmo(selected: false);
        }

        private void OnDrawGizmosSelected()
        {
            DrawObstacleGizmo(selected: true);
            DrawExtraGizmosSelected();
        }

        /// <summary>Optional subclass overlays (motion path, etc.).</summary>
        protected virtual void DrawExtraGizmosSelected()
        {
        }

        private void DrawObstacleGizmo(bool selected)
        {
            float width = ResolveWidth();
            float height = ResolveHeight();
            Vector3 center = transform.position + new Vector3(0f, height * 0.5f, 0f);
            Vector3 size = Vector3.Scale(new Vector3(width, height, Mathf.Max(0.4f, width * 0.5f)), placementScale);

            Color color = GizmoColor;
            if (!obstacleEnabled)
            {
                color.a *= 0.35f;
            }

            if (selected)
            {
                color.a = Mathf.Min(1f, color.a + 0.15f);
            }

            Gizmos.color = color;
            Matrix4x4 prev = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
            if (selected)
            {
                Color fill = color;
                fill.a *= 0.15f;
                Gizmos.color = fill;
                Gizmos.DrawCube(Vector3.zero, size);
            }

            Gizmos.matrix = prev;

            // Lane tick on the ground.
            Gizmos.color = new Color(color.r, color.g, color.b, 0.9f);
            Vector3 lanePoint = transform.position;
            lanePoint.x = RunnerLaneMath.LaneX(lane, laneCenterX, laneSpacing);
            lanePoint.y = transform.position.y;
            Gizmos.DrawWireSphere(lanePoint, 0.2f);
        }
#endif
    }
}

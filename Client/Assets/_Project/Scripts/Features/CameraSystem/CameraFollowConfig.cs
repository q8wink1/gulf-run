using UnityEngine;

namespace GulfRun.Features.CameraSystem
{
    /// <summary>
    /// Designer-exposed tuning values for the side-scrolling follow camera.
    /// Bounds are opt-in (disabled by default) since level extents are not
    /// yet defined by any approved specification — enable and set once a
    /// map/level system provides real world-space limits.
    /// </summary>
    [CreateAssetMenu(
        fileName = "CameraFollowConfig",
        menuName = "GulfRun/Camera/Follow Config")]
    public sealed class CameraFollowConfig : ScriptableObject
    {
        [Header("Follow")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);
        [SerializeField] private float smoothTime = 0.15f;
        [SerializeField] private bool followY = true;

        [Header("Bounds (opt-in)")]
        [SerializeField] private bool useBoundsX;
        [SerializeField] private float minX;
        [SerializeField] private float maxX;
        [SerializeField] private bool useBoundsY;
        [SerializeField] private float minY;
        [SerializeField] private float maxY;

        public Vector3 Offset => offset;
        public float SmoothTime => smoothTime;
        public bool FollowY => followY;
        public bool UseBoundsX => useBoundsX;
        public float MinX => minX;
        public float MaxX => maxX;
        public bool UseBoundsY => useBoundsY;
        public float MinY => minY;
        public float MaxY => maxY;
    }
}

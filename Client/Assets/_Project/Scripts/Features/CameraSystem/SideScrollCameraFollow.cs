using UnityEngine;

namespace GulfRun.Features.CameraSystem
{
    /// <summary>
    /// Smooth side-scrolling follow camera with optional world-space bounds.
    /// Takes a plain <see cref="Transform"/> target rather than referencing
    /// the PlayerController feature directly (features must not reference
    /// other features — see FOLDER_ARCHITECTURE.md §4). If no target is
    /// assigned in the Inspector, it falls back to the built-in "Player" tag
    /// at runtime; a future multiplayer local-player spawner should assign
    /// <see cref="Target"/> explicitly instead of relying on the tag lookup.
    /// </summary>
    public sealed class SideScrollCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private CameraFollowConfig config;

        private Vector3 _velocity;

        public Transform Target
        {
            get => target;
            set => target = value;
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
        }

        private void LateUpdate()
        {
            if (target == null || config == null)
            {
                return;
            }

            Vector3 desired = target.position + config.Offset;
            if (!config.FollowY)
            {
                desired.y = transform.position.y;
            }

            if (config.UseBoundsX)
            {
                desired.x = Mathf.Clamp(desired.x, config.MinX, config.MaxX);
            }

            if (config.UseBoundsY)
            {
                desired.y = Mathf.Clamp(desired.y, config.MinY, config.MaxY);
            }

            transform.position = Vector3.SmoothDamp(
                transform.position,
                desired,
                ref _velocity,
                config.SmoothTime);
        }
    }
}

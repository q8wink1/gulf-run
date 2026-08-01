using UnityEngine;

namespace GulfRun.Core.Platforms
{
    /// <summary>
    /// Reference implementation of <see cref="IMovingPlatform"/>: ping-pongs
    /// between the spawn position and a configurable local offset. Not yet
    /// wired into any World Generation spawn category (no platform art
    /// exists this sprint) — drop this component onto any Collider2D on the
    /// player's configured ground layer to make it a rideable moving
    /// platform with zero PlayerController changes required.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MovingPlatform : MonoBehaviour, IMovingPlatform
    {
        [Tooltip("Local-space offset (relative to the start position) the platform travels to and from.")]
        [SerializeField] private Vector3 travelOffset = new Vector3(3f, 0f, 0f);

        [Tooltip("Full round-trip cycles per second.")]
        [SerializeField] private float cyclesPerSecond = 0.2f;

        private Vector3 _startPosition;

        public Vector2 FrameDelta { get; private set; }

        private void Awake()
        {
            _startPosition = transform.position;
        }

        private void Update()
        {
            Vector3 previousPosition = transform.position;

            float travelDistance = travelOffset.magnitude;
            float t = travelDistance > 0f
                ? Mathf.PingPong(Time.time * cyclesPerSecond * 2f, 1f)
                : 0f;

            Vector3 nextPosition = _startPosition + travelOffset * t;
            transform.position = nextPosition;

            FrameDelta = nextPosition - previousPosition;
        }
    }
}

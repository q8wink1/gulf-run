using GulfRun.Core.Services;
using UnityEngine;

namespace GulfRun.Features.Maps.Background
{
    /// <summary>
    /// Generic, reusable scrolling background layer (Sprint 12
    /// "BACKGROUND": clouds/birds/palm trees/flags/traffic/sea waves are
    /// all, mechanically, "a sprite that drifts left at some fraction of
    /// world scroll speed and wraps"). Deliberately has zero per-element
    /// special-casing — every animated background element is just another
    /// ParallaxLayer with a different <see cref="parallaxFactor"/>/
    /// <see cref="wrapDistanceMeters"/>, satisfying the brief's "reusable
    /// level components" Code Quality requirement. Reads world scroll speed
    /// from the existing <see cref="RunSpeedService"/> seam (the same one
    /// the Player already consumes) so it needs zero coupling to
    /// Features.EndlessRunner; falls back to <see cref="fallbackScrollSpeed"/>
    /// so this component still runs/demos correctly even before that
    /// provider is registered in a given scene.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ParallaxLayer : MonoBehaviour
    {
        [Tooltip("0 = fixed/stationary (e.g. a distant skyline), 1 = moves at full world scroll speed.")]
        [Range(0f, 1f)]
        [SerializeField] private float parallaxFactor = 0.2f;

        [SerializeField] private float fallbackScrollSpeed = 2f;

        [Tooltip("World units after which this layer wraps back to its start X, for a seamless infinite scroll. 0 = never wraps.")]
        [SerializeField] private float wrapDistanceMeters;

        private Vector3 _startPosition;

        private void Awake()
        {
            _startPosition = transform.localPosition;
        }

        private void Update()
        {
            float speed = RunSpeedService.Current != null ? RunSpeedService.Current.CurrentSpeed : fallbackScrollSpeed;

            Vector3 position = transform.localPosition;
            position.x -= speed * parallaxFactor * Time.deltaTime;

            if (wrapDistanceMeters > 0f && position.x <= _startPosition.x - wrapDistanceMeters)
            {
                position.x += wrapDistanceMeters;
            }

            transform.localPosition = position;
        }
    }
}

using UnityEngine;

namespace GulfRun.Features.CameraSystem
{
    /// <summary>
    /// Sprint 23.5 — reusable camera shake. Public API only; nothing triggers
    /// it yet (reserved for future collision / landing / power-up / explosion).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CameraShake : MonoBehaviour
    {
        [SerializeField] private float defaultIntensity = 0.25f;
        [SerializeField] private float defaultDuration = 0.2f;

        private float _remaining;
        private float _duration;
        private float _intensity;
        private Vector3 _offset;

        public Vector3 CurrentOffset => _offset;
        public bool IsShaking => _remaining > 0f;

        public void Shake(float intensity, float duration)
        {
            _intensity = Mathf.Max(0f, intensity);
            _duration = Mathf.Max(0.01f, duration);
            _remaining = _duration;
        }

        /// <summary>Convenience overload using Inspector defaults.</summary>
        public void Shake() => Shake(defaultIntensity, defaultDuration);

        private void LateUpdate()
        {
            if (_remaining <= 0f)
            {
                _offset = Vector3.zero;
                return;
            }

            _remaining -= Time.deltaTime;
            float t = Mathf.Clamp01(_remaining / _duration);
            float magnitude = _intensity * t;
            // Deterministic-ish shake without Random allocations.
            float time = Time.unscaledTime;
            _offset = new Vector3(
                Mathf.Sin(time * 37.1f) * magnitude,
                Mathf.Sin(time * 53.7f) * magnitude * 0.65f,
                Mathf.Sin(time * 29.3f) * magnitude * 0.35f);

            if (_remaining <= 0f)
            {
                _offset = Vector3.zero;
            }
        }
    }
}

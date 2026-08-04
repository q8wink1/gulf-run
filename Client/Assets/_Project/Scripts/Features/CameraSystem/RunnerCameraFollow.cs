using UnityEngine;

namespace GulfRun.Features.CameraSystem
{
    /// <summary>
    /// Sprint 23.5 — premium mobile runner follow camera.
    /// LateUpdate SmoothDamp position + Slerp look-at with locked roll.
    /// Separate vertical damp for jumps; slide should not yank the camera.
    /// Takes a plain Transform (no Feature→Feature reference to Gameplay).
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class RunnerCameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Follow")]
        [SerializeField] private float followSpeed = 10f;
        [SerializeField] private float rotationSpeed = 8f;
        [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 4.5f, -8.5f);
        [SerializeField] private float verticalOffset;
        [SerializeField] private float horizontalOffset;
        [SerializeField] private float smoothTime = 0.12f;
        [Tooltip("Extra SmoothDamp time applied to Y so jumps feel gentle.")]
        [SerializeField] private float verticalSmoothTime = 0.28f;
        [Tooltip("While target Y is near ground, keep Y more stable (slide / run).")]
        [SerializeField] private float groundedVerticalSmoothTime = 0.4f;
        [SerializeField] private float groundedYThreshold = 0.35f;
        [SerializeField] private float lookAtHeight = 1.2f;

        [Header("Lens")]
        [SerializeField] private float fieldOfView = 60f;

        private Camera _camera;
        private CameraShake _shake;
        private Vector3 _velocity;
        private float _yVelocity;
        private float _currentY;
        private bool _initialized;

        public Transform Target
        {
            get => target;
            set => target = value;
        }

        public float FollowSpeed { get => followSpeed; set => followSpeed = value; }
        public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
        public Vector3 CameraOffset { get => cameraOffset; set => cameraOffset = value; }
        public float VerticalOffset { get => verticalOffset; set => verticalOffset = value; }
        public float HorizontalOffset { get => horizontalOffset; set => horizontalOffset = value; }
        public float SmoothTime { get => smoothTime; set => smoothTime = Mathf.Max(0.01f, value); }
        public float FieldOfView { get => fieldOfView; set => fieldOfView = value; }

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _shake = GetComponent<CameraShake>();
            ApplyFov();
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

            if (target != null)
            {
                SnapToTarget();
            }
        }

        private void OnValidate()
        {
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
            }

            ApplyFov();
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            if (!_initialized)
            {
                SnapToTarget();
            }

            ApplyFov();

            Vector3 desired = ResolveDesiredPosition();
            float posSmooth = Mathf.Max(0.01f, smoothTime);
            if (followSpeed > 0.01f)
            {
                // followSpeed acts as a designer-friendly scale on SmoothDamp responsiveness.
                posSmooth = Mathf.Max(0.01f, smoothTime * (10f / followSpeed));
            }

            float targetY = desired.y;
            float ySmooth = ResolveVerticalSmooth();
            _currentY = Mathf.SmoothDamp(_currentY, targetY, ref _yVelocity, ySmooth);

            Vector3 flatDesired = desired;
            flatDesired.y = transform.position.y;
            Vector3 next = Vector3.SmoothDamp(transform.position, flatDesired, ref _velocity, posSmooth);
            next.y = _currentY;

            if (_shake != null)
            {
                next += _shake.CurrentOffset;
            }

            transform.position = next;

            Vector3 lookPoint = target.position + new Vector3(0f, lookAtHeight, 0f);
            Vector3 toLook = lookPoint - transform.position;
            if (toLook.sqrMagnitude > 0.0001f)
            {
                Quaternion desiredRot = Quaternion.LookRotation(toLook.normalized, Vector3.up);
                // Lock roll — flatten any unintended bank.
                Vector3 euler = desiredRot.eulerAngles;
                desiredRot = Quaternion.Euler(euler.x, euler.y, 0f);
                float rotT = 1f - Mathf.Exp(-rotationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotT);
            }
        }

        private Vector3 ResolveDesiredPosition()
        {
            Vector3 offset = cameraOffset;
            offset.x += horizontalOffset;
            offset.y += verticalOffset;
            return target.position + offset;
        }

        private float ResolveVerticalSmooth()
        {
            // Prefer stable Y while near ground (run / slide); softer catch-up in air (jump).
            float heightAbove = target.position.y;
            if (heightAbove <= groundedYThreshold)
            {
                return Mathf.Max(0.01f, groundedVerticalSmoothTime);
            }

            return Mathf.Max(0.01f, verticalSmoothTime);
        }

        private void SnapToTarget()
        {
            Vector3 desired = ResolveDesiredPosition();
            transform.position = desired;
            _currentY = desired.y;
            _velocity = Vector3.zero;
            _yVelocity = 0f;
            _initialized = true;

            Vector3 lookPoint = target.position + new Vector3(0f, lookAtHeight, 0f);
            Vector3 toLook = lookPoint - transform.position;
            if (toLook.sqrMagnitude > 0.0001f)
            {
                Quaternion rot = Quaternion.LookRotation(toLook.normalized, Vector3.up);
                Vector3 euler = rot.eulerAngles;
                transform.rotation = Quaternion.Euler(euler.x, euler.y, 0f);
            }
        }

        private void ApplyFov()
        {
            if (_camera != null)
            {
                _camera.fieldOfView = fieldOfView;
            }
        }
    }
}

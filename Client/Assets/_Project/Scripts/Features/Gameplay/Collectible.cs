using System;
using System.Collections;
using GulfRun.Core.Pooling;
using GulfRun.Domain;
using GulfRun.Features.GameplayHud;
using UnityEngine;
using UnityEngine.Events;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.12 — pooled collectible base. Rotates, detects player via trigger
    /// or collection radius, plays a short collect animation, updates HUD, then
    /// returns to <see cref="ObjectPoolManager"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SphereCollider))]
    public abstract class Collectible : MonoBehaviour, IPoolable
    {
        private const float DefaultLaneSpacing = 2.2f;
        private const float CollectAnimDuration = 0.18f;

        [Header("Values (Inspector)")]
        [SerializeField] private int coinValue = 1;
        [SerializeField] private int gemValue = 1;

        [Header("Motion / Pickup")]
        [SerializeField] private float rotationSpeed = 120f;
        [SerializeField] private float collectionRadius = 0.85f;

        [Header("Lane Fit")]
        [SerializeField] private float laneSpacing = DefaultLaneSpacing;
        [SerializeField] private float laneCenterX;

        [Header("Components")]
        [SerializeField] private SphereCollider collectibleCollider;
        [SerializeField] private Transform visualModel;

        [Header("Feedback (placeholders)")]
        [SerializeField] private UnityEvent onCollectedSound;

        private RunnerLane _lane = RunnerLane.Center;
        private bool _collected;
        private bool _animating;
        private Vector3 _visualBaseScale = Vector3.one;
        private Coroutine _collectRoutine;

        /// <summary>Raised when any collectible completes a pickup credit.</summary>
        public static event Action<Collectible, int> AnyCollected;

        public abstract CollectibleType Type { get; }

        public int CoinValue => coinValue < 0 ? 0 : coinValue;
        public int GemValue => gemValue < 0 ? 0 : gemValue;
        public float RotationSpeed => rotationSpeed;
        public float CollectionRadius => collectionRadius;
        public RunnerLane Lane => _lane;
        public bool IsCollected => _collected;

        /// <summary>Credit amount for this instance's type.</summary>
        public int ResolveValue()
        {
            return Type == CollectibleType.Gem ? GemValue : CoinValue;
        }

        protected virtual void Awake()
        {
            EnsureComponents();
            ApplyCollectionRadius();
            CacheVisualScale();
        }

        protected virtual void OnValidate()
        {
            coinValue = Mathf.Max(0, coinValue);
            gemValue = Mathf.Max(0, gemValue);
            rotationSpeed = Mathf.Max(0f, rotationSpeed);
            collectionRadius = Mathf.Max(0.1f, collectionRadius);
            laneSpacing = Mathf.Max(0.1f, laneSpacing);
            EnsureComponents();
            ApplyCollectionRadius();
        }

        private void Update()
        {
            if (_collected || visualModel == null || rotationSpeed <= 0f)
            {
                return;
            }

            visualModel.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
        }

        /// <summary>
        /// Places this instance from a <see cref="SpawnManager"/> plan.
        /// Caller already owns the pooled instance.
        /// </summary>
        public void ApplyPlannedSlot(in PlannedSpawnSlot slot, RunnerLane plannedLane)
        {
            _lane = plannedLane;
            transform.SetPositionAndRotation(slot.WorldPosition, slot.WorldRotation);
            ApplyLaneX();
            _collected = false;
            _animating = false;
            ApplyCollectionRadius();
            ResetVisual();
            if (collectibleCollider != null)
            {
                collectibleCollider.enabled = true;
            }
        }

        public void ApplyWorldPose(Vector3 worldPosition, Quaternion worldRotation, RunnerLane plannedLane)
        {
            _lane = plannedLane;
            transform.SetPositionAndRotation(worldPosition, worldRotation);
            ApplyLaneX();
            _collected = false;
            _animating = false;
            ApplyCollectionRadius();
            ResetVisual();
            if (collectibleCollider != null)
            {
                collectibleCollider.enabled = true;
            }
        }

        public virtual void OnSpawned()
        {
            _collected = false;
            _animating = false;
            ApplyCollectionRadius();
            ResetVisual();
            if (collectibleCollider != null)
            {
                collectibleCollider.enabled = true;
            }
        }

        public virtual void OnDespawned()
        {
            if (_collectRoutine != null)
            {
                StopCoroutine(_collectRoutine);
                _collectRoutine = null;
            }

            _collected = false;
            _animating = false;
            ResetVisual();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            TryCollectFromCollider(other);
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            // Radius may exceed collider if designer widens collectionRadius at runtime.
            TryCollectFromCollider(other);
        }

        private void TryCollectFromCollider(Collider other)
        {
            if (_collected || _animating || other == null)
            {
                return;
            }

            RunnerPlayerController player = other.GetComponentInParent<RunnerPlayerController>();
            if (player == null && other.CompareTag("Player"))
            {
                player = other.GetComponent<RunnerPlayerController>();
            }

            if (player == null)
            {
                return;
            }

            float radius = collectionRadius > 0.1f ? collectionRadius : 0.85f;
            Vector3 playerPoint = other.ClosestPoint(transform.position);
            if ((playerPoint - transform.position).sqrMagnitude > radius * radius)
            {
                return;
            }

            BeginCollect();
        }

        private void BeginCollect()
        {
            if (_collected || _animating)
            {
                return;
            }

            _collected = true;
            _animating = true;
            if (collectibleCollider != null)
            {
                collectibleCollider.enabled = false;
            }

            int value = ResolveValue();
            CreditHud(value);
            onCollectedSound?.Invoke();
            AnyCollected?.Invoke(this, value);

            if (_collectRoutine != null)
            {
                StopCoroutine(_collectRoutine);
            }

            _collectRoutine = StartCoroutine(CollectAndRelease());
        }

        private void CreditHud(int value)
        {
            if (value <= 0)
            {
                return;
            }

            GameplayHudController hud = GameplayHudController.Instance;
            if (hud == null)
            {
                return;
            }

            if (Type == CollectibleType.Gem)
            {
                hud.AddGems(value);
            }
            else
            {
                hud.AddCoins(value);
            }
        }

        private IEnumerator CollectAndRelease()
        {
            Transform target = visualModel != null ? visualModel : transform;
            Vector3 startScale = target.localScale;
            Vector3 startPos = target.localPosition;
            float elapsed = 0f;

            while (elapsed < CollectAnimDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / CollectAnimDuration);
                float eased = t * t * (3f - (2f * t));
                target.localScale = Vector3.Lerp(startScale, Vector3.zero, eased);
                if (visualModel != null)
                {
                    target.localPosition = startPos + new Vector3(0f, 0.35f * eased, 0f);
                }

                yield return null;
            }

            _collectRoutine = null;
            _animating = false;

            ObjectPoolManager pools = ObjectPoolManager.Instance;
            if (pools != null)
            {
                pools.Release(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void EnsureComponents()
        {
            if (collectibleCollider == null)
            {
                collectibleCollider = GetComponent<SphereCollider>();
            }

            if (collectibleCollider != null)
            {
                collectibleCollider.isTrigger = true;
            }

            if (visualModel == null)
            {
                Transform child = transform.Find("Visual");
                if (child != null)
                {
                    visualModel = child;
                }
            }

            CacheVisualScale();
        }

        private void ApplyCollectionRadius()
        {
            if (collectibleCollider == null)
            {
                return;
            }

            collectibleCollider.isTrigger = true;
            collectibleCollider.radius = Mathf.Max(0.1f, collectionRadius);
            collectibleCollider.center = new Vector3(0f, collectionRadius * 0.15f, 0f);
        }

        private void ApplyLaneX()
        {
            Vector3 p = transform.position;
            p.x = RunnerLaneMath.LaneX(_lane, laneCenterX, laneSpacing);
            transform.position = p;
        }

        private void CacheVisualScale()
        {
            if (visualModel != null)
            {
                _visualBaseScale = visualModel.localScale;
                if (_visualBaseScale.sqrMagnitude < 0.0001f)
                {
                    _visualBaseScale = Vector3.one;
                }
            }
        }

        private void ResetVisual()
        {
            if (visualModel == null)
            {
                return;
            }

            visualModel.localScale = _visualBaseScale;
            visualModel.localPosition = new Vector3(0f, 0.35f, 0f);
            visualModel.localRotation = Quaternion.identity;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Type == CollectibleType.Gem
                ? new Color(0.35f, 0.75f, 1f, 0.85f)
                : new Color(1f, 0.85f, 0.2f, 0.85f);
            Gizmos.DrawWireSphere(transform.position, Mathf.Max(0.1f, collectionRadius));
        }
#endif
    }
}

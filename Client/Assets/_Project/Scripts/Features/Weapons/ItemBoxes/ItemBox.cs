using System.Collections;
using GulfRun.Core.Managers;
using GulfRun.Core.Networking;
using GulfRun.Core.Pooling;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Weapons.ItemBoxes
{
    /// <summary>
    /// A single Mystery Item Box instance. Spawned/pooled/recycled entirely
    /// by the existing Sprint 3 pipeline (<c>ChunkContentSpawner</c> +
    /// <c>ObjectPoolManager</c>, via the new <see cref="SpawnCategory.ItemBox"/>
    /// category) — this component only knows how to react to being touched.
    ///
    /// Uses the pooled GameObject's own instance id as its wire-level
    /// <c>BoxId</c>, so no separate id-allocator system is needed. On touch
    /// it immediately disables its own collider (so one player can't trigger
    /// it twice while the pickup round-trip is in flight) and asks the
    /// authority to resolve the pickup — it never grants itself a weapon.
    /// Despawns itself back to the pool after the opening animation window,
    /// regardless of whether a weapon was actually granted ("the Item Box is
    /// lost" if the collector's inventory was full).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class ItemBox : MonoBehaviour, IPoolable
    {
        [SerializeField] private float openingAnimationSeconds = 0.6f;
        [SerializeField] private AudioClip openSound;
        [SerializeField] private Animator animator;
        [SerializeField] private string openingAnimatorTrigger = "Open";

        private Collider2D _collider;
        private Coroutine _despawnRoutine;
        private bool _claimed;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        public void OnSpawned()
        {
            _claimed = false;
            if (_collider != null)
            {
                _collider.enabled = true;
            }
        }

        public void OnDespawned()
        {
            if (_despawnRoutine != null)
            {
                StopCoroutine(_despawnRoutine);
                _despawnRoutine = null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_claimed || !other.CompareTag("Player"))
            {
                return;
            }

            _claimed = true;
            if (_collider != null)
            {
                _collider.enabled = false;
            }

            if (animator != null && !string.IsNullOrEmpty(openingAnimatorTrigger))
            {
                animator.SetTrigger(openingAnimatorTrigger);
            }

            AudioManager.Instance?.PlayOneShot(openSound);

            IMatchTransport transport = MatchTransportService.Current;
            int collectorConnectionId = transport != null ? transport.LocalConnectionId : -1;
            transport?.RequestWeaponPickup(new WeaponPickupRequest(gameObject.GetInstanceID(), collectorConnectionId, Time.timeAsDouble));

            _despawnRoutine = StartCoroutine(DespawnAfterOpening());
        }

        private IEnumerator DespawnAfterOpening()
        {
            yield return new WaitForSeconds(openingAnimationSeconds);
            ObjectPoolManager.Instance?.Release(gameObject);
        }
    }
}

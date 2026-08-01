using GulfRun.Core.Pooling;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.Spawning
{
    /// <summary>
    /// Placeholder power-up pickup. No power-up types or gameplay effects are
    /// defined by any approved specification yet, so this only proves the
    /// spawn category end-to-end (detect pickup, release back to pool).
    /// TODO(Design): wire real effects here once power-up types are
    /// specified, and route any Boost-style effect through
    /// GameSpeedController.ApplyTemporaryModifier for consistency.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class PowerUpPickup : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            ObjectPoolManager.Instance.Release(gameObject);
        }
    }
}

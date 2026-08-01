using GulfRun.Core.Pooling;
using GulfRun.Features.EndlessRunner.Scoring;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.Spawning
{
    /// <summary>
    /// Placeholder coin pickup: on contact with the Player, adds to the
    /// score's coin count and releases itself back to the pool. Detects the
    /// player purely by tag so this component never references the
    /// PlayerController feature.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class CoinPickup : MonoBehaviour
    {
        [SerializeField] private int value = 1;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            if (ScoreController.Instance != null)
            {
                ScoreController.Instance.AddCoins(value);
            }

            ObjectPoolManager.Instance.Release(gameObject);
        }
    }
}

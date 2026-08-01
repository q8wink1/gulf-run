using GulfRun.Features.EndlessRunner.GameLoop;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.Spawning
{
    /// <summary>
    /// Placeholder obstacle behaviour: on contact with the Player, ends the
    /// run. No health/shield/lives system is specified by any approved
    /// document, so "touch obstacle -> Game Over" is the minimal, standard
    /// genre-convention rule wiring the Obstacle spawn category to the Game
    /// Loop's Game Over state. Detects the player purely by tag so this
    /// component never references the PlayerController feature.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class ObstacleContact : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            if (GameLoopController.Instance != null)
            {
                GameLoopController.Instance.RequestGameOver();
            }
        }
    }
}

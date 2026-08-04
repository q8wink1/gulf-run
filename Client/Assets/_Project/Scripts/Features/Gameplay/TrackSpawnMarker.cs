using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.6 — fixed placeholder slot on a <see cref="TrackSegment"/> for a
    /// future spawnable (obstacle / coin / gem / power-up / decoration / NPC).
    /// Markers are authoring data only; nothing is spawned here yet.
    /// </summary>
    public sealed class TrackSpawnMarker : MonoBehaviour
    {
        [SerializeField] private SpawnCategory category = SpawnCategory.Obstacle;

        public SpawnCategory Category => category;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = CategoryColor(category);
            Gizmos.DrawWireSphere(transform.position, 0.35f);
        }

        private static Color CategoryColor(SpawnCategory kind)
        {
            switch (kind)
            {
                case SpawnCategory.Obstacle: return new Color(1f, 0.25f, 0.25f, 0.9f);
                case SpawnCategory.Coin: return new Color(1f, 0.85f, 0.2f, 0.9f);
                case SpawnCategory.Gem: return new Color(0.35f, 0.75f, 1f, 0.9f);
                case SpawnCategory.PowerUp: return new Color(0.2f, 0.85f, 1f, 0.9f);
                case SpawnCategory.Decoration: return new Color(0.35f, 0.9f, 0.4f, 0.9f);
                case SpawnCategory.Npc: return new Color(0.85f, 0.45f, 1f, 0.9f);
                default: return Color.white;
            }
        }
#endif
    }
}

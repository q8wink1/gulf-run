using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.WorldGeneration
{
    /// <summary>
    /// Marks a fixed slot within a chunk prefab where content of a specific
    /// <see cref="SpawnCategory"/> may be spawned. Purely data — placement
    /// and category are decided by whoever authors the chunk prefab.
    /// </summary>
    public sealed class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private SpawnCategory category;

        public SpawnCategory Category => category;
    }
}

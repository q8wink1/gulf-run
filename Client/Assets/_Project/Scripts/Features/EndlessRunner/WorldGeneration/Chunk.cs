using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.WorldGeneration
{
    /// <summary>
    /// A single pooled level segment. The chunk's own root position marks its
    /// left/start edge; content spans forward (+X) for <see cref="Length"/>
    /// world units. <see cref="BiomeId"/> is metadata only today, ready for a
    /// future biome system to filter/tag chunk prefab variants without any
    /// change to <see cref="WorldGenerator"/>. <see cref="SectionType"/> is
    /// the Sprint 12 "Level Structure" tag (Flat/Hill/Slope/Bridge/
    /// Platform/Tunnel/OpenArea/Drop/Climb) carried purely for
    /// variety/telemetry/debug purposes — every variant still shares the
    /// exact same proven-safe continuous ground collider footprint as the
    /// original Default chunk, so chunk-to-chunk transitions can never
    /// unfairly gap or block the race regardless of section type.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Chunk : MonoBehaviour
    {
        [SerializeField] private float length = 20f;
        [SerializeField] private string biomeId = "Default";
        [SerializeField] private LevelSectionType sectionType = LevelSectionType.FlatSection;
        [SerializeField] private SpawnPoint[] spawnPoints;

        /// <summary>Content instances spawned into this chunk by <see cref="Spawning.ChunkContentSpawner"/>, tracked so they can be released when the chunk is recycled.</summary>
        public List<GameObject> SpawnedContent { get; } = new List<GameObject>();

        public float Length => length;
        public string BiomeId => biomeId;
        public LevelSectionType SectionType => sectionType;
        public IReadOnlyList<SpawnPoint> SpawnPoints => spawnPoints;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                spawnPoints = GetComponentsInChildren<SpawnPoint>(includeInactive: true);
            }
        }
#endif
    }
}

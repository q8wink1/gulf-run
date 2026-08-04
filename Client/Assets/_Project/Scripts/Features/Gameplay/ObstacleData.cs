using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.9 — tunable obstacle definition (no spawn / damage logic).
    /// Referenced by <see cref="Obstacle"/> instances and <see cref="ObstacleCatalog"/>.
    /// </summary>
    [CreateAssetMenu(
        fileName = "ObstacleData",
        menuName = "GulfRun/Gameplay/Obstacle Data")]
    public sealed class ObstacleData : ScriptableObject
    {
        [Tooltip("Designer-facing name (e.g. Static Barrier, Low Beam).")]
        [SerializeField] private string displayName = "Obstacle";

        [SerializeField] private ObstacleType obstacleType = ObstacleType.Static;

        [Tooltip("World-space footprint width used for gizmos / future fit checks.")]
        [SerializeField] private float width = 1.2f;

        [Tooltip("World-space height used for gizmos / future jump-clear checks.")]
        [SerializeField] private float height = 1.5f;

        [Tooltip("Relative challenge tier (1 = easy, 5 = hard).")]
        [Range(1, 5)]
        [SerializeField] private int difficulty = 1;

        [Tooltip("Relative pick weight for future weighted spawn selection.")]
        [SerializeField] private float spawnWeight = 1f;

        [Tooltip("Action the runner is expected to perform — consequences not wired yet.")]
        [SerializeField] private ObstacleRequiredAction requiredAction = ObstacleRequiredAction.None;

        public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;
        public ObstacleType ObstacleType => obstacleType;
        public float Width => width;
        public float Height => height;
        public int Difficulty => difficulty;
        public float SpawnWeight => spawnWeight;
        public ObstacleRequiredAction RequiredAction => requiredAction;

#if UNITY_EDITOR
        private void OnValidate()
        {
            width = Mathf.Max(0.05f, width);
            height = Mathf.Max(0.05f, height);
            difficulty = Mathf.Clamp(difficulty, 1, 5);
            spawnWeight = Mathf.Max(0f, spawnWeight);
            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = name;
            }
        }
#endif
    }
}

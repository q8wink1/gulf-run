using UnityEngine;

namespace GulfRun.Features.EndlessRunner.Configuration
{
    /// <summary>Tuning values for the shared 0..1 difficulty ramp.</summary>
    [CreateAssetMenu(
        fileName = "DifficultyConfig",
        menuName = "GulfRun/EndlessRunner/Difficulty Config")]
    public sealed class DifficultyConfig : ScriptableObject
    {
        [SerializeField] private float rampStartMeters;
        [SerializeField] private float rampEndMeters = 2000f;

        public float RampStartMeters => rampStartMeters;
        public float RampEndMeters => rampEndMeters;
    }
}

using UnityEngine;

namespace GulfRun.Features.EndlessRunner.Configuration
{
    /// <summary>
    /// Tuning for the automatic race-start countdown. No values are
    /// hardcoded into <see cref="GameLoop.CountdownController"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "CountdownConfig", menuName = "GulfRun/EndlessRunner/Countdown Config")]
    public sealed class CountdownConfig : ScriptableObject
    {
        [Tooltip("Total countdown length in seconds, counted down as whole seconds (3, 2, 1) before GO.")]
        [SerializeField] private float durationSeconds = 3f;

        public float DurationSeconds => durationSeconds;
    }
}

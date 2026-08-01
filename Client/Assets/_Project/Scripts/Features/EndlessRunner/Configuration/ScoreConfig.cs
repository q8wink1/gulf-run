using UnityEngine;

namespace GulfRun.Features.EndlessRunner.Configuration
{
    /// <summary>Tuning values for the scoring system.</summary>
    [CreateAssetMenu(
        fileName = "ScoreConfig",
        menuName = "GulfRun/EndlessRunner/Score Config")]
    public sealed class ScoreConfig : ScriptableObject
    {
        [SerializeField] private float distanceScorePerMeter = 1f;
        [SerializeField] private float coinScoreValue = 10f;
        [SerializeField] private float baseMultiplier = 1f;

        public float DistanceScorePerMeter => distanceScorePerMeter;
        public float CoinScoreValue => coinScoreValue;
        public float BaseMultiplier => baseMultiplier;
    }
}

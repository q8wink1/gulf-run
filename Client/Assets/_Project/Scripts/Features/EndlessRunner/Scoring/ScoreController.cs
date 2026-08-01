using GulfRun.Core;
using GulfRun.Domain;
using GulfRun.Features.EndlessRunner.Configuration;
using GulfRun.Features.EndlessRunner.Distance;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.Scoring
{
    /// <summary>
    /// Tracks the current run's score: distance score + coin score, scaled by
    /// a multiplier, recomputed from <see cref="Domain.ScoreCalculator"/>
    /// every tick. The multiplier is already first-class so a future combo
    /// system only has to call <see cref="SetMultiplier"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ScoreController : SceneSingleton<ScoreController>
    {
        [SerializeField] private ScoreConfig config;

        private DistanceTracker _distanceTracker;

        public int CoinsCollected { get; private set; }
        public float Multiplier { get; private set; } = 1f;
        public float DistanceScore { get; private set; }
        public float CoinScore { get; private set; }
        public float TotalScore { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            _distanceTracker = GetComponent<DistanceTracker>();
            Multiplier = config.BaseMultiplier;
        }

        /// <summary>Recomputes score from the current distance/coins. Called only while the game loop is Running.</summary>
        public void Tick()
        {
            ScoreBreakdown breakdown = ScoreCalculator.Calculate(
                _distanceTracker.DistanceMeters,
                CoinsCollected,
                config.DistanceScorePerMeter,
                config.CoinScoreValue,
                Multiplier);

            DistanceScore = breakdown.DistanceScore;
            CoinScore = breakdown.CoinScore;
            TotalScore = breakdown.TotalScore;
        }

        public void AddCoins(int amount)
        {
            if (amount > 0)
            {
                CoinsCollected += amount;
            }
        }

        /// <summary>Sets the active score multiplier (future combo-system hook).</summary>
        public void SetMultiplier(float multiplier)
        {
            Multiplier = multiplier <= 0f ? 1f : multiplier;
        }

        /// <summary>Resets score state. Called by the game loop on Restart.</summary>
        public void ResetScore()
        {
            CoinsCollected = 0;
            Multiplier = config.BaseMultiplier;
            DistanceScore = 0f;
            CoinScore = 0f;
            TotalScore = 0f;
        }
    }
}

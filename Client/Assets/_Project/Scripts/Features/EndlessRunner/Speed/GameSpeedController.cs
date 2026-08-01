using GulfRun.Core;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.EndlessRunner.Configuration;
using GulfRun.Features.EndlessRunner.Distance;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.Speed
{
    /// <summary>
    /// Global Game Speed Controller: base speed, a progressive increase up to
    /// a maximum, and temporary modifiers (e.g. a future Boost pickup).
    /// Publishes itself through <see cref="RunSpeedService"/> so the Player
    /// (a different feature) can consume the live speed without either
    /// feature referencing the other.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GameSpeedController : SceneSingleton<GameSpeedController>, IRunSpeedProvider
    {
        [SerializeField] private GameSpeedConfig config;

        private DistanceTracker _distanceTracker;
        private float _temporaryModifier = 1f;
        private float _temporaryModifierTimeRemaining;

        public float CurrentSpeed { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            _distanceTracker = GetComponent<DistanceTracker>();
        }

        private void OnEnable()
        {
            RunSpeedService.Current = this;
        }

        private void OnDisable()
        {
            if (ReferenceEquals(RunSpeedService.Current, this))
            {
                RunSpeedService.Current = null;
            }
        }

        /// <summary>Advances the speed simulation by one frame. Called only while the game loop is Running.</summary>
        public void Tick(float deltaTime, double distanceMeters)
        {
            float baseline = SpeedCurve.Evaluate(distanceMeters, config.BaseSpeed, config.MaxSpeed, config.RampDistanceMeters);

            if (_temporaryModifierTimeRemaining > 0f)
            {
                _temporaryModifierTimeRemaining -= deltaTime;
                if (_temporaryModifierTimeRemaining <= 0f)
                {
                    _temporaryModifier = 1f;
                }
            }

            float boostedCap = config.MaxSpeed * Mathf.Max(_temporaryModifier, 1f);
            CurrentSpeed = Mathf.Min(baseline * _temporaryModifier, boostedCap);
        }

        /// <summary>
        /// Applies a temporary speed multiplier for a duration (future Boost
        /// System hook). A multiplier below 1 slows the run down; above 1
        /// speeds it up, intentionally allowed to exceed the normal max speed.
        /// </summary>
        public void ApplyTemporaryModifier(float multiplier, float durationSeconds)
        {
            _temporaryModifier = multiplier <= 0f ? 1f : multiplier;
            _temporaryModifierTimeRemaining = durationSeconds;
        }

        /// <summary>Resets speed state. Called by the game loop on Restart.</summary>
        public void ResetSpeed()
        {
            _temporaryModifier = 1f;
            _temporaryModifierTimeRemaining = 0f;
            CurrentSpeed = 0f;
        }
    }
}

using GulfRun.Core;
using GulfRun.Features.EndlessRunner.Configuration;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.Difficulty
{
    /// <summary>
    /// Computes the single shared 0..1 difficulty value once per tick, from
    /// distance traveled. Both the Game Speed Controller and the spawn system
    /// read this same value (each applying it to their own domain
    /// differently), so "how far into the difficulty ramp are we" is
    /// calculated in exactly one place.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DifficultyController : SceneSingleton<DifficultyController>
    {
        [SerializeField] private DifficultyConfig config;

        public float Current01 { get; private set; }

        /// <summary>Recomputes difficulty from the current distance. Called only while the game loop is Running.</summary>
        public void Tick(double distanceMeters)
        {
            Current01 = Domain.DifficultyCurve.Evaluate(distanceMeters, config.RampStartMeters, config.RampEndMeters);
        }

        /// <summary>Resets difficulty to zero. Called by the game loop on Restart.</summary>
        public void ResetDifficulty()
        {
            Current01 = 0f;
        }
    }
}

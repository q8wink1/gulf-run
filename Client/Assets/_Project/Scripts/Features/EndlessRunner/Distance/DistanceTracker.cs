using GulfRun.Core;

namespace GulfRun.Features.EndlessRunner.Distance
{
    /// <summary>
    /// Tracks total distance traveled this run, in meters, at double
    /// precision for long-run accuracy. Deliberately derived from the Game
    /// Speed Controller's simulated speed (time-integrated) rather than from
    /// the Player's raw physics transform: this keeps "distance" a
    /// deterministic function of elapsed time and speed, which is exactly
    /// what a future server-authoritative leaderboard needs to validate a
    /// client-reported run.
    /// </summary>
    public sealed class DistanceTracker : SceneSingleton<DistanceTracker>
    {
        public double DistanceMeters { get; private set; }

        /// <summary>Advances distance by one frame. Called only while the game loop is Running.</summary>
        public void Tick(float deltaTime, float currentSpeed)
        {
            DistanceMeters += currentSpeed * deltaTime;
        }

        /// <summary>Resets distance to zero. Called by the game loop on Restart.</summary>
        public void ResetDistance()
        {
            DistanceMeters = 0d;
        }
    }
}

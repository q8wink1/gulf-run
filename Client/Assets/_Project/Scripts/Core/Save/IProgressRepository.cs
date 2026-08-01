namespace GulfRun.Core.Save
{
    /// <summary>
    /// Abstraction over persisted player progress (best distance/score, coins
    /// collected). Deliberately has no platform-specific implementation yet —
    /// per Sprint 3 scope, only the contract is prepared here. Whoever ends up
    /// implementing it (local file, PlayerPrefs, or a future Cloud Save-backed
    /// <see cref="Managers.SaveManager"/>) is free to change storage without
    /// gameplay code (game loop, scoring) ever knowing the difference.
    /// </summary>
    public interface IProgressRepository
    {
        float GetBestDistance();

        float GetBestScore();

        int GetCoinsCollected();

        /// <summary>Persists a new best distance (in meters). Callers are responsible for the "is this actually better" check.</summary>
        void SaveBestDistance(float distanceMeters);

        /// <summary>Persists a new best score. Callers are responsible for the "is this actually better" check.</summary>
        void SaveBestScore(float score);

        /// <summary>Adds to the lifetime coins-collected total.</summary>
        void AddCoinsCollected(int amount);
    }
}

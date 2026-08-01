using GulfRun.Core.Save;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Coordinates local and cloud save/load of player data. Device-specific
    /// settings may remain local; account-linked data synchronizes with the
    /// backend, which remains the source of truth.
    /// References: P034 (Settings System), P039 (Backend Architecture),
    /// P040 (Database Architecture).
    ///
    /// Sprint 3 note: implements <see cref="IProgressRepository"/> with a
    /// simple in-memory store so gameplay (Game Loop, Scoring) has a working
    /// default to read/write best-distance, best-score, and coins-collected
    /// against today. This is intentionally NOT platform-specific and does
    /// NOT persist across application restarts — it exists purely so the
    /// interface has a real, swappable implementation. Replace the storage
    /// inside this class (PlayerPrefs / local file / Cloud Save) once the
    /// Backend/Database systems land, with zero changes required to any
    /// caller since they only depend on <see cref="IProgressRepository"/>.
    ///
    /// Sprint 8 note: also implements <see cref="IAccountRepository"/> with
    /// the same in-memory posture — this is the single place
    /// "Country becomes permanently linked to the account" is enforced
    /// (<see cref="CreateAccount"/> silently refuses to overwrite an
    /// existing account), so every consumer (Character system, Multiplayer's
    /// <c>SessionManager</c>) reads the same locked value.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SaveManager : Singleton<SaveManager>, IProgressRepository, IAccountRepository
    {
        private float _bestDistanceMeters;
        private float _bestScore;
        private int _coinsCollected;

        private PlayerAccount _account;

        protected override void OnInitialize()
        {
            // TODO(Sprint 4+): Replace in-memory fields below with real local
            // persistence and cloud save synchronization once the
            // Backend/Database systems are online.
        }

        public float GetBestDistance() => _bestDistanceMeters;

        public float GetBestScore() => _bestScore;

        public int GetCoinsCollected() => _coinsCollected;

        public void SaveBestDistance(float distanceMeters) => _bestDistanceMeters = distanceMeters;

        public void SaveBestScore(float score) => _bestScore = score;

        public void AddCoinsCollected(int amount)
        {
            if (amount > 0)
            {
                _coinsCollected += amount;
            }
        }

        public bool HasAccount { get; private set; }

        public PlayerAccount GetAccount() => _account;

        public PlayerAccount CreateAccount(string displayName, GulfCountry country)
        {
            if (HasAccount)
            {
                // Country selection happens ONLY ONCE — a repeat call (even
                // with a different country) never modifies the existing
                // account. This is the enforcement point for
                // "Country cannot be changed later."
                return _account;
            }

            string safeDisplayName = string.IsNullOrWhiteSpace(displayName) ? "Player" : displayName;
            _account = new PlayerAccount(safeDisplayName, country);
            HasAccount = true;
            return _account;
        }
    }
}

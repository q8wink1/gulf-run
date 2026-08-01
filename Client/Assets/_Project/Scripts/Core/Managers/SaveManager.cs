using System;
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
    ///
    /// Sprint 14 note: <see cref="HasSeenIntro"/>/<see cref="MarkIntroSeen"/>
    /// are a deliberate exception to this class's in-memory posture —
    /// the Brand Intro's "player may skip it after the first launch" needs
    /// something that genuinely survives an app restart, and a single
    /// device-local boolean is exactly the narrow use case
    /// <see cref="UnityEngine.PlayerPrefs"/> is designed for (no
    /// backend/account coupling, unlike the rest of this class).
    ///
    /// Sprint 16 note: <see cref="ILoadoutRepository"/> is the second
    /// PlayerPrefs-backed exception — Locker equip/character/ownership must
    /// "save automatically" across restarts per the brief. Account progress
    /// (best distance/score/coins) remains in-memory until cloud save lands.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SaveManager : Singleton<SaveManager>, IProgressRepository, IAccountRepository, ILoadoutRepository
    {
        private const string HasSeenIntroPrefKey = "GulfRun.HasSeenIntro";
        private const string LoadoutPrefKey = "GulfRun.Loadout.v1";

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
            _account = new PlayerAccount(safeDisplayName, country, GenerateNewPlayerId());
            HasAccount = true;
            return _account;
        }

        /// <summary>Sprint 14 (Brand Intro): true once <see cref="MarkIntroSeen"/> has ever been called on this device, across app restarts.</summary>
        public bool HasSeenIntro => PlayerPrefs.GetInt(HasSeenIntroPrefKey, 0) == 1;

        /// <summary>Sprint 14 (Brand Intro): records that the intro has played at least once. Idempotent — safe to call every time the intro finishes/is skipped.</summary>
        public void MarkIntroSeen()
        {
            PlayerPrefs.SetInt(HasSeenIntroPrefKey, 1);
            PlayerPrefs.Save();
        }

        /// <summary>Sprint 16 (Locker): restores the last saved Character/equipped cosmetics/ownership, or false if nothing was ever saved.</summary>
        public bool TryLoadLoadout(out LoadoutSaveData data) => LoadoutSaveData.TryDecode(PlayerPrefs.GetString(LoadoutPrefKey, string.Empty), out data);

        /// <summary>Sprint 16 (Locker): persists Character + equipped slots + permanent/temporary ownership immediately (brief: equip saves automatically).</summary>
        public void SaveLoadout(LoadoutSaveData data)
        {
            if (data == null)
            {
                return;
            }

            PlayerPrefs.SetString(LoadoutPrefKey, data.Encode());
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Sprint 9: mints a permanent Player ID, once, alongside the
        /// account itself. Human-friendly ("GR-XXXXXX") rather than a raw
        /// GUID since it is shown directly on the Player Profile screen.
        /// In-memory only today, exactly like the rest of this class —
        /// swap for a real backend-assigned ID once accounts persist
        /// server-side (see class remarks).
        /// </summary>
        private static PlayerId GenerateNewPlayerId()
        {
            string suffix = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant();
            return new PlayerId("GR-" + suffix);
        }
    }
}

using System;
using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Backend
{
    /// <summary>
    /// The single seam every Sprint 11 Daily Missions / Login Reward
    /// manager talks to instead of any concrete backend system — the same
    /// "swap the implementation, zero caller changes" contract
    /// <see cref="IStoreBackendService"/>/<see cref="IOnlineBackendService"/>
    /// already give the Store and Online Ecosystem. Owns exactly what a
    /// real backend would own here — brief "BACKEND: Backend controls:
    /// Mission pool [selection result]. Mission difficulty [scaling
    /// already applied by the time a mission reaches this interface].
    /// Reward tables [snapshotted onto <see cref="ActiveMission"/>].
    /// Temporary durations [snapshotted the same way]. Login calendar
    /// [<see cref="LoginStreakStatus"/>]" — the 3 active missions' progress/
    /// claimed state, the daily reset timer, the Login Streak, and the
    /// generic reward ledger for slot-less reward types (ProfileFrame/
    /// ChampionEffect/Title/Badge). Applying a reward's local effect
    /// (crediting Coins/Gems, granting a cosmetic, adding Battle Pass XP)
    /// is deliberately NOT this interface's job — that stays in
    /// <c>Features.Progression.Missions.MissionManager</c>/
    /// <c>Login.LoginRewardManager</c>, the identical "backend records,
    /// feature manager applies" split <see cref="IStoreBackendService"/>
    /// already established.
    /// </summary>
    public interface IProgressionBackendService
    {
        // --- Daily Missions ---

        IReadOnlyList<ActiveMission> GetActiveMissions();

        /// <summary>True when no missions exist yet, or the 24-hour reset window has elapsed since the current set was generated.</summary>
        bool NeedsNewMissions(double nowSeconds);

        void SetActiveMissions(IReadOnlyList<ActiveMission> missions, double nowSeconds);

        double GetMissionsResetAtSeconds();

        /// <summary>Adds <paramref name="amount"/> of progress to every active, not-yet-claimed, not-yet-completed mission of <paramref name="type"/>. Returns the slot indices that became newly completed as a result (for a "Mission completed" notification), or an empty list.</summary>
        IReadOnlyList<int> ReportMissionProgress(MissionType type, int amount);

        bool TryMarkMissionClaimed(int slotIndex);

        event Action MissionsChanged;

        // --- Login Streak ---

        LoginStreakStatus GetLoginStreakStatus();

        bool HasClaimedLoginToday(double nowSeconds);

        void RecordLoginClaim(int streakDay, double nowSeconds);

        event Action LoginStreakChanged;

        // --- Generic reward ledger (ProfileFrame/ChampionEffect/Title/Badge — no dedicated slot yet) ---

        bool OwnsProgressionRewardItem(string ledgerKey);

        void GrantProgressionRewardItem(string ledgerKey, RewardType rewardType);

        IReadOnlyList<ProgressionRewardLedgerEntry> GetProgressionRewardLedger();

        event Action ProgressionLedgerChanged;
    }
}

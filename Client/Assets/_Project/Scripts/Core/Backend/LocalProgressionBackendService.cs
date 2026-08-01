using System;
using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Backend
{
    /// <summary>
    /// In-memory mock <see cref="IProgressionBackendService"/> — the
    /// Sprint 11 counterpart to <see cref="LocalStoreBackendService"/>:
    /// tracked exactly like a real backend would (mission ledger, login
    /// streak, generic reward ledger), so the whole Missions/Login Reward
    /// UI has real, interactive, swappable-later data from the first
    /// frame.
    /// </summary>
    public sealed class LocalProgressionBackendService : IProgressionBackendService
    {
        private const double MissionResetIntervalSeconds = 24 * 60 * 60;

        private readonly List<ActiveMission> _activeMissions = new List<ActiveMission>();
        private readonly LoginStreakStatus _loginStreak = new LoginStreakStatus();
        private readonly HashSet<string> _rewardLedgerKeys = new HashSet<string>();
        private readonly List<ProgressionRewardLedgerEntry> _rewardLedger = new List<ProgressionRewardLedgerEntry>();

        private double _missionsGeneratedAtSeconds;

        public event Action MissionsChanged;
        public event Action LoginStreakChanged;
        public event Action ProgressionLedgerChanged;

        // --- Daily Missions ---

        public IReadOnlyList<ActiveMission> GetActiveMissions() => _activeMissions;

        public bool NeedsNewMissions(double nowSeconds) => _activeMissions.Count == 0 || nowSeconds >= _missionsGeneratedAtSeconds + MissionResetIntervalSeconds;

        public void SetActiveMissions(IReadOnlyList<ActiveMission> missions, double nowSeconds)
        {
            _activeMissions.Clear();
            if (missions != null)
            {
                _activeMissions.AddRange(missions);
            }

            _missionsGeneratedAtSeconds = nowSeconds;
            MissionsChanged?.Invoke();
        }

        public double GetMissionsResetAtSeconds() => _missionsGeneratedAtSeconds + MissionResetIntervalSeconds;

        public IReadOnlyList<int> ReportMissionProgress(MissionType type, int amount)
        {
            if (amount <= 0)
            {
                return Array.Empty<int>();
            }

            List<int> completed = null;
            for (int i = 0; i < _activeMissions.Count; i++)
            {
                ActiveMission mission = _activeMissions[i];
                if (mission.Type != type || mission.IsClaimed || mission.IsCompleted)
                {
                    continue;
                }

                mission.AddProgress(amount);
                if (mission.IsCompleted)
                {
                    (completed ??= new List<int>()).Add(i);
                }
            }

            if (completed != null)
            {
                MissionsChanged?.Invoke();
                return completed;
            }

            return Array.Empty<int>();
        }

        public bool TryMarkMissionClaimed(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _activeMissions.Count)
            {
                return false;
            }

            ActiveMission mission = _activeMissions[slotIndex];
            if (mission.IsClaimed || !mission.IsCompleted)
            {
                return false;
            }

            mission.IsClaimed = true;
            MissionsChanged?.Invoke();
            return true;
        }

        // --- Login Streak ---

        public LoginStreakStatus GetLoginStreakStatus() => _loginStreak;

        public bool HasClaimedLoginToday(double nowSeconds) => LoginStreakCalculator.HasClaimedForToday(_loginStreak.LastClaimAtSeconds, nowSeconds);

        public void RecordLoginClaim(int streakDay, double nowSeconds)
        {
            _loginStreak.CurrentStreakDay = streakDay;
            _loginStreak.LastClaimAtSeconds = nowSeconds;
            _loginStreak.TotalLoginsEver += 1;
            LoginStreakChanged?.Invoke();
        }

        // --- Generic reward ledger ---

        public bool OwnsProgressionRewardItem(string ledgerKey) => !string.IsNullOrEmpty(ledgerKey) && _rewardLedgerKeys.Contains(ledgerKey);

        public void GrantProgressionRewardItem(string ledgerKey, RewardType rewardType)
        {
            if (string.IsNullOrEmpty(ledgerKey) || !_rewardLedgerKeys.Add(ledgerKey))
            {
                return;
            }

            _rewardLedger.Add(new ProgressionRewardLedgerEntry(ledgerKey, rewardType, DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
            ProgressionLedgerChanged?.Invoke();
        }

        public IReadOnlyList<ProgressionRewardLedgerEntry> GetProgressionRewardLedger() => _rewardLedger;
    }
}

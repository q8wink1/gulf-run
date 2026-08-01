using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Backend;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Progression.Configuration;
using UnityEngine;

namespace GulfRun.Features.Progression.Missions
{
    /// <summary>
    /// Composition root for Daily Missions — the same role
    /// <c>Features.Store.BattlePass.BattlePassManager</c> plays for the
    /// Battle Pass. Generates 3 random missions from
    /// <see cref="MissionPoolCatalogConfig"/> every 24 hours (brief: "Every
    /// player receives: 3 Daily Missions only ... Daily missions reset
    /// every 24 hours"), listens to <see cref="PlayerStatEventService"/>
    /// for progress, and applies a claimed mission's reward via
    /// <see cref="RewardApplication"/>. Persistent (Boot-scene, alongside
    /// the other Sprint 7-10 progression-adjacent managers).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MissionManager : Singleton<MissionManager>
    {
        private const int DailyMissionCount = 3;

        [SerializeField] private MissionPoolCatalogConfig pool;

        private IRandomSource _random;

        public MissionPoolCatalogConfig Pool => pool;

        public IReadOnlyList<ActiveMission> ActiveMissions => ProgressionBackendService.Current.GetActiveMissions();

        public double MissionsResetAtSeconds => ProgressionBackendService.Current.GetMissionsResetAtSeconds();

        protected override void OnInitialize()
        {
            _random = SeededRandom.FromTime();
        }

        private void OnEnable()
        {
            PlayerStatEventService.LocalMatchCompleted += HandleLocalMatchCompleted;
            PlayerStatEventService.LocalWeaponUsed += HandleLocalWeaponUsed;
            PlayerStatEventService.LocalTrapAvoided += HandleLocalTrapAvoided;
            PlayerStatEventService.LocalJumpPerformed += HandleLocalJumpPerformed;
            PlayerStatEventService.LocalItemBoxOpened += HandleLocalItemBoxOpened;
        }

        private void OnDisable()
        {
            PlayerStatEventService.LocalMatchCompleted -= HandleLocalMatchCompleted;
            PlayerStatEventService.LocalWeaponUsed -= HandleLocalWeaponUsed;
            PlayerStatEventService.LocalTrapAvoided -= HandleLocalTrapAvoided;
            PlayerStatEventService.LocalJumpPerformed -= HandleLocalJumpPerformed;
            PlayerStatEventService.LocalItemBoxOpened -= HandleLocalItemBoxOpened;
        }

        private void Update()
        {
            EnsureMissionsFresh();
        }

        /// <summary>"Login today" mission progress + a fresh-missions check both funnel through the daily login claim, so <c>Login.LoginRewardManager</c> calls this the moment a login is recorded for the day.</summary>
        public void ReportLogin() => ReportProgress(MissionType.LoginToday, 1);

        public bool TryClaimMission(int slotIndex)
        {
            IReadOnlyList<ActiveMission> missions = ProgressionBackendService.Current.GetActiveMissions();
            if (slotIndex < 0 || slotIndex >= missions.Count)
            {
                return false;
            }

            ActiveMission mission = missions[slotIndex];
            if (mission.IsClaimed || !mission.IsCompleted)
            {
                return false;
            }

            if (!ProgressionBackendService.Current.TryMarkMissionClaimed(slotIndex))
            {
                return false;
            }

            RewardApplication.Apply(mission.RewardType, mission.RewardAmount, mission.RewardCosmeticId, mission.IsTemporaryCosmeticReward, mission.RewardDuration, mission.FallbackCoinsAmount, "mission_" + mission.SourceMissionId + "_" + slotIndex);
            return true;
        }

        private void EnsureMissionsFresh()
        {
            if (pool == null)
            {
                return;
            }

            double now = NowSeconds();
            if (!ProgressionBackendService.Current.NeedsNewMissions(now))
            {
                return;
            }

            List<ActiveMission> generated = GenerateDailyMissions();
            if (generated.Count == 0)
            {
                return;
            }

            ProgressionBackendService.Current.SetActiveMissions(generated, now);
            ProgressionNotificationBridge.Raise(NotificationType.NewMissionsAvailable, generated.Count + " new Daily Missions are available!");
        }

        private List<ActiveMission> GenerateDailyMissions()
        {
            var source = new List<MissionPoolCatalogConfig.MissionPoolEntry>(pool.Missions);
            var result = new List<ActiveMission>(DailyMissionCount);
            int count = Math.Min(DailyMissionCount, source.Count);

            for (int i = 0; i < count; i++)
            {
                int pick = _random.NextInt(0, source.Count);
                MissionPoolCatalogConfig.MissionPoolEntry entry = source[pick];
                source.RemoveAt(pick);

                int scaledReward = ScaleReward(entry);
                result.Add(new ActiveMission(entry.Id, entry.DisplayName, entry.Type, entry.Difficulty, entry.TargetAmount, entry.RewardType, scaledReward, entry.RewardCosmeticId, entry.IsTemporaryCosmeticReward, entry.TemporaryDuration, entry.FallbackCoinsAmount));
            }

            return result;
        }

        private int ScaleReward(MissionPoolCatalogConfig.MissionPoolEntry entry)
        {
            if (entry.RewardType != RewardType.Coins && entry.RewardType != RewardType.Gems && entry.RewardType != RewardType.BattlePassXp)
            {
                // Cosmetic/ledger reward types are not amount-scaled by difficulty.
                return entry.RewardAmount;
            }

            float multiplier = pool.GetRewardMultiplier(entry.Difficulty);
            return (int)Math.Round(entry.RewardAmount * multiplier, MidpointRounding.AwayFromZero);
        }

        private void HandleLocalMatchCompleted(PlayerMatchOutcome outcome)
        {
            if (outcome.Reason == FinishReason.Completed)
            {
                ReportProgress(MissionType.FinishRaces, 1);

                if (outcome.FinishPosition == 1)
                {
                    ReportProgress(MissionType.WinRaces, 1);
                }

                if (outcome.FinishPosition >= 1 && outcome.FinishPosition <= 3)
                {
                    ReportProgress(MissionType.ReachTopThree, 1);
                }
            }

            if (outcome.CoinsCollected > 0)
            {
                ReportProgress(MissionType.CollectCoins, outcome.CoinsCollected);
            }
        }

        private void HandleLocalWeaponUsed() => ReportProgress(MissionType.UseWeapons, 1);

        private void HandleLocalTrapAvoided() => ReportProgress(MissionType.AvoidTraps, 1);

        private void HandleLocalJumpPerformed() => ReportProgress(MissionType.PerformJumps, 1);

        private void HandleLocalItemBoxOpened() => ReportProgress(MissionType.OpenItemBoxes, 1);

        private void ReportProgress(MissionType type, int amount)
        {
            IReadOnlyList<int> newlyCompleted = ProgressionBackendService.Current.ReportMissionProgress(type, amount);
            if (newlyCompleted.Count == 0)
            {
                return;
            }

            ProgressionNotificationBridge.Raise(NotificationType.MissionCompleted, newlyCompleted.Count == 1 ? "A Daily Mission is complete! Claim your reward." : newlyCompleted.Count + " Daily Missions are complete! Claim your rewards.");
        }

        private static double NowSeconds() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}

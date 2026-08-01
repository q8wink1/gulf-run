using GulfRun.Core;
using GulfRun.Core.Services;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Online.Statistics
{
    /// <summary>
    /// Owns the local player's single <see cref="PlayerMatchStatistics"/>
    /// accumulator (every field the brief's "Player Statistics" section
    /// lists) and feeds it purely from <see cref="PlayerStatEventService"/> —
    /// no reference to PlayerController/Weapons/Traps/RaceFinish at all, so
    /// this stays a Features.Online-only concern. In-memory only today,
    /// resetting on Play Mode restart (see Sprint 9 report Remaining TODOs
    /// for real persistence).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayerStatisticsTracker : Singleton<PlayerStatisticsTracker>
    {
        private readonly PlayerMatchStatistics _statistics = new PlayerMatchStatistics();

        public PlayerMatchStatistics Statistics => _statistics;

        protected override void OnInitialize()
        {
        }

        private void OnEnable()
        {
            PlayerStatEventService.LocalMatchCompleted += HandleLocalMatchCompleted;
            PlayerStatEventService.LocalWeaponUsed += _statistics.RecordWeaponUsed;
            PlayerStatEventService.LocalTrapHit += _statistics.RecordTrapHit;
            PlayerStatEventService.LocalJumpPerformed += _statistics.RecordJump;
        }

        private void OnDisable()
        {
            PlayerStatEventService.LocalMatchCompleted -= HandleLocalMatchCompleted;
            PlayerStatEventService.LocalWeaponUsed -= _statistics.RecordWeaponUsed;
            PlayerStatEventService.LocalTrapHit -= _statistics.RecordTrapHit;
            PlayerStatEventService.LocalJumpPerformed -= _statistics.RecordJump;
        }

        private void HandleLocalMatchCompleted(PlayerMatchOutcome outcome)
        {
            CharacterId characterPlayed = LocalLoadoutProviderService.Current != null
                ? LocalLoadoutProviderService.Current.CurrentCharacterId
                : CharacterId.None;

            _statistics.RecordMatch(outcome, characterPlayed);
        }
    }
}

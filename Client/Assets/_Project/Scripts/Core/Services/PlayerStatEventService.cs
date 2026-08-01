using System;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Sprint 9 "Player Statistics" event bridge — the exact same
    /// decoupling shape Sprint 8's <see cref="CharacterAnimationCueService"/>
    /// established: the features that actually observe an action
    /// (PlayerController's jump, Weapons' confirmed use/pickup, Traps'
    /// confirmed hit/avoidance, RaceFinish's local race outcome) raise
    /// these static events; <c>Features.Online.Statistics.PlayerStatisticsTracker</c>
    /// and (Sprint 11) <c>Features.Progression.Missions.MissionManager</c>
    /// are the only subscribers, so none of those producer features need to
    /// reference Features.Online/Features.Progression (or each other)
    /// directly.
    /// </summary>
    public static class PlayerStatEventService
    {
        /// <summary>Raised once the local player's race outcome is fully known (see <see cref="PlayerMatchOutcome"/>).</summary>
        public static event Action<PlayerMatchOutcome> LocalMatchCompleted;

        /// <summary>Raised once per confirmed weapon activation by the local player.</summary>
        public static event Action LocalWeaponUsed;

        /// <summary>Raised once per confirmed trap hit against the local player.</summary>
        public static event Action LocalTrapHit;

        /// <summary>Raised once per accepted jump (ground or double) by the local player.</summary>
        public static event Action LocalJumpPerformed;

        /// <summary>Sprint 11: raised once per confirmed Item Box pickup touched by the local player (whether or not a weapon was actually granted — "opening" the box, not receiving its contents, is what Daily Missions count).</summary>
        public static event Action LocalItemBoxOpened;

        /// <summary>Sprint 11: raised once per active trap instance that expired without ever hitting the local player — see <see cref="Domain.MissionType.AvoidTraps"/> remarks for this honestly-scoped "avoided" definition.</summary>
        public static event Action LocalTrapAvoided;

        public static void RaiseLocalMatchCompleted(PlayerMatchOutcome outcome) => LocalMatchCompleted?.Invoke(outcome);

        public static void RaiseLocalWeaponUsed() => LocalWeaponUsed?.Invoke();

        public static void RaiseLocalTrapHit() => LocalTrapHit?.Invoke();

        public static void RaiseLocalJumpPerformed() => LocalJumpPerformed?.Invoke();

        public static void RaiseLocalItemBoxOpened() => LocalItemBoxOpened?.Invoke();

        public static void RaiseLocalTrapAvoided() => LocalTrapAvoided?.Invoke();
    }
}

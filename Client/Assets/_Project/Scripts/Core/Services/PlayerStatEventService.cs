using System;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Sprint 9 "Player Statistics" event bridge — the exact same
    /// decoupling shape Sprint 8's <see cref="CharacterAnimationCueService"/>
    /// established: the features that actually observe an action
    /// (PlayerController's jump, Weapons' confirmed use, Traps' confirmed
    /// hit, RaceFinish's local race outcome) raise these static events;
    /// <c>Features.Online.Statistics.PlayerStatisticsTracker</c> is the
    /// only subscriber, so none of those four features need to reference
    /// Features.Online (or each other) directly.
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

        public static void RaiseLocalMatchCompleted(PlayerMatchOutcome outcome) => LocalMatchCompleted?.Invoke(outcome);

        public static void RaiseLocalWeaponUsed() => LocalWeaponUsed?.Invoke();

        public static void RaiseLocalTrapHit() => LocalTrapHit?.Invoke();

        public static void RaiseLocalJumpPerformed() => LocalJumpPerformed?.Invoke();
    }
}

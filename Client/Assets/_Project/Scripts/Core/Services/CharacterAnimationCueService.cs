using System;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// One-shot animation cue bus for the local player's avatar, letting
    /// <c>Features.RaceFinish</c> (which raises Win/Lose the instant the
    /// local player's own <see cref="Domain.PlayerRaceResult"/> resolves,
    /// and Celebrate the instant the local Podium Ceremony phase begins for
    /// a local top-3 finish) drive <c>Features.PlayerController.
    /// PlayerAnimatorDriver</c> without either feature assembly referencing
    /// the other — the same zero-coupling seam pattern as
    /// <see cref="RaceProgressService"/>/<see cref="GameStateService"/>.
    ///
    /// Only ever raised for the local player: no networked remote-player
    /// avatar exists yet in this project (see Sprint 4/7 reports), so there
    /// is nothing else to target today. Extending this to carry a
    /// connection id once a remote avatar exists is a additive, non-breaking
    /// change.
    /// </summary>
    public static class CharacterAnimationCueService
    {
        public static event Action<CharacterAnimationState> LocalCueRaised;

        public static void RaiseLocalCue(CharacterAnimationState state) => LocalCueRaised?.Invoke(state);
    }
}

using System;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Abstraction over "what is the current session state right now"
    /// (Ready/Countdown/Running/Paused/GameOver/Restart). Exists in Core so
    /// PlayerController can react to the race lifecycle (auto-run only while
    /// Running, freeze input during Countdown/GameOver, ...) without the
    /// PlayerController feature referencing the EndlessRunner feature
    /// directly — see <see cref="GameStateService"/> and the equivalent
    /// pattern used by <see cref="IRunSpeedProvider"/>.
    /// </summary>
    public interface IGameStateProvider
    {
        GameLoopState CurrentState { get; }

        /// <summary>Raised whenever <see cref="CurrentState"/> changes.</summary>
        event Action<GameLoopState> StateChanged;
    }
}

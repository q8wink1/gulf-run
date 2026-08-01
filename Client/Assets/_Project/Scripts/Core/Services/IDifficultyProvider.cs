namespace GulfRun.Core.Services
{
    /// <summary>
    /// Read-only view of the current 0..1 difficulty ramp, published by the
    /// endless-runner <c>DifficultyController</c> (computed once per tick
    /// from distance traveled) so other features can scale their own
    /// difficulty-driven behaviour — e.g. Sprint 6's
    /// <c>Features.Traps.Authority.TrapAuthority</c> spawn rate/concurrency —
    /// without depending on the EndlessRunner feature assembly directly.
    /// Same decoupling pattern as <see cref="IRunSpeedProvider"/> and
    /// <see cref="IGameStateProvider"/>.
    /// </summary>
    public interface IDifficultyProvider
    {
        /// <summary>Normalized difficulty ramp: 0 at race start, 1 once fully ramped.</summary>
        float Current01 { get; }
    }
}

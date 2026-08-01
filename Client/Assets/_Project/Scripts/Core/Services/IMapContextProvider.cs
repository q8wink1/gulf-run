using System;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Read-only seam onto "what is this match's current Map/Weather/Time
    /// of Day/random seeds" — the same "implement a Core interface, never
    /// reference the owning Feature" shape as
    /// <see cref="ICosmeticGrantService"/>/<see cref="IBattlePassXpGrantService"/>.
    /// Implemented by <c>Features.Maps.MapEnvironmentManager</c>; consumed
    /// by <c>Features.Traps</c>/<c>Features.EndlessRunner</c> (re-seeding on
    /// a new match) and any debug/UI view that must show the active
    /// environment, none of which may reference Features.Maps directly.
    /// </summary>
    public interface IMapContextProvider
    {
        /// <summary>False until the very first match's environment has been resolved (Countdown of the first race).</summary>
        bool HasResolvedEnvironment { get; }

        /// <summary>The most recently resolved environment. Only meaningful once <see cref="HasResolvedEnvironment"/> is true.</summary>
        MatchEnvironmentSelection Current { get; }

        /// <summary>Raised every time a new match's environment has just been resolved (Sprint 12: "before every race").</summary>
        event Action<MatchEnvironmentSelection> EnvironmentResolved;

        /// <summary>
        /// Sprint 13: resolves <paramref name="mapId"/>'s presentation
        /// display name (e.g. "Kuwait City") for the Main Menu's "Current
        /// selected map" readout, without the caller ever needing
        /// <c>Features.Maps.Configuration.MapCatalogConfig</c> directly.
        /// Falls back to the raw <see cref="MapId.Value"/> if unresolved.
        /// </summary>
        string ResolveMapDisplayName(MapId mapId);

        /// <summary>Sprint 13: resolves a match's random environment right now, for a menu/lobby context with no Countdown to react to. A no-op if catalogs are unassigned.</summary>
        void ResolveNewEnvironment();
    }
}

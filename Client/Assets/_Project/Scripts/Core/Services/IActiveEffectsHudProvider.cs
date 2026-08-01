using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Active status-effect chips for Features.RaceHud. Implemented by
    /// <c>Features.PlayerController.PlayerStatusEffectController</c> when a
    /// local player exists; otherwise null / empty.
    /// </summary>
    public interface IActiveEffectsHudProvider
    {
        bool HasShield { get; }
        bool HasSpeedBoost { get; }
        IReadOnlyList<ActiveHudEffectSnapshot> ActiveEffects { get; }
    }

    public static class ActiveEffectsHudService
    {
        public static IActiveEffectsHudProvider Current { get; set; }
    }
}

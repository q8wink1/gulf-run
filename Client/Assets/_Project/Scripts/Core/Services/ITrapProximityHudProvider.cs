using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Nearby-trap warning for Features.RaceHud (indicator only — never
    /// auto-avoids). Implemented by
    /// <c>Features.Traps.Proximity.TrapProximityWatcher</c>.
    /// </summary>
    public interface ITrapProximityHudProvider
    {
        bool IsTrapNearby { get; }
        TrapId? NearbyTrapId { get; }
        float Proximity01 { get; }
    }

    public static class TrapProximityHudService
    {
        public static ITrapProximityHudProvider Current { get; set; }
    }
}

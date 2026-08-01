using System;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Read-only race-start countdown display state for Features.RaceHud —
    /// implemented by <c>Features.EndlessRunner.GameLoop.CountdownController</c>
    /// so the HUD never references EndlessRunner directly.
    /// </summary>
    public interface ICountdownHudProvider
    {
        bool IsActive { get; }
        string DisplayText { get; }
        int SecondsRemaining { get; }
        bool IsGo { get; }
        event Action<int> SecondsChanged;
        event Action Finished;
    }

    public static class CountdownHudService
    {
        public static ICountdownHudProvider Current { get; set; }
    }
}

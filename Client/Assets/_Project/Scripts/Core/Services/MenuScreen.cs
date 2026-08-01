namespace GulfRun.Core.Services
{
    /// <summary>Every screen the Sprint 13 Main Menu's Top Bar/Left Menu/Right Menu can open, routed through <see cref="MenuScreenRouter"/> so Features.MainMenu never references the Feature assembly that actually owns the screen.</summary>
    public enum MenuScreen
    {
        Friends,
        Leaderboard,
        Missions,
        BattlePass,
        Store,
        Characters,
        Locker,
        Inventory,
        Events,
        Championships,
        Notifications
    }
}

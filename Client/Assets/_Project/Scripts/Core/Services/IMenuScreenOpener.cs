namespace GulfRun.Core.Services
{
    /// <summary>
    /// Implemented by an existing Feature's own screen/panel (e.g.
    /// <c>FriendListView</c>, <c>LeaderboardView</c>) so
    /// <see cref="MenuScreenRouter"/> can open it from the Sprint 13 Main
    /// Menu's Left/Right Menu buttons with zero compile-time reference
    /// from Features.MainMenu to the owning Feature assembly.
    /// </summary>
    public interface IMenuScreenOpener
    {
        void OpenScreen(MenuScreen screen);
    }
}

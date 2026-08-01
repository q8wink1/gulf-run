namespace GulfRun.Core.Services
{
    /// <summary>
    /// Read-only seam onto the local player's Friends counts (Sprint 13
    /// "SOCIAL: Friends Online"). Implemented by
    /// <c>Features.Online.Friends.FriendManager</c> so Features.MainMenu
    /// can show it without ever referencing Features.Online.
    /// </summary>
    public interface IFriendsSummaryProvider
    {
        int TotalFriendsCount { get; }

        int OnlineFriendsCount { get; }
    }
}

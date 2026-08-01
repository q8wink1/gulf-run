namespace GulfRun.Domain
{
    /// <summary>
    /// The relationship between the local player and some other player, as
    /// seen from wherever that other player is displayed (Leaderboard,
    /// Search, Player Profile, ...) — drives which action button(s) a view
    /// should offer (Add Friend / Cancel Request / Accept / Remove / Unblock).
    /// </summary>
    public enum FriendLinkState
    {
        None,
        Friends,
        RequestSentByMe,
        RequestReceivedFromThem,
        Blocked
    }
}

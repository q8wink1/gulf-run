namespace GulfRun.Domain
{
    /// <summary>Lifecycle of one Friend Request, per the Sprint 9 Friend System brief.</summary>
    public enum FriendRequestStatus
    {
        Pending,
        Accepted,
        Rejected,
        Cancelled
    }
}

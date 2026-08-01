namespace GulfRun.Domain
{
    /// <summary>One Send/Accept/Reject/Cancel Friend Request record.</summary>
    public readonly struct FriendRequest
    {
        public readonly PlayerId From;
        public readonly PlayerId To;
        public readonly FriendRequestStatus Status;
        public readonly double TimestampSeconds;

        public FriendRequest(PlayerId from, PlayerId to, FriendRequestStatus status, double timestampSeconds)
        {
            From = from;
            To = to;
            Status = status;
            TimestampSeconds = timestampSeconds;
        }

        public FriendRequest WithStatus(FriendRequestStatus status) => new FriendRequest(From, To, status, TimestampSeconds);
    }
}

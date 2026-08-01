namespace GulfRun.Domain
{
    /// <summary>One queued/displayed notification — see <c>Features.Online.Notifications.NotificationManager</c>.</summary>
    public readonly struct PlayerNotification
    {
        public readonly NotificationType Type;
        public readonly string Message;
        public readonly double TimestampSeconds;
        public readonly bool Read;

        public PlayerNotification(NotificationType type, string message, double timestampSeconds, bool read)
        {
            Type = type;
            Message = message;
            TimestampSeconds = timestampSeconds;
            Read = read;
        }

        public PlayerNotification AsRead() => new PlayerNotification(Type, Message, TimestampSeconds, true);
    }
}

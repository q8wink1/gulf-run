namespace GulfRun.Domain
{
    /// <summary>Why a participant left a match, for debug/UI/analytics purposes.</summary>
    public enum DisconnectReason
    {
        PlayerLeft,
        HostLeft,
        Timeout,
        Unknown
    }
}

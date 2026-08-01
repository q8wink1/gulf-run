namespace GulfRun.Domain
{
    /// <summary>Why a participant left a match, for debug/UI/analytics purposes.</summary>
    public enum DisconnectReason
    {
        PlayerLeft,
        HostLeft,
        Timeout,
        Unknown,

        /// <summary>Sprint 15 (Owner Feature "Kick Player"). Appended rather than inserted so no existing ordinal shifts.</summary>
        Kicked
    }
}

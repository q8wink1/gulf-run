namespace GulfRun.Domain
{
    /// <summary>A player's live elimination standing during a race, for debug/UI display.</summary>
    public enum EliminationStatus
    {
        /// <summary>Within the allowed gap of the leader (or the finish line, once someone has crossed it).</summary>
        Safe,

        /// <summary>Fell too far behind; an elimination countdown is running unless the player recovers.</summary>
        Warning,

        /// <summary>The elimination countdown reached zero; the player has been assigned a final result.</summary>
        Eliminated
    }
}

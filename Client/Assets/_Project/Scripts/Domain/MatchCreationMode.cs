namespace GulfRun.Domain
{
    /// <summary>
    /// How the local player's current match was created (Sprint 15
    /// "Matchmaking, Room &amp; Pre-Race Lobby"): via public automatic
    /// matchmaking (<see cref="QuickPlay"/>, P017 "Quick Match" — searches
    /// automatically, never shows Bot Fill controls per P017 §12 "Explicitly
    /// Not Defined: Bot Filling"), or via an invitation-only
    /// <see cref="PrivateRoom"/> (P018 — Room Code/Friend-Invitation join,
    /// Bot Fill available to the Room Owner). Both are the exact same
    /// underlying Lobby/Match machinery (<see cref="MatchState"/>,
    /// <see cref="MatchParticipant"/>) — this flag only ever changes which
    /// Pre-Race Lobby controls are shown, never the networking model.
    /// </summary>
    public enum MatchCreationMode
    {
        QuickPlay,
        PrivateRoom
    }
}

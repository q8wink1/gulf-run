namespace GulfRun.Domain
{
    /// <summary>
    /// Pure resolution of the local player's <see cref="OnlineStatus"/> from
    /// facts every caller already has cheaply available (no new state of
    /// its own): whether an account exists yet, whether the match
    /// transport is currently active, its <see cref="MatchState"/>, and
    /// whether the current match is running under an active championship.
    /// Kept in Domain (rather than inline in
    /// <c>Features.Online.Profile.ProfileManager</c>) so it stays a single,
    /// independently testable decision instead of duplicated branching.
    /// </summary>
    public static class OnlineStatusResolver
    {
        public static OnlineStatus Resolve(bool hasAccount, bool transportActive, MatchState matchState, bool isTournamentContext)
        {
            if (!hasAccount)
            {
                return OnlineStatus.Offline;
            }

            if (!transportActive)
            {
                return OnlineStatus.Online;
            }

            if (isTournamentContext)
            {
                return OnlineStatus.InTournament;
            }

            return matchState == MatchState.Waiting ? OnlineStatus.InLobby : OnlineStatus.InMatch;
        }
    }
}

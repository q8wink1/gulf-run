namespace GulfRun.Domain
{
    /// <summary>
    /// The permanent, once-only outcome of Account Creation (Sprint 8):
    /// a Display Name and a <see cref="GulfCountry"/> that — per the brief —
    /// "becomes permanently linked to the account" and "cannot be changed
    /// later." Everything the Country determines (National Flag, Traditional
    /// Outfit, Profile/Lobby/Podium Flag) is derived from this struct alone;
    /// see <c>Core.Save.IAccountRepository</c> for the one-time-creation
    /// contract that guarantees the immutability rule.
    ///
    /// Sprint 9 note: also carries a permanent <see cref="PlayerId"/>,
    /// minted once by <c>Core.Managers.SaveManager.CreateAccount</c> the
    /// same instant the account itself is created. This is the stable
    /// identity the whole Online Ecosystem (Leaderboards, Friends, Hall of
    /// Fame, Profile) hangs its data on — distinct from
    /// <see cref="PlayerIdentity.PlayerId"/>, which is only a per-session
    /// match/connection identifier.
    /// </summary>
    public readonly struct PlayerAccount
    {
        public readonly string DisplayName;
        public readonly GulfCountry Country;
        public readonly PlayerId PlayerId;

        public PlayerAccount(string displayName, GulfCountry country, PlayerId playerId)
        {
            DisplayName = displayName;
            Country = country;
            PlayerId = playerId;
        }
    }
}

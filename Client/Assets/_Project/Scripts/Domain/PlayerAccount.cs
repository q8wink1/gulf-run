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
    /// </summary>
    public readonly struct PlayerAccount
    {
        public readonly string DisplayName;
        public readonly GulfCountry Country;

        public PlayerAccount(string displayName, GulfCountry country)
        {
            DisplayName = displayName;
            Country = country;
        }
    }
}

namespace GulfRun.Domain
{
    /// <summary>
    /// The single "public profile" DTO the whole Sprint 9 online ecosystem
    /// passes around: what a Leaderboard row, a Search result, a Friend
    /// list entry, and the full Player Profile screen all ultimately show,
    /// carrying every field the brief's "Player Profile" section lists.
    /// One shared type on purpose (Don't Repeat Yourself) rather than a
    /// separate shape per screen — a mutable class since it is always a
    /// freshly rebuilt snapshot (see
    /// <c>Features.Online.Profile.ProfileManager.BuildLocalProfile</c> and
    /// <c>Core.Backend.LocalOnlineBackendService</c>'s seeded rows), never
    /// a value incrementally mutated in place.
    /// </summary>
    public sealed class PlayerProfileSummary
    {
        public PlayerId PlayerId { get; set; } = PlayerId.None;
        public string Nickname { get; set; } = string.Empty;
        public GulfCountry Country { get; set; }
        public string CurrentCharacterDisplayName { get; set; } = string.Empty;
        public string CurrentOutfitDisplayName { get; set; } = string.Empty;

        /// <summary>Sprint 13 (P024 Level System) — see <see cref="PlayerLevelRules"/>.</summary>
        public int Level { get; set; } = 1;
        public int CurrentXp { get; set; }
        public int XpRequiredForNextLevel { get; set; } = PlayerLevelRules.XpRequiredForLevel(1);

        public SeasonProgress Season { get; set; }
        public int WorldRank { get; set; } = -1;
        public int GulfRank { get; set; } = -1;
        public int CountryRank { get; set; } = -1;
        public int TotalWins { get; set; }
        public int Top3Finishes { get; set; }
        public float WinRate { get; set; }
        public float BestFinishTimeSeconds { get; set; } = -1f;
        public int Coins { get; set; }
        public int Gems { get; set; }
        public string FavouriteCharacterDisplayName { get; set; } = string.Empty;
        public OnlineStatus Status { get; set; }
    }
}

namespace GulfRun.Domain
{
    /// <summary>
    /// The eight launch nations for Project GulfRun's Account/Country system
    /// (Sprint 8): the original six GCC nations from Sprint 7 (see
    /// Design/GDD/P006-MAP-SYSTEM-v1.0.md §3: Riyadh/Saudi Arabia, Jahra/Kuwait,
    /// Dubai/UAE, Doha/Qatar, Manama/Bahrain, Muscat/Oman), plus Iraq and Egypt
    /// added per the Sprint 8 brief's explicit "Launch Countries" list. Selected
    /// exactly once at account creation (see <see cref="PlayerAccount"/> /
    /// <c>Core.Save.IAccountRepository</c>) and permanently linked to the
    /// account thereafter — it determines the player's National/Profile/
    /// Lobby/Podium Flag and free Traditional Outfit (Sprint 8), and is still
    /// what the Victory Ceremony shows behind each podium position (Sprint 7
    /// addendum). Design/GDD/P020-PLAYER-PROFILE-SYSTEM-v1.0.md marks a
    /// profile "Country" field as "Future"; this sprint's explicit country
    /// system supersedes that placeholder status the same way Sprint 7's
    /// reward table superseded P011's "not yet defined" note.
    /// </summary>
    public enum GulfCountry
    {
        SaudiArabia,
        Kuwait,
        UnitedArabEmirates,
        Qatar,
        Bahrain,
        Oman,

        /// <summary>Added in Sprint 8 per the brief's launch country list.</summary>
        Iraq,

        /// <summary>Added in Sprint 8 per the brief's launch country list.</summary>
        Egypt
    }
}

namespace GulfRun.Domain
{
    /// <summary>
    /// The six GCC nations GulfRun's official maps represent (see
    /// Design/GDD/P006-MAP-SYSTEM-v1.0.md §3: Riyadh/Saudi Arabia, Jahra/Kuwait,
    /// Dubai/UAE, Doha/Qatar, Manama/Bahrain, Muscat/Oman) — reused here as the
    /// local player's selectable nationality on their <see cref="PlayerIdentity"/>,
    /// so the Victory Ceremony can show "the national flag of each winning
    /// player behind their podium position." Design/GDD/P020-PLAYER-PROFILE-SYSTEM-v1.0.md
    /// marks a profile "Country" field as "Future"; this sprint's explicit
    /// flag requirement supersedes that placeholder status the same way
    /// Sprint 7's reward table superseded P011's "not yet defined" note (see
    /// the Sprint 7 addendum report for how this is reconciled).
    /// </summary>
    public enum GulfCountry
    {
        SaudiArabia,
        Kuwait,
        UnitedArabEmirates,
        Qatar,
        Bahrain,
        Oman
    }
}

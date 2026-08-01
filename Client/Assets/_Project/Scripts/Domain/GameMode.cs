namespace GulfRun.Domain
{
    /// <summary>
    /// Every game mode Project GulfRun currently implements. Sprint 1-12
    /// only ever built the one shared "Auto Run, up to 4 players, 60-90s"
    /// race loop the Sprint 12 brief describes — a single-entry enum here
    /// (rather than a hardcoded "Quick Race" string wherever the current
    /// mode is displayed) is honest about that today while giving the
    /// Bottom Bar's "Current Game Mode" a real, reusable value to grow
    /// alongside future modes instead of a throwaway literal.
    /// </summary>
    public enum GameMode
    {
        QuickRace
    }
}

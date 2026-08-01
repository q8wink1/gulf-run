namespace GulfRun.Domain
{
    /// <summary>
    /// The body/rig presentation a <c>Features.Character.Configuration.
    /// CharacterDefinition</c> uses, so the 12 launch characters can share
    /// exactly two base rigs/animation sets (mirroring P005 §3's Character
    /// 01 = Male / Character 02 = Female precedent) rather than one unique
    /// rig per character — the "Shared Animation Controller... Reusable
    /// Assets... Minimal Memory Usage" performance requirement.
    /// </summary>
    public enum CharacterGenderPresentation
    {
        Male,
        Female
    }
}

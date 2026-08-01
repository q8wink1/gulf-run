namespace GulfRun.Domain
{
    /// <summary>Ownership / availability filters exposed by the Sprint 16 Locker.</summary>
    public enum LockerOwnershipFilter
    {
        All,
        Owned,
        NotOwned,
        Temporary,
        Permanent,
        Country
    }
}

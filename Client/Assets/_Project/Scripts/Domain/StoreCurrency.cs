namespace GulfRun.Domain
{
    /// <summary>What a Store product is priced/paid in. <see cref="Free"/> covers already-unlocked showcase items (e.g. launch Characters, all unlocked per the Sprint 8 brief) that still appear in the Store for discoverability.</summary>
    public enum StoreCurrency
    {
        Gems,
        Coins,
        RealMoney,
        Free
    }
}

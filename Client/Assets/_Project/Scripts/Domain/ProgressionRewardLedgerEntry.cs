namespace GulfRun.Domain
{
    /// <summary>
    /// One Mission/Login-Reward grant of a <see cref="Domain.RewardType"/>
    /// with no dedicated inventory slot yet (ProfileFrame/ChampionEffect/
    /// Title/Badge) — the Sprint 11 counterpart to Sprint 10's
    /// <see cref="OwnedStoreItem"/>, kept in its own ledger inside
    /// <c>Core.Backend.IProgressionBackendService</c> rather than
    /// reusing <c>IStoreBackendService</c>'s ledger, since that ledger's
    /// <see cref="StoreSection"/> tag has no value that honestly describes
    /// "a Mission or Login Reward grant".
    /// </summary>
    public readonly struct ProgressionRewardLedgerEntry
    {
        public readonly string LedgerKey;
        public readonly RewardType RewardType;
        public readonly double GrantedAtSeconds;

        public ProgressionRewardLedgerEntry(string ledgerKey, RewardType rewardType, double grantedAtSeconds)
        {
            LedgerKey = ledgerKey;
            RewardType = rewardType;
            GrantedAtSeconds = grantedAtSeconds;
        }
    }
}

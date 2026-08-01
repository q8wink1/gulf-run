namespace GulfRun.Domain
{
    /// <summary>One currently-active temporary cosmetic grant — "Countdown timer displayed" (Sprint 11 brief) reads <see cref="ExpiresAtSeconds"/> off this to render remaining time. Real-world (Unix epoch) seconds throughout, matching <c>Core.Backend.LocalStoreBackendService</c>'s existing wall-clock convention for anything duration/expiry-related.</summary>
    public readonly struct TemporaryCosmeticOwnership
    {
        public readonly CosmeticId Id;
        public readonly double GrantedAtSeconds;
        public readonly double ExpiresAtSeconds;

        public TemporaryCosmeticOwnership(CosmeticId id, double grantedAtSeconds, double expiresAtSeconds)
        {
            Id = id;
            GrantedAtSeconds = grantedAtSeconds;
            ExpiresAtSeconds = expiresAtSeconds;
        }

        public double RemainingSeconds(double nowSeconds)
        {
            double remaining = ExpiresAtSeconds - nowSeconds;
            return remaining > 0d ? remaining : 0d;
        }
    }
}

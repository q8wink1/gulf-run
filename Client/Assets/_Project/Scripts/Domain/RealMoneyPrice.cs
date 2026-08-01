namespace GulfRun.Domain
{
    /// <summary>
    /// A real-money price tag — a currency code (ISO 4217, e.g. "USD") and
    /// amount. Kept as its own tiny pure struct rather than a bare float so
    /// "Future regional pricing" (Sprint 10 brief, Gem Packages section) is
    /// already representable today: a future backend can hand back a
    /// different <see cref="RealMoneyPrice"/> per storefront/region for the
    /// exact same <see cref="StoreItemId"/> with zero shape changes.
    /// </summary>
    public readonly struct RealMoneyPrice
    {
        public static readonly RealMoneyPrice Free = new RealMoneyPrice("USD", 0f);

        public readonly string CurrencyCode;
        public readonly float Amount;

        public RealMoneyPrice(string currencyCode, float amount)
        {
            CurrencyCode = string.IsNullOrEmpty(currencyCode) ? "USD" : currencyCode;
            Amount = amount;
        }

        public bool IsFree => Amount <= 0f;

        /// <summary>A simple, symbol-free "9.99 USD" presentation — real storefront-native formatting (currency symbol placement, locale grouping) is a Presentation-layer concern for later.</summary>
        public string DisplayString => Amount.ToString("F2") + " " + CurrencyCode;
    }
}

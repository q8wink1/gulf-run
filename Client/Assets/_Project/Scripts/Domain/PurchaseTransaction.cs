namespace GulfRun.Domain
{
    /// <summary>
    /// One immutable row of Purchase History — every real-money or premium-
    /// currency purchase this session (Sprint 10 brief "Purchase System":
    /// Purchase Confirmation / Purchase History / Restore Purchases /
    /// Transaction Validation / Refund Protection). Restore Purchases
    /// re-surfaces every <see cref="IsRestorable"/> entry (durable, non-
    /// consumable products — today just the Battle Pass); Refund Protection
    /// is expressed as <see cref="RefundWindowExpiresAtSeconds"/>, a fixed
    /// window after which <c>IStoreBackendService.TryRefund</c> refuses.
    /// </summary>
    public readonly struct PurchaseTransaction
    {
        public readonly string TransactionId;
        public readonly string ProductId;
        public readonly StoreSection Section;
        public readonly string PriceDisplay;
        public readonly double TimestampSeconds;
        public readonly PurchaseResult Result;
        public readonly bool IsRestorable;
        public readonly double RefundWindowExpiresAtSeconds;

        public PurchaseTransaction(string transactionId, string productId, StoreSection section, string priceDisplay, double timestampSeconds, PurchaseResult result, bool isRestorable, double refundWindowExpiresAtSeconds)
        {
            TransactionId = transactionId;
            ProductId = productId;
            Section = section;
            PriceDisplay = priceDisplay;
            TimestampSeconds = timestampSeconds;
            Result = result;
            IsRestorable = isRestorable;
            RefundWindowExpiresAtSeconds = refundWindowExpiresAtSeconds;
        }
    }
}

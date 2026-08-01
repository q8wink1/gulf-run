namespace GulfRun.Domain
{
    /// <summary>Outcome of a Purchase Confirmation / Transaction Validation attempt (Sprint 10 brief "Purchase System").</summary>
    public enum PurchaseResult
    {
        Success,
        InsufficientFunds,
        AlreadyOwned,
        ValidationFailed,
        Cancelled,
        RefundIssued
    }
}

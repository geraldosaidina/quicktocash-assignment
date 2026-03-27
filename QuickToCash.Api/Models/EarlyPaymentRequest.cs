namespace QuickToCash.Api.Models;

public class EarlyPaymentRequest
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public decimal RequestedAmount { get; set; }
    public DateTime RequestedAtUtc { get; set; }
    public EarlyPaymentRequestStatus Status { get; set; } = EarlyPaymentRequestStatus.Pending;
}

public enum EarlyPaymentRequestStatus
{
    Pending = 1,
    Accepted = 2,
    Rejected = 3
}

namespace QuickToCash.Api.Models;

public class EarlyPaymentRequest
{
    public string RequestId { get; set; } = string.Empty;
    public string InvoiceId { get; set; } = string.Empty;
    public DateTime RequestedDate { get; set; }
    public decimal DisbursementAmount { get; set; }
    public decimal Fee { get; set; }
    public EarlyPaymentRequestStatus Status { get; set; } = EarlyPaymentRequestStatus.Pending;
}

public enum EarlyPaymentRequestStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}

namespace QuickToCash.Api.DTOs;

public class EarlyPaymentRequestDto
{
    public string RequestId { get; init; } = string.Empty;
    public string InvoiceId { get; init; } = string.Empty;
    public DateTime RequestedDate { get; init; }
    public decimal DisbursementAmount { get; init; }
    public decimal Fee { get; init; }
    public string Status { get; init; } = string.Empty;
}

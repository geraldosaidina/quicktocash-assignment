namespace QuickToCash.Api.DTOs;

public class EarlyPaymentRequestDto
{
    public Guid Id { get; init; }
    public Guid InvoiceId { get; init; }
    public decimal RequestedAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime RequestedAtUtc { get; init; }
}

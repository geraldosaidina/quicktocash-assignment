namespace QuickToCash.Api.DTOs;

public class EarlyPaymentEligibilityDto
{
    public Guid InvoiceId { get; init; }
    public bool IsEligible { get; init; }
    public decimal MaxRequestAmount { get; init; }
    public string Reason { get; init; } = string.Empty;
}

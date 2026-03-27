namespace QuickToCash.Api.DTOs;

public class EarlyPaymentEligibilityDto
{
    public string InvoiceId { get; init; } = string.Empty;
    public bool IsEligible { get; init; }
    public decimal MaxDisbursementAmount { get; init; }
    public string Reason { get; init; } = string.Empty;
}

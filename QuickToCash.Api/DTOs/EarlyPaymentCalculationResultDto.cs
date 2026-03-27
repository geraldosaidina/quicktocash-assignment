namespace QuickToCash.Api.DTOs;

public class EarlyPaymentCalculationResultDto
{
    public bool IsEligible { get; init; }
    public decimal Fee { get; init; }
    public decimal DisbursementAmount { get; init; }
    public int EarlyByDays { get; init; }
    public string Reason { get; init; } = string.Empty;
}

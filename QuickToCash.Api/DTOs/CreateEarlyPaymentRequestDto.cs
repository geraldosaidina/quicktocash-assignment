using System.ComponentModel.DataAnnotations;

namespace QuickToCash.Api.DTOs;

public class CreateEarlyPaymentRequestDto
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Disbursement amount must be greater than zero.")]
    public decimal DisbursementAmount { get; init; }
}

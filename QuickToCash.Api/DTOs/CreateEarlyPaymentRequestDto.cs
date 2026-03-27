using System.ComponentModel.DataAnnotations;

namespace QuickToCash.Api.DTOs;

public class CreateEarlyPaymentRequestDto
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Requested amount must be greater than zero.")]
    public decimal RequestedAmount { get; init; }
}

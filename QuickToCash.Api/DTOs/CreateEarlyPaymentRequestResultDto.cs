namespace QuickToCash.Api.DTOs;

public enum CreateEarlyPaymentRequestOutcome
{
    Created = 1,
    InvoiceNotFound = 2,
    DuplicateRequest = 3,
    NotEligible = 4
}

public class CreateEarlyPaymentRequestResultDto
{
    public CreateEarlyPaymentRequestOutcome Outcome { get; init; }
    public string Message { get; init; } = string.Empty;
    public IEnumerable<string> Errors { get; init; } = Array.Empty<string>();
    public EarlyPaymentRequestDto? Request { get; init; }
}

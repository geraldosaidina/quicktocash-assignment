namespace QuickToCash.Api.DTOs;

public class InvoiceDto
{
    public Guid Id { get; init; }
    public string SupplierId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public decimal AmountPaid { get; init; }
    public decimal AmountOutstanding { get; init; }
    public DateTime DueDateUtc { get; init; }
    public string Status { get; init; } = string.Empty;
}

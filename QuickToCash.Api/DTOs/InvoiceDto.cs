namespace QuickToCash.Api.DTOs;

public class InvoiceDto
{
    public string InvoiceId { get; init; } = string.Empty;
    public string InvoiceNumber { get; init; } = string.Empty;
    public string SupplierName { get; init; } = string.Empty;
    public string SupplierId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime SubmittedDate { get; init; }
    public DateTime DueDate { get; init; }
    public string Status { get; init; } = string.Empty;
}

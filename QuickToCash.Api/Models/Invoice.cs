namespace QuickToCash.Api.Models;

public class Invoice
{
    public string InvoiceId { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime DueDate { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
}

public enum InvoiceStatus
{
    Pending = 1,
    Approved = 2,
    Funded = 3,
    Rejected = 4
}

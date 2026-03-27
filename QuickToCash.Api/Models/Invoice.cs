namespace QuickToCash.Api.Models;

public class Invoice
{
    public Guid Id { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime DueDateUtc { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.PendingApproval;
}

public enum InvoiceStatus
{
    PendingApproval = 1,
    Approved = 2,
    Paid = 3
}

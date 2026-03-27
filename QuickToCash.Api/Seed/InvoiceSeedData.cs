using QuickToCash.Api.Models;

namespace QuickToCash.Api.Seed;

public static class InvoiceSeedData
{
    public static List<Invoice> CreateInvoices()
    {
        return new List<Invoice>
        {
            new()
            {
                InvoiceId = "INV-1001",
                InvoiceNumber = "QTC-2026-0001",
                SupplierName = "Acme Manufacturing",
                SupplierId = "SUP-100",
                Amount = 10000m,
                SubmittedDate = DateTime.UtcNow.Date.AddDays(-6),
                DueDate = DateTime.UtcNow.Date.AddDays(14),
                Status = InvoiceStatus.Approved
            },
            new()
            {
                InvoiceId = "INV-1002",
                InvoiceNumber = "QTC-2026-0002",
                SupplierName = "Acme Manufacturing",
                SupplierId = "SUP-100",
                Amount = 7500m,
                SubmittedDate = DateTime.UtcNow.Date.AddDays(-12),
                DueDate = DateTime.UtcNow.Date.AddDays(7),
                Status = InvoiceStatus.Pending
            },
            new()
            {
                InvoiceId = "INV-2001",
                InvoiceNumber = "QTC-2026-0003",
                SupplierName = "Global Parts Ltd",
                SupplierId = "SUP-200",
                Amount = 12000m,
                SubmittedDate = DateTime.UtcNow.Date.AddDays(-18),
                DueDate = DateTime.UtcNow.Date.AddDays(-5),
                Status = InvoiceStatus.Funded
            }
        };
    }
}

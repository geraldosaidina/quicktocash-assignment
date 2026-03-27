using QuickToCash.Api.Models;

namespace QuickToCash.Api.Seed;

public static class InvoiceSeedData
{
    public static List<Invoice> CreateInvoices()
    {
        return new List<Invoice>
        {
            // Eligible: Approved, due in future, no pending early payment request
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
            // Not eligible: Pending status
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
            // Not eligible: Already funded
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
            },
            // Not eligible: Rejected status
            new()
            {
                InvoiceId = "INV-2002",
                InvoiceNumber = "QTC-2026-0004",
                SupplierName = "Global Parts Ltd",
                SupplierId = "SUP-200",
                Amount = 5600m,
                SubmittedDate = DateTime.UtcNow.Date.AddDays(-10),
                DueDate = DateTime.UtcNow.Date.AddDays(9),
                Status = InvoiceStatus.Rejected
            },
            // Not eligible: Approved but overdue
            new()
            {
                InvoiceId = "INV-3001",
                InvoiceNumber = "QTC-2026-0005",
                SupplierName = "Northwind Components",
                SupplierId = "SUP-300",
                Amount = 9800m,
                SubmittedDate = DateTime.UtcNow.Date.AddDays(-30),
                DueDate = DateTime.UtcNow.Date.AddDays(-1),
                Status = InvoiceStatus.Approved
            },
            // Not eligible: Approved and due in future but has pending early payment request
            new()
            {
                InvoiceId = "INV-3002",
                InvoiceNumber = "QTC-2026-0006",
                SupplierName = "Northwind Components",
                SupplierId = "SUP-300",
                Amount = 14350m,
                SubmittedDate = DateTime.UtcNow.Date.AddDays(-5),
                DueDate = DateTime.UtcNow.Date.AddDays(18),
                Status = InvoiceStatus.Approved
            }
        };
    }
}

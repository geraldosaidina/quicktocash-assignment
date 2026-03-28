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
            },
            // Eligible: Approved, longer due horizon
            new()
            {
                InvoiceId = "INV-1003",
                InvoiceNumber = "QTC-2026-0007",
                SupplierName = "Acme Manufacturing",
                SupplierId = "SUP-100",
                Amount = 21500m,
                SubmittedDate = DateTime.UtcNow.Date.AddDays(-8),
                DueDate = DateTime.UtcNow.Date.AddDays(28),
                Status = InvoiceStatus.Approved
            },
            // Not eligible: Funded status
            new()
            {
                InvoiceId = "INV-1004",
                InvoiceNumber = "QTC-2026-0008",
                SupplierName = "Acme Manufacturing",
                SupplierId = "SUP-100",
                Amount = 8300m,
                SubmittedDate = DateTime.UtcNow.Date.AddDays(-21),
                DueDate = DateTime.UtcNow.Date.AddDays(-2),
                Status = InvoiceStatus.Funded
            },
            // Eligible: New supplier with Approved invoice
            new()
            {
                InvoiceId = "INV-1011",
                InvoiceNumber = "QTC-2026-0009",
                SupplierName = "Lusofin Supplies",
                SupplierId = "SUP-101",
                Amount = 18750m,
                SubmittedDate = DateTime.UtcNow.Date.AddDays(-6),
                DueDate = DateTime.UtcNow.Date.AddDays(19),
                Status = InvoiceStatus.Approved
            },
            // Not eligible: New supplier with Pending invoice
            new()
            {
                InvoiceId = "INV-1012",
                InvoiceNumber = "QTC-2026-0010",
                SupplierName = "Lusofin Supplies",
                SupplierId = "SUP-101",
                Amount = 5400m,
                SubmittedDate = DateTime.UtcNow.Date.AddDays(-2),
                DueDate = DateTime.UtcNow.Date.AddDays(16),
                Status = InvoiceStatus.Pending
            },
            // Eligible: Another approved invoice for manual POST testing
            new()
            {
                InvoiceId = "INV-2003",
                InvoiceNumber = "QTC-2026-0011",
                SupplierName = "Global Parts Ltd",
                SupplierId = "SUP-200",
                Amount = 9200m,
                SubmittedDate = DateTime.UtcNow.Date.AddDays(-4),
                DueDate = DateTime.UtcNow.Date.AddDays(17),
                Status = InvoiceStatus.Approved
            },
            // Not eligible: Approved but due in <= 5 days
            new()
            {
                InvoiceId = "INV-3003",
                InvoiceNumber = "QTC-2026-0012",
                SupplierName = "Northwind Components",
                SupplierId = "SUP-300",
                Amount = 11100m,
                SubmittedDate = DateTime.UtcNow.Date.AddDays(-7),
                DueDate = DateTime.UtcNow.Date.AddDays(4),
                Status = InvoiceStatus.Approved
            },
            // Not eligible: Rejected status
            new()
            {
                InvoiceId = "INV-4001",
                InvoiceNumber = "QTC-2026-0013",
                SupplierName = "Maputo Industrial Goods",
                SupplierId = "SUP-400",
                Amount = 6300m,
                SubmittedDate = DateTime.UtcNow.Date.AddDays(-13),
                DueDate = DateTime.UtcNow.Date.AddDays(11),
                Status = InvoiceStatus.Rejected
            },
            // Eligible: Additional supplier with approved invoice
            new()
            {
                InvoiceId = "INV-4002",
                InvoiceNumber = "QTC-2026-0014",
                SupplierName = "Maputo Industrial Goods",
                SupplierId = "SUP-400",
                Amount = 14400m,
                SubmittedDate = DateTime.UtcNow.Date.AddDays(-9),
                DueDate = DateTime.UtcNow.Date.AddDays(35),
                Status = InvoiceStatus.Approved
            }
        };
    }
}

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
                Id = Guid.Parse("e83695e5-4cbf-4a0f-9f2c-69d878ecf100"),
                SupplierId = "SUP-100",
                Amount = 10000m,
                AmountPaid = 0m,
                DueDateUtc = DateTime.UtcNow.Date.AddDays(14),
                Status = InvoiceStatus.Approved
            },
            new()
            {
                Id = Guid.Parse("9755ad67-0322-4574-8eb6-bf5976ffea01"),
                SupplierId = "SUP-100",
                Amount = 7500m,
                AmountPaid = 2500m,
                DueDateUtc = DateTime.UtcNow.Date.AddDays(7),
                Status = InvoiceStatus.Approved
            },
            new()
            {
                Id = Guid.Parse("611c6f7a-7f4b-4d4d-a96c-70c53c04df02"),
                SupplierId = "SUP-200",
                Amount = 12000m,
                AmountPaid = 12000m,
                DueDateUtc = DateTime.UtcNow.Date.AddDays(-5),
                Status = InvoiceStatus.Paid
            }
        };
    }
}

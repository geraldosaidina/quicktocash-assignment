using QuickToCash.Api.Models;
using QuickToCash.Api.Repositories.Interfaces;
using QuickToCash.Api.Seed;

namespace QuickToCash.Api.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly List<Invoice> _invoices;

    public InvoiceRepository()
    {
        _invoices = InvoiceSeedData.CreateInvoices();
    }

    public IReadOnlyCollection<Invoice> GetBySupplierId(string supplierId)
    {
        return _invoices
            .Where(i => i.SupplierId.Equals(supplierId, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    public Invoice? GetById(string invoiceId)
    {
        return _invoices.FirstOrDefault(i => i.InvoiceId.Equals(invoiceId, StringComparison.OrdinalIgnoreCase));
    }
}

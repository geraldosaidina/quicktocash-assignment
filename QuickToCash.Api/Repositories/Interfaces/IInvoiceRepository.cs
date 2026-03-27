using QuickToCash.Api.Models;

namespace QuickToCash.Api.Repositories.Interfaces;

public interface IInvoiceRepository
{
    IReadOnlyCollection<Invoice> GetBySupplierId(string supplierId);
    Invoice? GetById(Guid invoiceId);
}

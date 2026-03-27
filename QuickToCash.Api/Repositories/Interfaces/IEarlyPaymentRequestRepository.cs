using QuickToCash.Api.Models;

namespace QuickToCash.Api.Repositories.Interfaces;

public interface IEarlyPaymentRequestRepository
{
    EarlyPaymentRequest Add(EarlyPaymentRequest request);
    IReadOnlyCollection<EarlyPaymentRequest> GetByInvoiceId(string invoiceId);
}

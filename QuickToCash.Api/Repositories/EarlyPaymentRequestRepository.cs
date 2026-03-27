using QuickToCash.Api.Models;
using QuickToCash.Api.Repositories.Interfaces;

namespace QuickToCash.Api.Repositories;

public class EarlyPaymentRequestRepository : IEarlyPaymentRequestRepository
{
    private readonly List<EarlyPaymentRequest> _requests = new();

    public EarlyPaymentRequest Add(EarlyPaymentRequest request)
    {
        _requests.Add(request);
        return request;
    }

    public IReadOnlyCollection<EarlyPaymentRequest> GetByInvoiceId(Guid invoiceId)
    {
        return _requests.Where(r => r.InvoiceId == invoiceId).ToArray();
    }
}

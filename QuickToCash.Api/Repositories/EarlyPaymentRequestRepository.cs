using QuickToCash.Api.Models;
using QuickToCash.Api.Repositories.Interfaces;
using QuickToCash.Api.Seed;

namespace QuickToCash.Api.Repositories;

public class EarlyPaymentRequestRepository : IEarlyPaymentRequestRepository
{
    private readonly List<EarlyPaymentRequest> _requests;

    public EarlyPaymentRequestRepository()
    {
        _requests = EarlyPaymentRequestSeedData.CreateRequests();
    }

    public EarlyPaymentRequest Add(EarlyPaymentRequest request)
    {
        _requests.Add(request);
        return request;
    }

    public IReadOnlyCollection<EarlyPaymentRequest> GetByInvoiceId(string invoiceId)
    {
        return _requests
            .Where(r => r.InvoiceId.Equals(invoiceId, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }
}

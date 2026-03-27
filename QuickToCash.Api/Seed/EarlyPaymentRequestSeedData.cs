using QuickToCash.Api.Models;

namespace QuickToCash.Api.Seed;

public static class EarlyPaymentRequestSeedData
{
    public static List<EarlyPaymentRequest> CreateRequests()
    {
        return new List<EarlyPaymentRequest>
        {
            new()
            {
                RequestId = "REQ-9001",
                InvoiceId = "INV-3002",
                RequestedDate = DateTime.UtcNow.Date.AddDays(-1),
                DisbursementAmount = 10000m,
                Fee = 200m,
                Status = EarlyPaymentRequestStatus.Pending
            },
            new()
            {
                RequestId = "REQ-9002",
                InvoiceId = "INV-2001",
                RequestedDate = DateTime.UtcNow.Date.AddDays(-15),
                DisbursementAmount = 12000m,
                Fee = 240m,
                Status = EarlyPaymentRequestStatus.Approved
            }
        };
    }
}

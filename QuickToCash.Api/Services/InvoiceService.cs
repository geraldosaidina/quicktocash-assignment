using QuickToCash.Api.DTOs;
using QuickToCash.Api.Models;
using QuickToCash.Api.Repositories.Interfaces;
using QuickToCash.Api.Services.Interfaces;

namespace QuickToCash.Api.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IEarlyPaymentRequestRepository _earlyPaymentRequestRepository;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IEarlyPaymentRequestRepository earlyPaymentRequestRepository)
    {
        _invoiceRepository = invoiceRepository;
        _earlyPaymentRequestRepository = earlyPaymentRequestRepository;
    }

    public IReadOnlyCollection<InvoiceDto> GetInvoicesBySupplier(string supplierId)
    {
        return _invoiceRepository
            .GetBySupplierId(supplierId)
            .Select(MapInvoice)
            .ToArray();
    }

    public InvoiceDto? GetInvoiceById(string invoiceId)
    {
        var invoice = _invoiceRepository.GetById(invoiceId);
        return invoice is null ? null : MapInvoice(invoice);
    }

    public EarlyPaymentEligibilityDto? GetEarlyPaymentEligibility(string invoiceId)
    {
        var invoice = _invoiceRepository.GetById(invoiceId);
        if (invoice is null)
        {
            return null;
        }

        var hasPendingRequest = _earlyPaymentRequestRepository
            .GetByInvoiceId(invoiceId)
            .Any(r => r.Status == EarlyPaymentRequestStatus.Pending);

        var isEligible =
            invoice.Status == InvoiceStatus.Approved &&
            invoice.DueDate > DateTime.UtcNow &&
            !hasPendingRequest;

        var reason = isEligible
            ? "Invoice is eligible for early payment."
            : "Invoice must be approved, not overdue, and without a pending request.";

        return new EarlyPaymentEligibilityDto
        {
            InvoiceId = invoiceId,
            IsEligible = isEligible,
            MaxDisbursementAmount = invoice.Amount,
            Reason = reason
        };
    }

    public (bool Success, string Message, IEnumerable<string> Errors, EarlyPaymentRequestDto? Request) CreateEarlyPaymentRequest(
        string invoiceId,
        CreateEarlyPaymentRequestDto payload)
    {
        var eligibility = GetEarlyPaymentEligibility(invoiceId);
        if (eligibility is null)
        {
            return (false, "Invoice not found.", new[] { "Invalid invoice id." }, null);
        }

        if (!eligibility.IsEligible)
        {
            return (false, "Invoice is not eligible for early payment.", new[] { eligibility.Reason }, null);
        }

        if (payload.DisbursementAmount > eligibility.MaxDisbursementAmount)
        {
            return (false, "Disbursement amount exceeds invoice amount.", new[] { "Disbursement amount is too high." }, null);
        }

        var fee = decimal.Round(payload.DisbursementAmount * 0.02m, 2, MidpointRounding.AwayFromZero);

        var request = _earlyPaymentRequestRepository.Add(new EarlyPaymentRequest
        {
            RequestId = Guid.NewGuid().ToString("N"),
            InvoiceId = invoiceId,
            RequestedDate = DateTime.UtcNow,
            DisbursementAmount = payload.DisbursementAmount,
            Fee = fee,
            Status = EarlyPaymentRequestStatus.Pending
        });

        return (true, "Early payment request created.", Array.Empty<string>(), MapEarlyPaymentRequest(request));
    }

    private static InvoiceDto MapInvoice(Invoice invoice)
    {
        return new InvoiceDto
        {
            InvoiceId = invoice.InvoiceId,
            InvoiceNumber = invoice.InvoiceNumber,
            SupplierName = invoice.SupplierName,
            SupplierId = invoice.SupplierId,
            Amount = invoice.Amount,
            SubmittedDate = invoice.SubmittedDate,
            DueDate = invoice.DueDate,
            Status = invoice.Status.ToString()
        };
    }

    private static EarlyPaymentRequestDto MapEarlyPaymentRequest(EarlyPaymentRequest request)
    {
        return new EarlyPaymentRequestDto
        {
            RequestId = request.RequestId,
            InvoiceId = request.InvoiceId,
            RequestedDate = request.RequestedDate,
            DisbursementAmount = request.DisbursementAmount,
            Fee = request.Fee,
            Status = request.Status.ToString()
        };
    }
}

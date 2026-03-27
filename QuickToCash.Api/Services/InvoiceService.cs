using QuickToCash.Api.DTOs;
using QuickToCash.Api.Models;
using QuickToCash.Api.Repositories.Interfaces;
using QuickToCash.Api.Services.Interfaces;

namespace QuickToCash.Api.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IEarlyPaymentRequestRepository _earlyPaymentRequestRepository;
    private readonly IEarlyPaymentService _earlyPaymentService;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IEarlyPaymentRequestRepository earlyPaymentRequestRepository,
        IEarlyPaymentService earlyPaymentService)
    {
        _invoiceRepository = invoiceRepository;
        _earlyPaymentRequestRepository = earlyPaymentRequestRepository;
        _earlyPaymentService = earlyPaymentService;
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

        var calculation = _earlyPaymentService.Evaluate(invoice);
        var hasPendingRequest = _earlyPaymentRequestRepository
            .GetByInvoiceId(invoiceId)
            .Any(r => r.Status == EarlyPaymentRequestStatus.Pending);
        var isEligible = calculation.IsEligible && !hasPendingRequest;
        var reason = hasPendingRequest
            ? "Invoice already has a pending early payment request."
            : calculation.Reason;

        return new EarlyPaymentEligibilityDto
        {
            InvoiceId = invoiceId,
            IsEligible = isEligible,
            Fee = isEligible ? calculation.Fee : 0m,
            DisbursementAmount = isEligible ? calculation.DisbursementAmount : 0m,
            EarlyByDays = calculation.EarlyByDays,
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

        if (payload.DisbursementAmount > eligibility.DisbursementAmount)
        {
            return (false, "Disbursement amount exceeds eligible amount.", new[] { "Disbursement amount is too high." }, null);
        }

        var invoice = _invoiceRepository.GetById(invoiceId);
        if (invoice is null)
        {
            return (false, "Invoice not found.", new[] { "Invalid invoice id." }, null);
        }

        var calculated = _earlyPaymentService.Evaluate(invoice);
        var fee = calculated.Fee;

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

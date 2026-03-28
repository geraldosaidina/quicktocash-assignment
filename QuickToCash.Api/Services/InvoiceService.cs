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
    private readonly IDateTimeProvider _dateTimeProvider;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IEarlyPaymentRequestRepository earlyPaymentRequestRepository,
        IEarlyPaymentService earlyPaymentService,
        IDateTimeProvider dateTimeProvider)
    {
        _invoiceRepository = invoiceRepository;
        _earlyPaymentRequestRepository = earlyPaymentRequestRepository;
        _earlyPaymentService = earlyPaymentService;
        _dateTimeProvider = dateTimeProvider;
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

    public CreateEarlyPaymentRequestResultDto CreateEarlyPaymentRequest(
        string invoiceId,
        CreateEarlyPaymentRequestDto payload)
    {
        var invoice = _invoiceRepository.GetById(invoiceId);
        if (invoice is null)
        {
            return new CreateEarlyPaymentRequestResultDto
            {
                Outcome = CreateEarlyPaymentRequestOutcome.InvoiceNotFound,
                Message = "Invoice not found.",
                Errors = new[] { "Invalid invoice id." }
            };
        }

        if (_earlyPaymentRequestRepository.HasPendingRequestForInvoiceId(invoiceId))
        {
            return new CreateEarlyPaymentRequestResultDto
            {
                Outcome = CreateEarlyPaymentRequestOutcome.DuplicateRequest,
                Message = "A pending early payment request already exists for this invoice.",
                Errors = new[] { "Duplicate request." }
            };
        }

        var calculation = _earlyPaymentService.Evaluate(invoice);
        if (!calculation.IsEligible)
        {
            return new CreateEarlyPaymentRequestResultDto
            {
                Outcome = CreateEarlyPaymentRequestOutcome.NotEligible,
                Message = "Invoice is not eligible for early payment.",
                Errors = new[] { calculation.Reason }
            };
        }

        if (payload.DisbursementAmount != calculation.DisbursementAmount)
        {
            return new CreateEarlyPaymentRequestResultDto
            {
                Outcome = CreateEarlyPaymentRequestOutcome.NotEligible,
                Message = "Invoice is not eligible for early payment.",
                Errors = new[]
                {
                    $"Disbursement amount must equal the calculated value ({calculation.DisbursementAmount:0.00})."
                }
            };
        }

        var request = _earlyPaymentRequestRepository.Add(new EarlyPaymentRequest
        {
            RequestId = Guid.NewGuid().ToString("N"),
            InvoiceId = invoiceId,
            RequestedDate = _dateTimeProvider.UtcNow,
            DisbursementAmount = calculation.DisbursementAmount,
            Fee = calculation.Fee,
            Status = EarlyPaymentRequestStatus.Pending
        });

        return new CreateEarlyPaymentRequestResultDto
        {
            Outcome = CreateEarlyPaymentRequestOutcome.Created,
            Message = "Early payment request created.",
            Request = MapEarlyPaymentRequest(request),
            Errors = Array.Empty<string>()
        };
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

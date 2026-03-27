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

    public InvoiceDto? GetInvoiceById(Guid invoiceId)
    {
        var invoice = _invoiceRepository.GetById(invoiceId);
        return invoice is null ? null : MapInvoice(invoice);
    }

    public EarlyPaymentEligibilityDto? GetEarlyPaymentEligibility(Guid invoiceId)
    {
        var invoice = _invoiceRepository.GetById(invoiceId);
        if (invoice is null)
        {
            return null;
        }

        var amountOutstanding = invoice.Amount - invoice.AmountPaid;
        var hasPendingRequest = _earlyPaymentRequestRepository
            .GetByInvoiceId(invoiceId)
            .Any(r => r.Status == EarlyPaymentRequestStatus.Pending);

        var isEligible =
            invoice.Status == InvoiceStatus.Approved &&
            invoice.DueDateUtc > DateTime.UtcNow &&
            amountOutstanding > 0 &&
            !hasPendingRequest;

        var reason = isEligible
            ? "Invoice is eligible for early payment."
            : "Invoice must be approved, unpaid, not overdue, and without a pending request.";

        return new EarlyPaymentEligibilityDto
        {
            InvoiceId = invoiceId,
            IsEligible = isEligible,
            MaxRequestAmount = amountOutstanding > 0 ? amountOutstanding : 0,
            Reason = reason
        };
    }

    public (bool Success, string Message, IEnumerable<string> Errors, EarlyPaymentRequestDto? Request) CreateEarlyPaymentRequest(
        Guid invoiceId,
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

        if (payload.RequestedAmount > eligibility.MaxRequestAmount)
        {
            return (false, "Requested amount exceeds outstanding amount.", new[] { "Requested amount is too high." }, null);
        }

        var request = _earlyPaymentRequestRepository.Add(new EarlyPaymentRequest
        {
            Id = Guid.NewGuid(),
            InvoiceId = invoiceId,
            RequestedAmount = payload.RequestedAmount,
            RequestedAtUtc = DateTime.UtcNow,
            Status = EarlyPaymentRequestStatus.Pending
        });

        return (true, "Early payment request created.", Array.Empty<string>(), MapEarlyPaymentRequest(request));
    }

    private static InvoiceDto MapInvoice(Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            SupplierId = invoice.SupplierId,
            Amount = invoice.Amount,
            AmountPaid = invoice.AmountPaid,
            AmountOutstanding = invoice.Amount - invoice.AmountPaid,
            DueDateUtc = invoice.DueDateUtc,
            Status = invoice.Status.ToString()
        };
    }

    private static EarlyPaymentRequestDto MapEarlyPaymentRequest(EarlyPaymentRequest request)
    {
        return new EarlyPaymentRequestDto
        {
            Id = request.Id,
            InvoiceId = request.InvoiceId,
            RequestedAmount = request.RequestedAmount,
            RequestedAtUtc = request.RequestedAtUtc,
            Status = request.Status.ToString()
        };
    }
}

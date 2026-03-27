using QuickToCash.Api.DTOs;

namespace QuickToCash.Api.Services.Interfaces;

public interface IInvoiceService
{
    IReadOnlyCollection<InvoiceDto> GetInvoicesBySupplier(string supplierId);
    InvoiceDto? GetInvoiceById(Guid invoiceId);
    EarlyPaymentEligibilityDto? GetEarlyPaymentEligibility(Guid invoiceId);
    (bool Success, string Message, IEnumerable<string> Errors, EarlyPaymentRequestDto? Request) CreateEarlyPaymentRequest(
        Guid invoiceId,
        CreateEarlyPaymentRequestDto payload);
}

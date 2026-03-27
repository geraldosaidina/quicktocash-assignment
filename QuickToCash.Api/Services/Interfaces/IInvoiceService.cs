using QuickToCash.Api.DTOs;

namespace QuickToCash.Api.Services.Interfaces;

public interface IInvoiceService
{
    IReadOnlyCollection<InvoiceDto> GetInvoicesBySupplier(string supplierId);
    InvoiceDto? GetInvoiceById(string invoiceId);
    EarlyPaymentEligibilityDto? GetEarlyPaymentEligibility(string invoiceId);
    (bool Success, string Message, IEnumerable<string> Errors, EarlyPaymentRequestDto? Request) CreateEarlyPaymentRequest(
        string invoiceId,
        CreateEarlyPaymentRequestDto payload);
}

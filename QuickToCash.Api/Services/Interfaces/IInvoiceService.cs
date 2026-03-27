using QuickToCash.Api.DTOs;

namespace QuickToCash.Api.Services.Interfaces;

public interface IInvoiceService
{
    IReadOnlyCollection<InvoiceDto> GetInvoicesBySupplier(string supplierId);
    InvoiceDto? GetInvoiceById(string invoiceId);
    EarlyPaymentEligibilityDto? GetEarlyPaymentEligibility(string invoiceId);
    CreateEarlyPaymentRequestResultDto CreateEarlyPaymentRequest(string invoiceId, CreateEarlyPaymentRequestDto payload);
}

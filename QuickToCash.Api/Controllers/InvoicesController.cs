using Microsoft.AspNetCore.Mvc;
using QuickToCash.Api.Common;
using QuickToCash.Api.DTOs;
using QuickToCash.Api.Services.Interfaces;

namespace QuickToCash.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public ActionResult<ApiResponse<IReadOnlyCollection<InvoiceDto>>> GetInvoices([FromQuery] string supplierId)
    {
        if (string.IsNullOrWhiteSpace(supplierId))
        {
            return BadRequest(ApiResponse<IReadOnlyCollection<InvoiceDto>>.Fail(
                "supplierId is required.",
                new[] { "Query parameter supplierId is missing." }));
        }

        var invoices = _invoiceService.GetInvoicesBySupplier(supplierId);
        return Ok(ApiResponse<IReadOnlyCollection<InvoiceDto>>.Ok(invoices));
    }

    [HttpGet("{id:guid}")]
    public ActionResult<ApiResponse<InvoiceDto>> GetInvoiceById(Guid id)
    {
        var invoice = _invoiceService.GetInvoiceById(id);
        if (invoice is null)
        {
            return NotFound(ApiResponse<InvoiceDto>.Fail("Invoice not found.", new[] { "Invalid invoice id." }));
        }

        return Ok(ApiResponse<InvoiceDto>.Ok(invoice));
    }

    [HttpGet("{id:guid}/early-payment-eligibility")]
    public ActionResult<ApiResponse<EarlyPaymentEligibilityDto>> GetEarlyPaymentEligibility(Guid id)
    {
        var eligibility = _invoiceService.GetEarlyPaymentEligibility(id);
        if (eligibility is null)
        {
            return NotFound(ApiResponse<EarlyPaymentEligibilityDto>.Fail(
                "Invoice not found.",
                new[] { "Invalid invoice id." }));
        }

        return Ok(ApiResponse<EarlyPaymentEligibilityDto>.Ok(eligibility));
    }

    [HttpPost("{id:guid}/early-payment-request")]
    public ActionResult<ApiResponse<EarlyPaymentRequestDto>> CreateEarlyPaymentRequest(
        Guid id,
        [FromBody] CreateEarlyPaymentRequestDto payload)
    {
        var result = _invoiceService.CreateEarlyPaymentRequest(id, payload);
        if (!result.Success || result.Request is null)
        {
            return BadRequest(ApiResponse<EarlyPaymentRequestDto>.Fail(result.Message, result.Errors));
        }

        return Ok(ApiResponse<EarlyPaymentRequestDto>.Ok(result.Request, result.Message));
    }
}

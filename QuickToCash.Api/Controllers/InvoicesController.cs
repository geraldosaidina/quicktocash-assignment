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

        var invoices = _invoiceService.GetInvoicesBySupplier(supplierId.Trim());
        return Ok(ApiResponse<IReadOnlyCollection<InvoiceDto>>.Ok(invoices));
    }

    [HttpGet("{id}")]
    public ActionResult<ApiResponse<InvoiceDto>> GetInvoiceById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(ApiResponse<InvoiceDto>.Fail(
                "Invoice id is required.",
                new[] { "Route parameter id is missing." }));
        }

        var invoice = _invoiceService.GetInvoiceById(id.Trim());
        if (invoice is null)
        {
            return NotFound(ApiResponse<InvoiceDto>.Fail("Invoice not found.", new[] { "Invalid invoice id." }));
        }

        return Ok(ApiResponse<InvoiceDto>.Ok(invoice));
    }

    [HttpGet("{id}/early-payment-eligibility")]
    public ActionResult<ApiResponse<EarlyPaymentEligibilityDto>> GetEarlyPaymentEligibility(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(ApiResponse<EarlyPaymentEligibilityDto>.Fail(
                "Invoice id is required.",
                new[] { "Route parameter id is missing." }));
        }

        var eligibility = _invoiceService.GetEarlyPaymentEligibility(id.Trim());
        if (eligibility is null)
        {
            return NotFound(ApiResponse<EarlyPaymentEligibilityDto>.Fail(
                "Invoice not found.",
                new[] { "Invalid invoice id." }));
        }

        return Ok(ApiResponse<EarlyPaymentEligibilityDto>.Ok(eligibility));
    }

    [HttpPost("{id}/early-payment-request")]
    public ActionResult<ApiResponse<EarlyPaymentRequestDto>> CreateEarlyPaymentRequest(
        string id,
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

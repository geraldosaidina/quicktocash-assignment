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
            return BadRequestResponse<IReadOnlyCollection<InvoiceDto>>(
                "supplierId is required.",
                "Query parameter supplierId is missing.");
        }

        var invoices = _invoiceService.GetInvoicesBySupplier(supplierId.Trim());
        return Ok(ApiResponse<IReadOnlyCollection<InvoiceDto>>.Ok(invoices));
    }

    [HttpGet("{id}")]
    public ActionResult<ApiResponse<InvoiceDto>> GetInvoiceById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequestResponse<InvoiceDto>(
                "Invoice id is required.",
                "Route parameter id is missing.");
        }

        var invoice = _invoiceService.GetInvoiceById(id.Trim());
        if (invoice is null)
        {
            return NotFoundResponse<InvoiceDto>("Invoice not found.", "Invalid invoice id.");
        }

        return Ok(ApiResponse<InvoiceDto>.Ok(invoice));
    }

    [HttpGet("{id}/early-payment-eligibility")]
    public ActionResult<ApiResponse<EarlyPaymentEligibilityDto>> GetEarlyPaymentEligibility(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequestResponse<EarlyPaymentEligibilityDto>(
                "Invoice id is required.",
                "Route parameter id is missing.");
        }

        var eligibility = _invoiceService.GetEarlyPaymentEligibility(id.Trim());
        if (eligibility is null)
        {
            return NotFoundResponse<EarlyPaymentEligibilityDto>(
                "Invoice not found.",
                "Invalid invoice id.");
        }

        return Ok(ApiResponse<EarlyPaymentEligibilityDto>.Ok(eligibility));
    }

    [HttpPost("{id}/early-payment-request")]
    public ActionResult<ApiResponse<EarlyPaymentRequestDto>> CreateEarlyPaymentRequest(
        string id,
        [FromBody] CreateEarlyPaymentRequestDto payload)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequestResponse<EarlyPaymentRequestDto>(
                "Invoice id is required.",
                "Route parameter id is missing.");
        }

        var invoiceId = id.Trim();
        var result = _invoiceService.CreateEarlyPaymentRequest(invoiceId, payload);

        if (result.Outcome == CreateEarlyPaymentRequestOutcome.InvoiceNotFound)
        {
            return NotFoundResponse<EarlyPaymentRequestDto>(result.Message, result.Errors.ToArray());
        }

        if (result.Outcome is CreateEarlyPaymentRequestOutcome.DuplicateRequest or CreateEarlyPaymentRequestOutcome.NotEligible)
        {
            return ConflictResponse<EarlyPaymentRequestDto>(result.Message, result.Errors.ToArray());
        }

        if (result.Request is null)
        {
            return BadRequestResponse<EarlyPaymentRequestDto>(
                "Unable to create early payment request.",
                "Invalid request.");
        }

        var response = ApiResponse<EarlyPaymentRequestDto>.Ok(result.Request, result.Message);
        return Created($"/api/invoices/{invoiceId}/early-payment-request", response);
    }

    private ActionResult<ApiResponse<T>> BadRequestResponse<T>(string message, params string[] errors)
    {
        return BadRequest(ApiResponse<T>.Fail(message, errors));
    }

    private ActionResult<ApiResponse<T>> NotFoundResponse<T>(string message, params string[] errors)
    {
        return NotFound(ApiResponse<T>.Fail(message, errors));
    }

    private ActionResult<ApiResponse<T>> ConflictResponse<T>(string message, params string[] errors)
    {
        return Conflict(ApiResponse<T>.Fail(message, errors));
    }
}

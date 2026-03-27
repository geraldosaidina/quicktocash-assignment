using QuickToCash.Api.DTOs;
using QuickToCash.Api.Models;
using QuickToCash.Api.Services.Interfaces;

namespace QuickToCash.Api.Services;

public class EarlyPaymentService : IEarlyPaymentService
{
    private const decimal MonthlyFeeRate = 0.015m;
    private const int MinDaysBeforeDueDate = 5;
    private const int ProrationDays = 30;

    private readonly IDateTimeProvider _dateTimeProvider;

    public EarlyPaymentService(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public EarlyPaymentCalculationResultDto Evaluate(Invoice invoice)
    {
        var earlyByDays = (invoice.DueDate.Date - _dateTimeProvider.UtcNow.Date).Days;

        if (invoice.Status != InvoiceStatus.Approved)
        {
            return NotEligible(earlyByDays, "Invoice must be Approved.");
        }

        if (earlyByDays <= MinDaysBeforeDueDate)
        {
            return NotEligible(earlyByDays, "Due date must be more than 5 days away.");
        }

        var fee = decimal.Round(
            invoice.Amount * MonthlyFeeRate * earlyByDays / ProrationDays,
            2,
            MidpointRounding.AwayFromZero);

        var disbursementAmount = decimal.Round(
            invoice.Amount - fee,
            2,
            MidpointRounding.AwayFromZero);

        return new EarlyPaymentCalculationResultDto
        {
            IsEligible = true,
            Fee = fee,
            DisbursementAmount = disbursementAmount,
            EarlyByDays = earlyByDays,
            Reason = string.Empty
        };
    }

    private static EarlyPaymentCalculationResultDto NotEligible(int earlyByDays, string reason)
    {
        return new EarlyPaymentCalculationResultDto
        {
            IsEligible = false,
            Fee = 0m,
            DisbursementAmount = 0m,
            EarlyByDays = earlyByDays,
            Reason = reason
        };
    }
}

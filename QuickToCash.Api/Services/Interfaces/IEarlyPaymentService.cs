using QuickToCash.Api.DTOs;
using QuickToCash.Api.Models;

namespace QuickToCash.Api.Services.Interfaces;

public interface IEarlyPaymentService
{
    EarlyPaymentCalculationResultDto Evaluate(Invoice invoice);
}

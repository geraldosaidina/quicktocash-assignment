using Moq;
using QuickToCash.Api.DTOs;
using QuickToCash.Api.Models;
using QuickToCash.Api.Repositories.Interfaces;
using QuickToCash.Api.Services;
using QuickToCash.Api.Services.Interfaces;

namespace QuickToCash.Api.Tests.Services;

public class InvoiceServiceTests
{
    [Fact]
    public void CreateEarlyPaymentRequest_DisbursementMismatch_ReturnsNotEligibleAndDoesNotCreateRequest()
    {
        // Reason for this test:
        // The assignment requires disbursement to be InvoiceAmount - Fee (service-calculated),
        // so a client-provided amount that differs must be rejected to prevent inconsistent requests.
        var invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        var earlyPaymentRequestRepositoryMock = new Mock<IEarlyPaymentRequestRepository>();
        var earlyPaymentServiceMock = new Mock<IEarlyPaymentService>();
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var invoice = new Invoice
        {
            InvoiceId = "INV-1001",
            InvoiceNumber = "QTC-2026-0001",
            SupplierId = "SUP-100",
            SupplierName = "Acme Manufacturing",
            Amount = 10000m,
            SubmittedDate = new DateTime(2026, 3, 20),
            DueDate = new DateTime(2026, 4, 11),
            Status = InvoiceStatus.Approved
        };

        invoiceRepositoryMock.Setup(r => r.GetById(invoice.InvoiceId)).Returns(invoice);
        earlyPaymentRequestRepositoryMock.Setup(r => r.HasPendingRequestForInvoiceId(invoice.InvoiceId)).Returns(false);
        earlyPaymentServiceMock.Setup(s => s.Evaluate(invoice)).Returns(new EarlyPaymentCalculationResultDto
        {
            IsEligible = true,
            Fee = 70m,
            DisbursementAmount = 9930m,
            EarlyByDays = 14,
            Reason = string.Empty
        });

        var service = new InvoiceService(
            invoiceRepositoryMock.Object,
            earlyPaymentRequestRepositoryMock.Object,
            earlyPaymentServiceMock.Object,
            dateTimeProviderMock.Object);

        var result = service.CreateEarlyPaymentRequest(invoice.InvoiceId, new CreateEarlyPaymentRequestDto
        {
            DisbursementAmount = 1000m
        });

        Assert.Equal(CreateEarlyPaymentRequestOutcome.NotEligible, result.Outcome);
        Assert.Equal("Invoice is not eligible for early payment.", result.Message);
        Assert.Contains(result.Errors, error => error.Contains("must equal the calculated value", StringComparison.OrdinalIgnoreCase));

        earlyPaymentRequestRepositoryMock.Verify(
            r => r.Add(It.IsAny<EarlyPaymentRequest>()),
            Times.Never);
    }
}

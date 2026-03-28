using Moq;
using QuickToCash.Api.Models;
using QuickToCash.Api.Services;
using QuickToCash.Api.Services.Interfaces;

namespace QuickToCash.Api.Tests.Services;

public class EarlyPaymentServiceTests
{
    private static readonly DateTime FixedUtcNow = new(2026, 3, 27, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Evaluate_ApprovedWith30DaysAway_ReturnsEligible()
    {
        var service = CreateService();
        var invoice = CreateInvoice(InvoiceStatus.Approved, 10000m, FixedUtcNow.Date.AddDays(30));

        var result = service.Evaluate(invoice);

        Assert.True(result.IsEligible);
        Assert.Equal(30, result.EarlyByDays);
        Assert.Equal(150.00m, result.Fee);
        Assert.Equal(9850.00m, result.DisbursementAmount);
        Assert.Equal(string.Empty, result.Reason);
    }

    [Fact]
    public void Evaluate_ApprovedWith3DaysAway_ReturnsNotEligible()
    {
        var service = CreateService();
        var invoice = CreateInvoice(InvoiceStatus.Approved, 10000m, FixedUtcNow.Date.AddDays(3));

        var result = service.Evaluate(invoice);

        Assert.False(result.IsEligible);
        Assert.Equal(3, result.EarlyByDays);
        Assert.Equal(0m, result.Fee);
        Assert.Equal(0m, result.DisbursementAmount);
        Assert.Equal("Due date is too close.", result.Reason);
    }

    [Fact]
    public void Evaluate_PendingInvoice_ReturnsNotEligible()
    {
        var service = CreateService();
        var invoice = CreateInvoice(InvoiceStatus.Pending, 10000m, FixedUtcNow.Date.AddDays(30));

        var result = service.Evaluate(invoice);

        Assert.False(result.IsEligible);
        Assert.Equal("Invoice is not yet approved.", result.Reason);
    }

    [Fact]
    public void Evaluate_RejectedInvoice_ReturnsNotEligible()
    {
        var service = CreateService();
        var invoice = CreateInvoice(InvoiceStatus.Rejected, 10000m, FixedUtcNow.Date.AddDays(30));

        var result = service.Evaluate(invoice);

        Assert.False(result.IsEligible);
        Assert.Equal("Invoice has been rejected.", result.Reason);
    }

    [Fact]
    public void Evaluate_FeeCalculation_IsProratedDaily()
    {
        var service = CreateService();
        var invoice = CreateInvoice(InvoiceStatus.Approved, 20000m, FixedUtcNow.Date.AddDays(45));

        var result = service.Evaluate(invoice);

        Assert.True(result.IsEligible);
        Assert.Equal(45, result.EarlyByDays);
        Assert.Equal(450.00m, result.Fee);
        Assert.Equal(19550.00m, result.DisbursementAmount);
    }

    private static IEarlyPaymentService CreateService()
    {
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock
            .Setup(x => x.UtcNow)
            .Returns(FixedUtcNow);

        return new EarlyPaymentService(dateTimeProviderMock.Object);
    }

    private static Invoice CreateInvoice(InvoiceStatus status, decimal amount, DateTime dueDate)
    {
        return new Invoice
        {
            InvoiceId = "INV-TEST",
            InvoiceNumber = "QTC-TEST-0001",
            SupplierId = "SUP-TEST",
            SupplierName = "Supplier Test",
            Amount = amount,
            SubmittedDate = FixedUtcNow.Date.AddDays(-2),
            DueDate = dueDate,
            Status = status
        };
    }
}

using QuickToCash.Api.Services.Interfaces;

namespace QuickToCash.Api.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

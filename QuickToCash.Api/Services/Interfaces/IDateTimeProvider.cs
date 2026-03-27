namespace QuickToCash.Api.Services.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

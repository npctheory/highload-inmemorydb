namespace Core.Infrastructure.Providers;

using Core.Application.Abstractions;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
namespace Core.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow {get;}
}
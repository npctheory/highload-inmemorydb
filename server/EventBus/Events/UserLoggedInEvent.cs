using MediatR;

namespace EventBus.Events;

public class UserLoggedInEvent : INotification
{
    public string UserId { get; set; }

    public UserLoggedInEvent(string userId)
    {
        UserId = userId;
    }
}
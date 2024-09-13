using MediatR;

namespace EventBus.Events;

public class FriendAddedEvent : INotification
{
    public string UserId { get; set; }
    public string FriendId { get; set; }


    public FriendAddedEvent(string userId, string friendId)
    {
        UserId = userId;
        FriendId = friendId;
    }
}
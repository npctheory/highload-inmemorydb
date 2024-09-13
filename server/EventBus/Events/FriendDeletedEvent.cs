using MediatR;

namespace EventBus.Events;

public class FriendDeletedEvent : INotification
{
    public string UserId { get; set; }
    public string FriendId { get; set; }


    public FriendDeletedEvent(string userId, string friendId)
    {
        UserId = userId;
        FriendId = friendId;
    }
}
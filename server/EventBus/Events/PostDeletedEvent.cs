using MediatR;

namespace EventBus.Events;

public class PostDeletedEvent : INotification
{
    public string UserId { get; set; }
    public string PostId { get; set; }


    public PostDeletedEvent(string userId, string postId)
    {
        UserId = userId;
        PostId = postId;
    }
}
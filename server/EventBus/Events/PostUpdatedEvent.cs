using MediatR;

namespace EventBus.Events;

public class PostUpdatedEvent : INotification
{
    public string UserId { get; set; }
    public Guid PostId { get; set; }
    public string Text {get; set;}


    public PostUpdatedEvent(string userId, Guid postId, string text)
    {
        UserId = userId;
        PostId = postId;
        Text = text;
    }
}
using MediatR;

namespace EventBus.Events;

public class PostCreatedEvent : INotification
{
    public string UserId { get; set; }
    public string PostId { get; set; }
    public string Text {get; set;}


    public PostCreatedEvent(string userId, string postId, string text)
    {
        UserId = userId;
        PostId = postId;
        Text = text;
    }
}
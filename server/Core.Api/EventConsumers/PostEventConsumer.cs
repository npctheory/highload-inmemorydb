using MassTransit;
using Microsoft.AspNetCore.SignalR;
using EventBus.Events;
using System.Threading.Tasks;
using Core.Api.Hubs;
using MediatR;

namespace Core.Api.EventConsumers
{
    public class PostEventConsumer :
        IConsumer<PostDeletedEvent>,
        IConsumer<PostCreatedEvent>,
        IConsumer<PostUpdatedEvent>
    {
        private readonly IHubContext<PostHub> _hubContext;
        private readonly IMediator _mediator;

        public PostEventConsumer(IHubContext<PostHub> hubContext, IMediator mediator)
        {
            _hubContext = hubContext;
            _mediator = mediator;
            
        }

        public async Task Consume(ConsumeContext<PostDeletedEvent> context)
        {
            var postDeletedEvent = context.Message;
            string group = postDeletedEvent.UserId;
            await _hubContext.Clients.Group(group)
                                      .SendAsync("PostDeletedEvent", postDeletedEvent);
        }

        public async Task Consume(ConsumeContext<PostCreatedEvent> context)
        {
            var postcreatedEvent = context.Message;
            string group = postcreatedEvent.UserId;
            await _mediator.Publish(postcreatedEvent);
            await _hubContext.Clients.Group($"{postcreatedEvent.UserId}")
                                      .SendAsync("PostCreatedEvent", postcreatedEvent);
        }

        public async Task Consume(ConsumeContext<PostUpdatedEvent> context)
        {
            var postUpdatedEvent = context.Message;
            string group = postUpdatedEvent.UserId;
            await _hubContext.Clients.Group($"{postUpdatedEvent.UserId}")
                                      .SendAsync("PostUpdatedEvent", postUpdatedEvent);
        }
    }
}

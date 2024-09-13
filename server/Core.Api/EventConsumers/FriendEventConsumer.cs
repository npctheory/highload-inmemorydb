using MassTransit;
using MediatR;
using EventBus.Events;
using System.Threading.Tasks;

namespace Core.Api.EventConsumers;

public class FriendEventConsumer : 
    IConsumer<FriendAddedEvent>,
    IConsumer<FriendDeletedEvent>
{
    private readonly IMediator _mediator;

    public FriendEventConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<FriendAddedEvent> context)
    {
        await _mediator.Publish(context.Message);
    }

    public async Task Consume(ConsumeContext<FriendDeletedEvent> context)
    {
        await _mediator.Publish(context.Message);
    }
}
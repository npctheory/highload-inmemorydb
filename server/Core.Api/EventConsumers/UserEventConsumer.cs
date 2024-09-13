using MassTransit;
using MediatR;
using EventBus.Events;
using System.Threading.Tasks;

namespace Core.Api.EventConsumers;

public class UserEventConsumer : 
    IConsumer<UserLoggedInEvent>
{
    private readonly IMediator _mediator;

    public UserEventConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<UserLoggedInEvent> context)
    {
        await _mediator.Publish(context.Message);
    }
}
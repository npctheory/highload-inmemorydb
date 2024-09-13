using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Interfaces;
using EventBus;
using EventBus.Events;
using MediatR;


namespace Core.Application.Friends.Commands.AddFriend;

public class AddFriendQueryHandler : IRequestHandler<AddFriendQuery, bool>
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IMapper _mapper;
    private readonly IEventBus _eventBus;

    public AddFriendQueryHandler(
        IFriendshipRepository friendshipRepository, 
        IMapper mapper, 
        IEventBus eventBus)
    {
        _friendshipRepository = friendshipRepository;
        _mapper = mapper;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(AddFriendQuery request, CancellationToken cancellationToken)
    {
        await _friendshipRepository.AddFriendship(request.UserId, request.FriendId);
        var friendAddEvent = new FriendAddedEvent(request.UserId,request.FriendId);
        await _eventBus.PublishAsync(friendAddEvent, cancellationToken);
        return true;
    }
}
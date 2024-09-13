using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Core.Domain.Interfaces;
using EventBus;
using EventBus.Events;

namespace Core.Application.Friends.Commands.DeleteFriend
{
    public class DeleteFriendQueryHandler : IRequestHandler<DeleteFriendQuery, bool>
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;

        public DeleteFriendQueryHandler(
            IFriendshipRepository friendshipRepository, 
            IMapper mapper, 
            IEventBus eventBus)
        {
            _friendshipRepository = friendshipRepository;
            _mapper = mapper;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(DeleteFriendQuery request, CancellationToken cancellationToken)
        {
            await _friendshipRepository.DeleteFriendship(request.UserId, request.FriendId);
            var friendDeleteEvent = new FriendDeletedEvent(request.UserId,request.FriendId);
            await _eventBus.PublishAsync(friendDeleteEvent, cancellationToken);
            return true;
        }
    }
}

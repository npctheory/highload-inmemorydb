using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Core.Application.Friends.DTO;
using Core.Domain.Entities;
using Core.Domain.Interfaces;


namespace Core.Application.Friends.Queries.ListFriends;

public class ListFriendsQueryHandler : IRequestHandler<ListFriendsQuery, List<FriendDTO>>
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IMapper _mapper;

    public ListFriendsQueryHandler(IFriendshipRepository friendshipRepository, IMapper mapper)
    {
        _friendshipRepository = friendshipRepository;
        _mapper = mapper;
    }

    public async Task<List<FriendDTO>> Handle(ListFriendsQuery request, CancellationToken cancellationToken)
    {
        List<Friendship> friends = await _friendshipRepository.ListFriendships(request.userId);
        return _mapper.Map<List<FriendDTO>>(friends);
    }
}

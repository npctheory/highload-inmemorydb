using MediatR;
using Core.Application.Friends.DTO;

namespace Core.Application.Friends.Queries.ListFriends;

public record ListFriendsQuery(string userId) : IRequest<List<FriendDTO>>;
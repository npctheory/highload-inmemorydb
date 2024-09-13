using MediatR;

namespace Core.Application.Friends.Commands.DeleteFriend;

public record DeleteFriendQuery(string UserId, string FriendId) : IRequest<bool>;
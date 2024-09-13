using MediatR;

namespace Core.Application.Friends.Commands.AddFriend;

public record AddFriendQuery(string UserId, string FriendId) : IRequest<bool>;
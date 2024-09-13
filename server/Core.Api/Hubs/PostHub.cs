using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Core.Application.Friends.DTO;
using Core.Application.Friends.Queries.ListFriends;
using System.Security.Claims;
using MediatR;
using EventBus.Events;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Core.Api.Hubs
{
    [Authorize]
    public class PostHub : Hub
    {
        private readonly IMediator _mediator;

        public PostHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                await base.OnConnectedAsync();
                return;
            }

            List<FriendDTO> friends = await _mediator.Send(new ListFriendsQuery(userId));
            var groupNames = new List<string>();

            foreach (var friend in friends)
            {
                var groupName = friend.FriendId.ToString();
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                groupNames.Add(groupName);
            }

            var formattedGroupNames = string.Join(", ", groupNames);

            await Clients.Caller.SendAsync("ReceiveGroupInfo", formattedGroupNames);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }

            List<FriendDTO> friends = await _mediator.Send(new ListFriendsQuery(userId));
            var groupNames = new List<string>();

            foreach (var friend in friends)
            {
                var groupName = friend.FriendId.ToString();
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                groupNames.Add(groupName);
            }

            var formattedGroupNames = string.Join(", ", groupNames);

            await Clients.Caller.SendAsync("Добавлен к группам:", formattedGroupNames);

            await base.OnDisconnectedAsync(exception);
        }
    }
}

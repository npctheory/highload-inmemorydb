using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Interfaces;
using EventBus.Events;
using Core.Application.Abstractions;
using Core.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Core.Application.Posts.Queries.GetPostFeed;

public class PostFeedCacheBuilder :
    INotificationHandler<UserLoggedInEvent>,
    INotificationHandler<FriendAddedEvent>,
    INotificationHandler<FriendDeletedEvent>
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly ICacheService _cacheService;
    private readonly IPostRepository _postRepository;
    private readonly TimeSpan _postFeedTtl;

    public PostFeedCacheBuilder(
        IFriendshipRepository friendshipRepository,
        ICacheService cacheService,
        IPostRepository postRepository)
    {
        _friendshipRepository = friendshipRepository;
        _cacheService = cacheService;
        _postRepository = postRepository;
        _postFeedTtl = TimeSpan.FromMinutes(5);
    }

    public async Task Handle(UserLoggedInEvent notification, CancellationToken cancellationToken)
    {
        var userId = notification.UserId;
        await BuildFriendsPostsCacheForUser(userId);
    }

    public async Task Handle(FriendAddedEvent notification, CancellationToken cancellationToken)
    {
        var userId = notification.UserId;
        await BuildFriendsPostsCacheForUser(userId);
    }

    public async Task Handle(FriendDeletedEvent notification, CancellationToken cancellationToken)
    {
        var userId = notification.UserId;
        await BuildFriendsPostsCacheForUser(userId);
    }

    private async Task BuildFriendsPostsCacheForUser(string userId)
    {
        string prefix = $"FriendsPosts:{userId}:";
        string cacheKeyForFirst1000 = $"FriendsPosts:{userId}:0:1000";

        await _cacheService.RemoveByPrefixAsync(prefix);

        List<Friendship> friendships = await _friendshipRepository.ListFriendships(userId);

        List<string> friendsIds = friendships.Select(f => f.FriendId).ToList();
        List<Post> cachedFirst1000Posts = await _postRepository.ListPostsByUserIds(friendsIds, 0, 1000);
        if (cachedFirst1000Posts.Count > 0)
        {
            await _cacheService.SetAsync(cacheKeyForFirst1000, cachedFirst1000Posts, _postFeedTtl);
        }
    }
}

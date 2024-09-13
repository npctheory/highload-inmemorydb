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

public class FriendsPostFeedCacheRebuilder :
    INotificationHandler<PostDeletedEvent>,
    INotificationHandler<PostCreatedEvent>,
    INotificationHandler<PostUpdatedEvent>
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly ICacheService _cacheService;
    private readonly IPostRepository _postRepository;
    private readonly TimeSpan _postFeedTtl;

    public FriendsPostFeedCacheRebuilder(
        IFriendshipRepository friendshipRepository,
        ICacheService cacheService,
        IPostRepository postRepository)
    {
        _friendshipRepository = friendshipRepository;
        _cacheService = cacheService;
        _postRepository = postRepository;
        _postFeedTtl = TimeSpan.FromMinutes(5);
    }

    public async Task Handle(PostDeletedEvent notification, CancellationToken cancellationToken)
    {
        var userId = notification.UserId;
        await RebuildFriendsPostsCacheForFriends(userId);
    }

    public async Task Handle(PostCreatedEvent notification, CancellationToken cancellationToken)
    {
        var userId = notification.UserId;
        await RebuildFriendsPostsCacheForFriends(userId);
    }

    public async Task Handle(PostUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var userId = notification.UserId;
        await RebuildFriendsPostsCacheForFriends(userId);
    }

    private async Task RebuildFriendsPostsCacheForFriends(string userId)
    {
        List<Friendship> users = await _friendshipRepository.ListUsersWithFriend(userId);

        foreach (var friendship in users)
        {
            string cacheKeyForFirst1000 = $"FriendsPosts:{friendship.UserId}:0:1000";
            string prefix = $"FriendsPosts:{friendship.UserId}:";

            var cachedPosts = await _cacheService.GetAsync<List<Post>>(cacheKeyForFirst1000);
            if (cachedPosts != null && cachedPosts.Count > 0)
            {
                await _cacheService.RemoveByPrefixAsync(prefix);

                List<Friendship> friends = await _friendshipRepository.ListFriendships(friendship.UserId);
                List<string> friendIds = friends.Select(f => f.FriendId).ToList();

                List<Post> postsToCache = await _postRepository.ListPostsByUserIds(friendIds, 0, 1000);
                if (postsToCache.Count > 0)
                {
                    await _cacheService.SetAsync(cacheKeyForFirst1000, postsToCache, _postFeedTtl);
                }
            }
        }
    }
}

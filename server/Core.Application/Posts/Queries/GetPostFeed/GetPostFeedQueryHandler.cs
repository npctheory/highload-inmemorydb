using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Abstractions;
using Core.Application.Posts.DTO;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using EventBus;
using MediatR;

namespace Core.Application.Posts.Queries.GetPostFeed;

public class GetPostFeedQueryHandler : IRequestHandler<GetPostFeedQuery, List<PostDTO>>
{
    private readonly IPostRepository _postRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly TimeSpan _postFeedTtl;


    public GetPostFeedQueryHandler(IPostRepository postRepository, IFriendshipRepository friendshipRepository, IMapper mapper, ICacheService cacheService)
    {
        _postRepository = postRepository;
        _friendshipRepository = friendshipRepository;
        _mapper = mapper;
        _cacheService = cacheService;
        _postFeedTtl = TimeSpan.FromMinutes(5);
    }

    public async Task<List<PostDTO>> Handle(GetPostFeedQuery request, CancellationToken cancellationToken)
    {
        string cacheKeyForFirst1000 = $"FriendsPosts:{request.userId}:0:1000";
        List<Post> posts;


        List<Post> cachedFirst1000Posts = await _cacheService.GetAsync<List<Post>>(cacheKeyForFirst1000);


        if (cachedFirst1000Posts == null || cachedFirst1000Posts.Count == 0)
        {
            List<Friendship> friendships = await _friendshipRepository.ListFriendships(request.userId);
            List<string> friendsIds = friendships.Select(f => f.FriendId).ToList();


            cachedFirst1000Posts = await _postRepository.ListPostsByUserIds(friendsIds, 0, 1000);

            if (cachedFirst1000Posts.Count > 0)
            {
                await _cacheService.SetAsync(cacheKeyForFirst1000, cachedFirst1000Posts, _postFeedTtl);
            }
        }

        if ((request.offset + request.limit) <= 1000)
        {
            posts = cachedFirst1000Posts.Skip(request.offset).Take(request.limit).ToList();
        }
        else
        {
            string dynamicCacheKey = $"FriendsPosts:{request.userId}:{request.offset}:{request.limit}";

            List<Post> cachedPosts = await _cacheService.GetAsync<List<Post>>(dynamicCacheKey);

            if (cachedPosts != null && cachedPosts.Count > 0)
            {
                posts = cachedPosts;
            }
            else
            {
                List<Friendship> friendships = await _friendshipRepository.ListFriendships(request.userId);
                List<string> friendsIds = friendships.Select(f => f.FriendId).ToList();
                posts = await _postRepository.ListPostsByUserIds(friendsIds, request.offset, request.limit);

                if (posts.Count > 0)
                {
                    await _cacheService.SetAsync(dynamicCacheKey, posts, _postFeedTtl);
                }
            }
        }
        return _mapper.Map<List<PostDTO>>(posts);
    }
}

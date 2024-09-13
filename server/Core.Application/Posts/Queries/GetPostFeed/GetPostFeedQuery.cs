using Core.Application.Posts.DTO;
using MediatR;

namespace Core.Application.Posts.Queries.GetPostFeed;

public record GetPostFeedQuery(string userId, int offset, int limit) : IRequest<List<PostDTO>>;
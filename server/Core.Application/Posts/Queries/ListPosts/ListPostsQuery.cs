using Core.Application.Posts.DTO;
using MediatR;

namespace Core.Application.Posts.Queries.ListPosts;

public record ListPostsQuery(string userId) : IRequest<List<PostDTO>>;
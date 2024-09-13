using Core.Application.Posts.DTO;
using MediatR;

namespace Core.Application.Posts.Queries.GetPost;

public record GetPostQuery(string postId) : IRequest<PostDTO>;
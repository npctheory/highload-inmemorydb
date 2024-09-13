using Core.Application.Posts.DTO;
using MediatR;

namespace Core.Application.Posts.Commands.DeletePost;

public record DeletePostCommand(string userId, string postId) : IRequest<PostDTO>;
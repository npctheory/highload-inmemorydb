using Core.Application.Posts.DTO;
using MediatR;
using System;

namespace Core.Application.Posts.Commands.UpdatePost;

public record UpdatePostCommand(string UserId, string PostId, string Text) : IRequest<PostDTO>;
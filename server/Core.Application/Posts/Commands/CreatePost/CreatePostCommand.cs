using Core.Application.Posts.DTO;
using MediatR;
using System;

namespace Core.Application.Posts.Commands.CreatePost;

public record CreatePostCommand(string UserId, string Text) : IRequest<PostDTO>;
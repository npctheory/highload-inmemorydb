using AutoMapper;
using Core.Application.Posts.DTO;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using EventBus;
using EventBus.Events;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Posts.Commands.CreatePost;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, PostDTO>
{
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;

        public CreatePostCommandHandler(
            IPostRepository postRepository, 
            IMapper mapper, 
            IEventBus eventBus)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _eventBus = eventBus;
        }

    public async Task<PostDTO> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.CreatePost(request.UserId, request.Text);
        var postCreatedEvent = new PostCreatedEvent(post.UserId, post.Id.ToString(), post.Text);
            await _eventBus.PublishAsync(postCreatedEvent, cancellationToken);
        return _mapper.Map<PostDTO>(post);
    }
}


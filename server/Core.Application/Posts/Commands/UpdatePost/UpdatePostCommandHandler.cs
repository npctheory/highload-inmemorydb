using AutoMapper;
using Core.Application.Posts.DTO;
using Core.Domain.Interfaces;
using EventBus;
using EventBus.Events;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Posts.Commands.UpdatePost
{
    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, PostDTO>
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;

        public UpdatePostCommandHandler(
            IPostRepository postRepository, 
            IMapper mapper, 
            IEventBus eventBus)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _eventBus = eventBus;
        }

        public async Task<PostDTO> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
        var post = await _postRepository.UpdatePost(request.UserId, Guid.Parse(request.PostId), request.Text);
        var postUpdatedEvent = new PostUpdatedEvent(post.UserId, post.Id, post.Text);
            await _eventBus.PublishAsync(postUpdatedEvent, cancellationToken);
        return _mapper.Map<PostDTO>(post);
        }
    }
}

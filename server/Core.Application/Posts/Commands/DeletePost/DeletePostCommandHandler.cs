using AutoMapper;
using Core.Application.Posts.DTO;
using Core.Domain.Interfaces;
using EventBus;
using EventBus.Events;
using MediatR;

namespace Core.Application.Posts.Commands.DeletePost;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, PostDTO>
{
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;

        public DeletePostCommandHandler(
            IPostRepository postRepository, 
            IMapper mapper, 
            IEventBus eventBus)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _eventBus = eventBus;
        }

    public async Task<PostDTO> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var deleted = _mapper.Map<PostDTO>(await _postRepository.DeletePost(request.userId, Guid.Parse(request.postId)));
        var postDeleteEvent = new PostDeletedEvent(request.userId, request.postId);
        await _eventBus.PublishAsync(postDeleteEvent, cancellationToken);
        return deleted;
    }
}

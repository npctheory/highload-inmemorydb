using AutoMapper;
using Core.Application.Posts.DTO;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.Posts.Queries.GetPost;

public class GetPostQueryHandler : IRequestHandler<GetPostQuery, PostDTO>
{
    private readonly IPostRepository _postRepository;
    private readonly IMapper _mapper;

    public GetPostQueryHandler(IPostRepository postRepository, IMapper mapper)
    {
        _postRepository = postRepository;
        _mapper = mapper;
    }

    public async Task<PostDTO> Handle(GetPostQuery request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetPostById(Guid.Parse(request.postId));
        return _mapper.Map<PostDTO>(post);
    }
}

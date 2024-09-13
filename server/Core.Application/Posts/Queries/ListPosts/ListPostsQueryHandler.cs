using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Posts.DTO;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.Posts.Queries.ListPosts;
public class ListPostsQueryHandler : IRequestHandler<ListPostsQuery, List<PostDTO>>
{
    private readonly IPostRepository _postRepository;
    private readonly IMapper _mapper;

    public ListPostsQueryHandler(IPostRepository postRepository, IMapper mapper)
    {
        _postRepository = postRepository;
        _mapper = mapper;
    }

    public async Task<List<PostDTO>> Handle(ListPostsQuery request, CancellationToken cancellationToken)
    {
        List<Post> posts = await _postRepository.ListPostsByUserId(request.userId);
        return _mapper.Map<List<PostDTO>>(posts);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Users.Queries.SearchUsers;
using Core.Domain.Entities;
using AutoMapper;
using MediatR;
using Core.Domain.Interfaces;
using Core.Application.Users.DTO;

namespace Core.Application.Users.Queries.SearchUsers
{
    public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, List<UserDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public SearchUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<UserDTO>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            List<User> users = await _userRepository.SearchUsersAsync(request.first_name, request.second_name);
            return _mapper.Map<List<UserDTO>>(users);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Users.Queries.GetUser;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using AutoMapper;
using MediatR;
using Core.Application.Users.DTO;

namespace Core.Application.Users.Queries.GetUser
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDTO> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            User user = await _userRepository.GetUserByIdAsync(request.Id);
            return _mapper.Map<UserDTO>(user);
        }
    }
}
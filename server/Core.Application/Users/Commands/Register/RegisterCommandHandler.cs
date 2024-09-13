using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Users.DTO;
using Core.Application.Abstractions;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Core.Application.Users.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IMapper mapper,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<UserDTO> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = request.FirstName,
                SecondName = request.SecondName,
                Birthdate = DateTime.Parse(request.Birthdate),
                Biography = request.Biography,
                City = request.City,
                PasswordHash = _passwordHasher.HashPassword(request.Password)
            };

            await _userRepository.CreateUserAsync(user);

            return _mapper.Map<UserDTO>(user);
        }
    }
}
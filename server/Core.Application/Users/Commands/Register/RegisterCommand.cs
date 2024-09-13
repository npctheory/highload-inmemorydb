using Core.Application.Users.DTO;
using MediatR;

namespace Core.Application.Users.Commands.Register;

public record RegisterCommand(string FirstName, string SecondName, string Birthdate, string Biography, string City, string Password) : IRequest<UserDTO>;
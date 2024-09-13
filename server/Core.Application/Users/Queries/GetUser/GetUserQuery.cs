using Core.Application.Users.DTO;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Users.Queries.GetUser;

public record GetUserQuery(string Id) : IRequest<UserDTO>;
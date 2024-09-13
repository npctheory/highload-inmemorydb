using Core.Application.Users.DTO;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Users.Queries.SearchUsers;

public record SearchUsersQuery(string first_name, string second_name) : IRequest<List<UserDTO>>;
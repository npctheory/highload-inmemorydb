using Core.Application.Users.DTO;
using MediatR;

namespace Core.Application.Users.Queries.Login;

public record LoginQuery(string Id, string Password) : IRequest<TokenDTO>;
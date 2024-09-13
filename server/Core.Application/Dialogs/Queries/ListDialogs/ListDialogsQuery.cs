using Core.Application.Dialogs.DTO;
using MediatR;

namespace Core.Application.Dialogs.Queries.ListDialogs;

public record ListDialogsQuery(string userId) : IRequest<List<AgentDTO>>;
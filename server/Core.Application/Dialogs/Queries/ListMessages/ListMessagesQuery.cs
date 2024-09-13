using global::Core.Application.Dialogs.DTO;
using MediatR;

namespace Core.Application.Dialogs.Queries.ListMessages;

public record ListMessagesQuery(string userId, string agentId) : IRequest<List<DialogMessageDTO>>;
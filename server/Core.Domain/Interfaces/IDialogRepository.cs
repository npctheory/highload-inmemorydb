using Core.Domain.Aggregates;
using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

public interface IDialogRepository
{
    Task<List<DialogMessage>> ListMessages(string userId, string agentId);
    Task<DialogMessage> SendMessage(DialogMessage message);
    Task<List<Dialog>> ListDialogs(string user);
}
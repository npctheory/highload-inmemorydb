using Core.Domain.Aggregates;
using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

public interface IDialogRepository
{
    Task<DialogMessage> SendMessage(DialogMessage message);
    Task<List<DialogMessage>> ListMessages(string userId, string agentId);
    Task<List<Dialog>> ListDialogs(string user);
}
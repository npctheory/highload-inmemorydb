using Core.Domain.Interfaces;
using Core.Domain.Entities;
using StackExchange.Redis;
using Core.Domain.Aggregates;

namespace Core.Infrastructure.Repositories;

public class RedisDialogRepository : IDialogRepository
{
    private readonly IDatabase _database;

    public RedisDialogRepository(IConnectionMultiplexer redis, int databaseIndex = 1)
    {
        _database = redis.GetDatabase(databaseIndex);
    }

    public async Task<DialogMessage> SendMessage(DialogMessage message)
    {
        var key = message.Id.ToString();
        var script = RedisDialogRepositoryScripts.SendMessageLuaScript;

        var result = await _database.ScriptEvaluateAsync(script, new RedisKey[] { }, new RedisValue[]
        {
            key,
            message.SenderId,
            message.ReceiverId,
            message.Text,
            message.IsRead.ToString(),
            message.Timestamp.ToString("o")
        });

        if (result.ToString() == "OK")
        {
            return message;
        }

        throw new Exception("Failed to execute Lua script.");
    }

    public Task<List<DialogMessage>> ListMessages(string userId, string agentId)
    {
        throw new NotImplementedException();
    }

public async Task<List<Dialog>> ListDialogs(string user)
{
    var script = RedisDialogRepositoryScripts.ListDialogsLuaScript;
    var result = (RedisValue[])await _database.ScriptEvaluateAsync(script, new RedisKey[] { }, new RedisValue[] { user });
    var dialogList = result.Select(agentId => new Dialog { AgentId = agentId }).ToList();
    return dialogList;
}
}
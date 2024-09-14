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
            message.Timestamp.ToString("o"),
            key
        });

        if (result.ToString() == "OK")
        {
            return message;
        }

        throw new Exception("Failed to execute Lua script.");
    }

    public async Task<List<DialogMessage>> ListMessages(string userId, string agentId)
    {
        var script = RedisDialogRepositoryScripts.ListMessagesLuaScript;

        var result = (RedisResult[])await _database.ScriptEvaluateAsync(script, new RedisKey[] { }, new RedisValue[] { userId, agentId });
        var dialogMessages = new List<DialogMessage>();

        foreach (RedisResult message in result)
        {
            var messageData = (RedisValue[])message;
            var dialogMessage = new DialogMessage();
            for (int i = 0; i < messageData.Length; i += 2)
            {
                string fieldName = messageData[i].ToString();
                string fieldValue = messageData[i + 1].ToString();

                switch (fieldName)
                {
                    case "Id":
                        dialogMessage.Id = Guid.Parse(fieldValue);
                        break;
                    case "SenderId":
                        dialogMessage.SenderId = fieldValue;
                        break;
                    case "ReceiverId":
                        dialogMessage.ReceiverId = fieldValue;
                        break;
                    case "Text":
                        dialogMessage.Text = fieldValue;
                        break;
                    case "IsRead":
                        dialogMessage.IsRead = bool.Parse(fieldValue);
                        break;
                    case "Timestamp":
                        dialogMessage.Timestamp = DateTime.Parse(fieldValue);
                        break;
                }
            }

            dialogMessages.Add(dialogMessage);
        }

        return dialogMessages;
    }



    public async Task<List<Dialog>> ListDialogs(string user)
    {
        var script = RedisDialogRepositoryScripts.ListDialogsLuaScript;
        var result = (RedisValue[])await _database.ScriptEvaluateAsync(script, new RedisKey[] { }, new RedisValue[] { user });
        var dialogList = result.Select(agentId => new Dialog { AgentId = agentId }).ToList();
        return dialogList;
    }
}
namespace Core.Infrastructure.Repositories;

public static class RedisDialogRepositoryScripts
{
    public const string SendMessageLuaScript = @"
        local key = ARGV[1]
        local senderId = ARGV[2]
        local receiverId = ARGV[3]
        local text = ARGV[4]
        local isRead = ARGV[5] -- Expecting 'true' or 'false' as string
        local timestamp = ARGV[6]
        local id = ARGV[7]  -- Include ID in the message hash

        redis.call('HMSET', key, 'Id', id, 'SenderId', senderId, 'ReceiverId', receiverId, 'Text', text, 'IsRead', isRead, 'Timestamp', timestamp)
        redis.call('SADD', senderId .. ':' .. receiverId, key)
        return 'OK'
    ";

     public const string ListDialogsLuaScript = @"
        local user = ARGV[1]
        local keysWithPrefix = redis.call('KEYS', user .. ':*')
        local keysWithPostfix = redis.call('KEYS', '*:' .. user)
        local combinedKeys = {}

        for _, key in ipairs(keysWithPrefix) do
            table.insert(combinedKeys, key)
        end

        for _, key in ipairs(keysWithPostfix) do
            table.insert(combinedKeys, key)
        end

        local dialogs = {}

        for _, key in ipairs(combinedKeys) do
            local otherUser

            if string.sub(key, 1, #user + 1) == user .. ':' then
                otherUser = string.sub(key, #user + 2)
            else
                otherUser = string.sub(key, 1, string.find(key, ':') - 1)
            end

            dialogs[otherUser] = true  -- Ensure unique dialogs
        end

        local result = {}
        for otherUser, _ in pairs(dialogs) do
            table.insert(result, otherUser)
        end

        return result
    ";

    public const string ListMessagesLuaScript = @"
        local userId = ARGV[1]
        local agentId = ARGV[2]
        local set1 = redis.call('SMEMBERS', userId .. ':' .. agentId)
        local set2 = redis.call('SMEMBERS', agentId .. ':' .. userId)
        local combinedKeys = {}

        for _, key in ipairs(set1) do
            table.insert(combinedKeys, key)
        end

        for _, key in ipairs(set2) do
            table.insert(combinedKeys, key)
        end

        local messages = {}
        
        for _, key in ipairs(combinedKeys) do
            local message = redis.call('HGETALL', key)
            table.insert(messages, message)
        end

        return messages
    ";
}

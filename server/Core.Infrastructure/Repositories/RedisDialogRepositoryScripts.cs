namespace Core.Infrastructure.Repositories;

public static class RedisDialogRepositoryScripts
{
    public const string SendMessageLuaScript = @"
        local key = ARGV[1]
        local senderId = ARGV[2]
        local receiverId = ARGV[3]
        local text = ARGV[4]
        local isRead = ARGV[5]
        local timestamp = ARGV[6]

        -- Store the message in a hash
        redis.call('HMSET', key, 'SenderId', senderId, 'ReceiverId', receiverId, 'Text', text, 'IsRead', isRead, 'Timestamp', timestamp)

        -- Add the message key to the set representing the dialog between sender and receiver
        redis.call('SADD', senderId .. ':' .. receiverId, key)

        return 'OK'
    ";

     public const string ListDialogsLuaScript = @"
        local user = ARGV[1]

        -- Search for dialogs where the user is the sender
        local keysWithPrefix = redis.call('KEYS', user .. ':*')

        -- Search for dialogs where the user is the receiver
        local keysWithPostfix = redis.call('KEYS', '*:' .. user)

        -- Combine both sets of keys
        local combinedKeys = {}

        for _, key in ipairs(keysWithPrefix) do
            table.insert(combinedKeys, key)
        end

        for _, key in ipairs(keysWithPostfix) do
            table.insert(combinedKeys, key)
        end

        -- Extract the other user from the dialog keys
        local dialogs = {}

        for _, key in ipairs(combinedKeys) do
            local otherUser

            -- If the user is the sender, extract the receiver
            if string.sub(key, 1, #user + 1) == user .. ':' then
                otherUser = string.sub(key, #user + 2)
            else
                -- If the user is the receiver, extract the sender
                otherUser = string.sub(key, 1, string.find(key, ':') - 1)
            end

            dialogs[otherUser] = true  -- Ensure unique dialogs
        end

        -- Convert the dialog table to a list
        local result = {}
        for otherUser, _ in pairs(dialogs) do
            table.insert(result, otherUser)
        end

        return result
    ";
}

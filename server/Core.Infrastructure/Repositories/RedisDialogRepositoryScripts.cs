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

        redis.call('HMSET', key, 'SenderId', senderId, 'ReceiverId', receiverId, 'Text', text, 'IsRead', isRead, 'Timestamp', timestamp)
        
        -- Add message ID to sets for quick lookups
        redis.call('SADD', 'messages:' .. senderId .. ':to:' .. receiverId, key)
        redis.call('SADD', 'messages:' .. receiverId .. ':to:' .. senderId, key)
        
        return 'OK'
    ";
}

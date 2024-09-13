using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using Core.Application.Abstractions;

namespace Core.Application.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _database;

        // Constructor now takes a database index
        public RedisCacheService(IConnectionMultiplexer redis, int databaseIndex = 0)
        {
            _database = redis.GetDatabase(databaseIndex);
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            var redisValue = await _database.StringGetAsync(key);
            if (redisValue.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<T>(redisValue);
        }

        public async Task<T> SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var serializedValue = JsonSerializer.Serialize(value);

            await _database.StringSetAsync(key, serializedValue, expiration);

            return value;
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            await _database.KeyDeleteAsync(key);
        }

        public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prefixKey))
                throw new ArgumentException("Prefix key cannot be null or whitespace.", nameof(prefixKey));

            var server = GetServer();
            var keys = server.Keys(pattern: $"{prefixKey}*");

            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
            }
        }

        private IServer GetServer()
        {
            var endpoints = _database.Multiplexer.GetEndPoints();
            return _database.Multiplexer.GetServer(endpoints[0]);
        }
    }
}

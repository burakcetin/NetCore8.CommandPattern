// Infrastructure/Cache/MemoryCacheProvider.cs
using NetCore8.CommandPattern.Core.Interfaces;
using StackExchange.Redis;
using System.Text.Json;



namespace NetCore8.CommandPattern.Infrastructure.Cache
{
    public class RedisProvider : ICacheProvider
    {
        private readonly IDatabase _db;
        private readonly IConnectionMultiplexer _redis;

        public RedisProvider(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _db = _redis.GetDatabase();
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);
            if (!value.HasValue)
                return default;

            return JsonSerializer.Deserialize<T>(value);
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            var value = await _db.StringGetAsync(key);
            if (value.HasValue)
                return JsonSerializer.Deserialize<T>(value);

            var result = await factory();
            await SetAsync(key, result, expiration);
            return result;
        }

        public Task RemoveAsync(string key)
        {
            return _db.KeyDeleteAsync(key);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            return _db.StringSetAsync(key, serializedValue, expiration);
        }
    }
}

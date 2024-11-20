
using NetCore8.CommandPattern.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace NetCore8.CommandPattern.Application.Infrastructure
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;

       

        public MemoryCacheProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<T> GetAsync<T>(string key)
        {
            return Task.FromResult(_cache.Get<T>(key));
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            if (_cache.TryGetValue<T>(key, out var value))
                return value;

            value = await factory();
            var cacheOptions = new MemoryCacheEntryOptions();
            
            if (expiration.HasValue)
                cacheOptions.SetAbsoluteExpiration(expiration.Value);

            _cache.Set(key, value, cacheOptions);
            return value;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
                options.SetAbsoluteExpiration(expiration.Value);

            _cache.Set(key, value, options);
            return Task.CompletedTask;
        }
    }
}

using System;
using System.Threading.Tasks;

namespace CommandPatternDemo.Core.Interfaces
{
    public interface ICacheProvider
    {
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
    }
}
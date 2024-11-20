
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace NetCore8.CommandPattern.Core.Models
{
    public class CommandContext
    {
        private readonly Dictionary<string, object> _contextData = new();

        public IServiceProvider ServiceProvider { get; }
        public DbContext DbContext { get; }
        public ILogger Logger { get; }
        public ClaimsPrincipal User { get; }

        public CommandContext(
            IServiceProvider serviceProvider,
            DbContext dbContext,
            ILogger logger,
            ClaimsPrincipal user)
        {
            ServiceProvider = serviceProvider;
            DbContext = dbContext;
            Logger = logger;
            User = user;
        }

        public void SetData(string key, object value) => _contextData[key] = value;
        public T GetData<T>(string key) => _contextData.TryGetValue(key, out var value) ? (T)value : default;
        public bool HasData(string key) => _contextData.ContainsKey(key);
    }
}
 
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using NetCore8.CommandPattern.Core.Interfaces;
using NetCore8.CommandPattern.Core.Metrics;
using NetCore8.CommandPattern.Infrastructure.Metrics;
  
using Scrutor;
using NetCore8.CommandPattern.Infrastructure.Cache;
using Microsoft.Extensions.Logging;
using NetCore8.CommandPattern.Core.Commands.Interfaces;

namespace NetCore8.CommandPattern.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandPattern(
            this IServiceCollection services,
            Action<CommandPatternOptions> configureOptions = null)
        {
            var options = new CommandPatternOptions();
            configureOptions?.Invoke(options);

            // Core Command Pattern Services
            services.AddScoped<ICommandHandler, CommandHandler>();
            services.AddScoped<ITransactionManager, TransactionManager>();
            services.AddScoped<AuthorizationHandler>();

            // Event Publisher
            services.AddScoped<IEventPublisher, EventPublisher>();

            // Cache Provider
            if (options.UseRedisCache)
            {
                services.AddSingleton<ICacheProvider>(sp =>
                    new RedisProvider(options.RedisConnectionString));
            }
            else
            {
                services.AddMemoryCache();
                services.AddSingleton<ICacheProvider, MemoryCacheProvider>();
            }

            // Metrics
            services.AddSingleton<IMetricsPublisher, PrometheusMetricsPublisher>();
            services.AddSingleton<IMetricsRegistry, MetricsRegistry>();

            // Logging
            services.AddLogging(configure =>
            {
                configure.AddConsole();
                configure.AddDebug();
            });

            return services;
        }
    }
    public class CommandPatternOptions
    {
        public bool UseRedisCache { get; set; } = false;
        public string RedisConnectionString { get; set; }
    }
}
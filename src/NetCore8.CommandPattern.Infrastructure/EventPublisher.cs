
using Microsoft.Extensions.Logging;
using NetCore8.CommandPattern.Core.Commands.Interfaces;

namespace NetCore8.CommandPattern.Infrastructure
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(ILogger<EventPublisher> logger)
        {
            _logger = logger;
        }

        public async Task PublishAsync<T>(T @event) where T : class
        {
            try
            {
                // Burada event bus (RabbitMQ, Azure Service Bus vb.) implementasyonu yapılabilir
                _logger.LogInformation($"Event published: {typeof(T).Name}");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error publishing event {typeof(T).Name}");
                throw;
            }
        }
    }
}
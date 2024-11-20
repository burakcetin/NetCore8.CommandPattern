
using NetCore8.CommandPattern.Core.Commands;
using NetCore8.CommandPattern.Core.Metrics;

namespace NetCore8.CommandPattern.Infrastructure.Metrics
{
    public class MetricsRegistry : IMetricsRegistry
    {
        private readonly IMetricsPublisher _publisher;

        public MetricsRegistry(IMetricsPublisher publisher)
        {
            _publisher = publisher;
        }

        public void IncrementCommandExecution<TCommand>() where TCommand : ICommand
        {
            _publisher.Increment("command_executions_total", new[] { typeof(TCommand).Name });
        }

        public void IncrementCommandSuccess<TCommand>() where TCommand : ICommand
        {
            _publisher.Increment("command_successes_total", new[] { typeof(TCommand).Name });
        }

        public void IncrementCommandFailure<TCommand>() where TCommand : ICommand
        {
            _publisher.Increment("command_failures_total", new[] { typeof(TCommand).Name });
        }

        public void RecordCommandDuration<TCommand>(TimeSpan duration) where TCommand : ICommand
        {
            _publisher.Record("command_duration_seconds", duration.TotalSeconds, new[] { typeof(TCommand).Name });
        }

        public void RecordCommandSize<TCommand>(long bytes) where TCommand : ICommand
        {
            _publisher.Record("command_size_bytes", bytes, new[] { typeof(TCommand).Name });
        }
    }
}
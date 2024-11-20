
using NetCore8.CommandPattern.Core.Commands;
using NetCore8.CommandPattern.Core.Interfaces;
using NetCore8.CommandPattern.Core.Metrics;
using System.Diagnostics;

namespace NetCore8.CommandPattern.Infrastructure
{
    public class MetricsDecoratedCommandHandler : ICommandHandler
    {
        private readonly ICommandHandler _innerHandler;
        private readonly IMetricsRegistry _metrics;

        public MetricsDecoratedCommandHandler(ICommandHandler innerHandler, IMetricsRegistry metrics)
        {
            _innerHandler = innerHandler;
            _metrics = metrics;
        }

        public async Task<object> HandleAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            var stopwatch = Stopwatch.StartNew();
            _metrics.IncrementCommandExecution<TCommand>();

            try
            {
                var serializedSize = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(command).Length;
                _metrics.RecordCommandSize<TCommand>(serializedSize);

                var result = await _innerHandler.HandleAsync(command);
                stopwatch.Stop();

                _metrics.IncrementCommandSuccess<TCommand>();
                _metrics.RecordCommandDuration<TCommand>(stopwatch.Elapsed);

                return result;
            }
            catch
            {
                _metrics.IncrementCommandFailure<TCommand>();
                throw;
            }
        }
    }
}
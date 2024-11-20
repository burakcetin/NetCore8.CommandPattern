
using NetCore8.CommandPattern.Core.Commands;

namespace NetCore8.CommandPattern.Core.Metrics
{
    public interface IMetricsRegistry
    {
        void IncrementCommandExecution<TCommand>() where TCommand : ICommand;
        void IncrementCommandSuccess<TCommand>() where TCommand : ICommand;
        void IncrementCommandFailure<TCommand>() where TCommand : ICommand;
        void RecordCommandDuration<TCommand>(TimeSpan duration) where TCommand : ICommand;
        void RecordCommandSize<TCommand>(long bytes) where TCommand : ICommand;
    }
}
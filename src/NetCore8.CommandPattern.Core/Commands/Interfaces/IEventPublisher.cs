
namespace NetCore8.CommandPattern.Core.Commands.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event) where T : class;
    }
}
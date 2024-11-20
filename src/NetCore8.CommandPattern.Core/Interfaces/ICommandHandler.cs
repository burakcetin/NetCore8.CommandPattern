
using NetCore8.CommandPattern.Core.Commands;
using System.Threading.Tasks;

namespace NetCore8.CommandPattern.Core.Interfaces
{
    public interface ICommandHandler
    {
        Task<object> HandleAsync<TCommand>(TCommand command) where TCommand : ICommand;
    }
}

using NetCore8.CommandPattern.Core.Models;
using System.Threading.Tasks;

namespace NetCore8.CommandPattern.Core.Commands
{
    public interface ICommand
    {
        Task<object> ExecuteAsync(CommandContext context);
    }
}
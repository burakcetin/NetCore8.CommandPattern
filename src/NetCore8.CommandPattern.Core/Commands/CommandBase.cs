
using NetCore8.CommandPattern.Core.Models;
using System.Threading.Tasks;

namespace NetCore8.CommandPattern.Core.Commands
{
    public abstract class CommandBase : ICommand
    {
        public abstract Task<object> ExecuteAsync(CommandContext context);
    }
}
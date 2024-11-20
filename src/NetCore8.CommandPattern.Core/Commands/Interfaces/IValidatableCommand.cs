
using NetCore8.CommandPattern.Core.Models;

namespace NetCore8.CommandPattern.Core.Commands.Interfaces
{
    public interface IValidatableCommand 
    {
        Task ValidateAsync(CommandContext context);
    }
}
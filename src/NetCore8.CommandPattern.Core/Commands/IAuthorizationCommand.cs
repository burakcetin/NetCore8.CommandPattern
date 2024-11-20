
namespace NetCore8.CommandPattern.Core.Commands
{
    public interface IAuthorizationCommand : ICommand
    {
        string[] RequiredPermissions { get; }
    }
}
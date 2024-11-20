
namespace CommandPatternDemo.Core.Commands
{
    public interface IAuthorizationCommand : ICommand
    {
        string[] RequiredPermissions { get; }
    }
}
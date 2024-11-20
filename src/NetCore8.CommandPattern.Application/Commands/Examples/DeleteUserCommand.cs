
using NetCore8.CommandPattern.Application.Exceptions;
using NetCore8.CommandPattern.Core.Commands;
using NetCore8.CommandPattern.Core.Models;
using System.Threading.Tasks;

namespace NetCore8.CommandPattern.Application.Commands.Examples
{
    public class DeleteUserCommand : CommandBase, IAuthorizationCommand
    {
        public int UserId { get; }
        public string[] RequiredPermissions => new[] { "Users.Delete" };

        public DeleteUserCommand(int userId)
        {
            UserId = userId;
        }

        public override async Task<object> ExecuteAsync(CommandContext context)
        {
            var user = await context.DbContext.Set<User>()
                .FindAsync(UserId);

            if (user == null)
                throw new NotFoundException($"User with id {UserId} not found");

            context.DbContext.Set<User>().Remove(user);
            return user;
        }
    }
}
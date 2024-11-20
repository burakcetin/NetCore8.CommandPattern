
using NetCore8.CommandPattern.Application.Exceptions;
using NetCore8.CommandPattern.Core.Commands;
using NetCore8.CommandPattern.Core.Models;
using System.Threading.Tasks;

namespace NetCore8.CommandPattern.Application.Commands.Examples
{
    public class UpdateUserCommand : CommandBase, IAuthorizationCommand
    {
        public int UserId { get; }
        public string NewEmail { get; }
        public string[] RequiredPermissions => new[] { "Users.Update" };

        public UpdateUserCommand(int userId, string newEmail)
        {
            UserId = userId;
            NewEmail = newEmail;
        }

        public override async Task<object> ExecuteAsync(CommandContext context)
        {
            var user = await context.DbContext.Set<User>()
                .FindAsync(UserId);

            if (user == null)
                throw new NotFoundException($"User with id {UserId} not found");

            user.Email = NewEmail;
            return user;
        }
    }
}
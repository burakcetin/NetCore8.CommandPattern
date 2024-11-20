
using NetCore8.CommandPattern.Core.Commands;
using NetCore8.CommandPattern.Core.Models;
using System;
using System.Threading.Tasks;

namespace NetCore8.CommandPattern.Application.Commands
{
    public class CreateUserCommand : CommandBase, IAuthorizationCommand, ICacheableCommand
    {
        public string Username { get; }
        public string Email { get; }
        public string[] RequiredPermissions => new[] { "Users.Create" };
        public string CacheKey => $"user_create_{Username}_{Email}";
        public TimeSpan CacheDuration => TimeSpan.FromMinutes(5);

        public CreateUserCommand(string username, string email)
        {
            Username = username;
            Email = email;
        }

        public override async Task<object> ExecuteAsync(CommandContext context)
        {
            var user = new User { Username = Username, Email = Email };
            context.DbContext.Set<User>().Add(user);
            return user;
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
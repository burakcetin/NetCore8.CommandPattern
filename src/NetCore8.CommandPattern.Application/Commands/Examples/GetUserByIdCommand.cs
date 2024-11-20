
using NetCore8.CommandPattern.Application.Exceptions;
using NetCore8.CommandPattern.Core.Commands;
using NetCore8.CommandPattern.Core.Models;
using System;
using System.Threading.Tasks;

namespace NetCore8.CommandPattern.Application.Commands.Examples
{
    public class GetUserByIdCommand : CommandBase, ICacheableCommand
    {
        public int UserId { get; }
        public string CacheKey => $"user_{UserId}";
        public TimeSpan CacheDuration => TimeSpan.FromMinutes(10);

        public GetUserByIdCommand(int userId)
        {
            UserId = userId;
        }

        public override async Task<object> ExecuteAsync(CommandContext context)
        {
            var user = await context.DbContext.Set<User>()
                .FindAsync(UserId);

            if (user == null)
                throw new NotFoundException($"User with id {UserId} not found");

            return user;
        }
    }
}

using NetCore8.CommandPattern.Core.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using NetCore8.CommandPattern.Core.Models;

namespace NetCore8.CommandPattern.Application.Commands.Examples
{
    public class ListUsersCommand : CommandBase, ICacheableCommand
    {
        public int Page { get; }
        public int PageSize { get; }
        public string CacheKey => $"users_page_{Page}_size_{PageSize}";
        public TimeSpan CacheDuration => TimeSpan.FromMinutes(5);

        public ListUsersCommand(int page = 1, int pageSize = 10)
        {
            Page = page;
            PageSize = pageSize;
        }

        public override async Task<object> ExecuteAsync(CommandContext context)
        {
            var users = await context.DbContext.Set<User>()
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return new 
            {
                Items = users,
                Total = await context.DbContext.Set<User>().CountAsync(),
                Page,
                PageSize
            };
        }
    }
}

using Microsoft.EntityFrameworkCore;
using NetCore8.CommandPattern.Core.Commands;
using NetCore8.CommandPattern.Core.Commands.Interfaces;
using NetCore8.CommandPattern.Core.Models;
using NetCore8.CommandPattern.Infrastructure.Entities;

namespace NetCore8.CommandPattern.Application.Commands
{
    public class ListOrdersCommand : CommandBase, ICacheableCommand
    {
        public int Page { get; }
        public int PageSize { get; }
        public string CacheKey => $"orders_page_{Page}_size_{PageSize}";
        public TimeSpan CacheDuration => TimeSpan.FromMinutes(5);

        public ListOrdersCommand(int page = 1, int pageSize = 10)
        {
            Page = page;
            PageSize = pageSize;
        }

        public override async Task<object> ExecuteAsync(CommandContext context)
        {
            var query = context.DbContext.Set<Order>().AsNoTracking();
            
            var totalItems = await query.CountAsync();
            var items = await query
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return new 
            {
                Items = items,
                TotalItems = totalItems,
                Page,
                PageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize)
            };
        }
    }
}
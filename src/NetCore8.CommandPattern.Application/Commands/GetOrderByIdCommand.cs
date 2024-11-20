
using Microsoft.EntityFrameworkCore;
using NetCore8.CommandPattern.Application.Exceptions;
using NetCore8.CommandPattern.Core.Commands;
using NetCore8.CommandPattern.Core.Commands.Interfaces;
using NetCore8.CommandPattern.Core.Models;
using NetCore8.CommandPattern.Infrastructure.Entities;

namespace NetCore8.CommandPattern.Application.Commands
{
    public class GetOrderByIdCommand : CommandBase, ICacheableCommand
    {
        public int OrderId { get; }
        public string CacheKey => $"order_{OrderId}";
        public TimeSpan CacheDuration => TimeSpan.FromMinutes(10);

        public GetOrderByIdCommand(int orderId)
        {
            OrderId = orderId;
        }

        public override async Task<object> ExecuteAsync(CommandContext context)
        {
            var order = await context.DbContext.Set<Order>()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == OrderId);

            if (order == null)
                throw new NotFoundException($"Order {OrderId} not found");

            return order;
        }
    }
}
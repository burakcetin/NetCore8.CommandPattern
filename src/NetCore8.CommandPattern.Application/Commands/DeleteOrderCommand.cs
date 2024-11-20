
using NetCore8.CommandPattern.Application.Exceptions;
using NetCore8.CommandPattern.Core.Commands;
using NetCore8.CommandPattern.Core.Commands.Interfaces;
using NetCore8.CommandPattern.Core.Models;
using NetCore8.CommandPattern.Infrastructure.Entities;

namespace NetCore8.CommandPattern.Application.Commands
{
    public class DeleteOrderCommand : CommandBase, IAuthorizationCommand
    {
        public int OrderId { get; }
        public string[] RequiredPermissions => new[] { "Orders.Delete" };

        public DeleteOrderCommand(int orderId)
        {
            OrderId = orderId;
        }

        public override async Task<object> ExecuteAsync(CommandContext context)
        {
            var order = await context.DbContext.Set<Order>().FindAsync(OrderId);
            
            if (order == null)
                throw new NotFoundException($"Order {OrderId} not found");

            context.DbContext.Set<Order>().Remove(order);
            return order;
        }
    }
}
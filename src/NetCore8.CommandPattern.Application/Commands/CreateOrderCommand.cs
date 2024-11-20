
using NetCore8.CommandPattern.Core.Commands;
using NetCore8.CommandPattern.Core.Commands.Interfaces;
using NetCore8.CommandPattern.Core.Models;
using NetCore8.CommandPattern.Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;

namespace NetCore8.CommandPattern.Application.Commands
{
    public class CreateOrderCommand : CommandBase, IValidatableCommand
    {
        public int UserId { get; }
        public List<OrderItem> Items { get; }

        public CreateOrderCommand(int userId, List<OrderItem> items)
        {
            UserId = userId;
            Items = items;
        }

        public async Task ValidateAsync(CommandContext context)
        {
            var user = await context.DbContext.Set<User>().FindAsync(UserId);
            if (user == null)
                throw new ValidationException($"User {UserId} not found");

            if (!Items.Any())
                throw new ValidationException("Order must contain at least one item");
        }

        public override async Task<object> ExecuteAsync(CommandContext context)
        {
            var order = new Order 
            { 
                UserId = UserId, 
                Items = Items,
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow
            };

            context.DbContext.Set<Order>().Add(order);
            return order;
        }
    }
}
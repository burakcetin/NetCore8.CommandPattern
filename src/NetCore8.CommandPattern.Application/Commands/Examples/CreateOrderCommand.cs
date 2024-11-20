
using Microsoft.EntityFrameworkCore;
using NetCore8.CommandPattern.Core.Commands;
using NetCore8.CommandPattern.Core.Commands.Interfaces;
using NetCore8.CommandPattern.Core.Models;
using NetCore8.CommandPattern.Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;

namespace NetCore8.CommandPattern.Application.Commands.Examples
{
    public class CreateOrderCommand : CommandBase, IValidatableCommand, IRetryableCommand
    {
        public int UserId { get; }
        public List<OrderItem> Items { get; }
        public int MaxRetries => 3;
        public TimeSpan RetryDelay => TimeSpan.FromSeconds(1);

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

        public bool ShouldRetry(Exception ex) => ex is DbUpdateConcurrencyException;

        public override async Task<object> ExecuteAsync(CommandContext context)
        {
            var order = new Order { UserId = UserId, Items = Items };
            context.DbContext.Set<Order>().Add(order);
            
            // Set context data for other commands
            context.SetData("LastOrderId", order.Id);
            
            return order;
        }
    }
}
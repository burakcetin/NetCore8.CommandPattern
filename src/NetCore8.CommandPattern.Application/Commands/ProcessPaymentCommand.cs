
using Microsoft.EntityFrameworkCore;
using NetCore8.CommandPattern.Application.Exceptions;
using NetCore8.CommandPattern.Core.Commands;
using NetCore8.CommandPattern.Core.Commands.Interfaces;
using NetCore8.CommandPattern.Core.Models;
using NetCore8.CommandPattern.Infrastructure.Entities;

namespace NetCore8.CommandPattern.Application.Commands
{
    public class ProcessPaymentCommand : CommandBase, IPrioritizedCommand, IRetryableCommand
    {
        private readonly CommandContext _context;
        public CommandPriority Priority => CommandPriority.High;
        public int OrderId { get; }
        
        public int MaxRetries => 3;
        public TimeSpan RetryDelay => TimeSpan.FromSeconds(1);

        public ProcessPaymentCommand(CommandContext context, int orderId)
        {
            _context = context;
            OrderId = orderId;
        }

        public bool ShouldRetry(Exception ex) => 
            ex is TimeoutException || ex is DbUpdateConcurrencyException;

        public override async Task<object> ExecuteAsync(CommandContext context)
        {
            var order = await context.DbContext.Set<Order>()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == OrderId);

            if (order == null)
                throw new NotFoundException($"Order {OrderId} not found");

            var payment = new Payment 
            { 
                OrderId = OrderId,
                Amount = order.Items.Sum(i => i.Price * i.Quantity),
                ProcessedAt = DateTime.UtcNow,
                Status = PaymentStatus.Completed
            };

            context.DbContext.Set<Payment>().Add(payment);
            order.Status = OrderStatus.PaymentCompleted;

            return payment;
        }
    }
}
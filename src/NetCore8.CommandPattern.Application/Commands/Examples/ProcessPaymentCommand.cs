
using NetCore8.CommandPattern.Application.Exceptions;
using NetCore8.CommandPattern.Core.Commands;
using NetCore8.CommandPattern.Core.Commands.Interfaces;
using NetCore8.CommandPattern.Core.Models;
using NetCore8.CommandPattern.Infrastructure.Entities;

namespace NetCore8.CommandPattern.Application.Commands.Examples
{
    public class ProcessPaymentCommand : CommandBase, IPrioritizedCommand
    {
        private readonly CommandContext _context;
        public CommandPriority Priority => CommandPriority.High;
        public int OrderId { get; }

        public ProcessPaymentCommand(CommandContext context)
        {
            _context = context;
            OrderId = context.GetData<int>("LastOrderId");
        }

        public override async Task<object> ExecuteAsync(CommandContext context)
        {
            var order = await context.DbContext.Set<Order>()
                .FindAsync(OrderId);

            if (order == null)
                throw new NotFoundException($"Order {OrderId} not found");

            var payment = new Payment
            {
                OrderId = OrderId,
                Amount = order.Items.Sum(i => i.Price)
            };

            context.DbContext.Set<Payment>().Add(payment);
            return payment;
        }
    }
}
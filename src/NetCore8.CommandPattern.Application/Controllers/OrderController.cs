
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCore8.CommandPattern.Application.Commands;
using NetCore8.CommandPattern.Application.Models.Requests;
using NetCore8.CommandPattern.Core.Interfaces;
using NetCore8.CommandPattern.Core.Models;
using NetCore8.CommandPattern.Infrastructure.Entities;

namespace NetCore8.CommandPattern.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ICommandHandler _commandHandler;
        private readonly CommandContext _context;

        public OrderController(ICommandHandler commandHandler, CommandContext context)
        {
            _commandHandler = commandHandler;
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var command = new CreateOrderCommand(
                request.UserId,
                request.Items.Select(i => new OrderItem 
                { 
                    ProductId = i.ProductId, 
                    Price = i.Price, 
                    Quantity = i.Quantity 
                }).ToList()
            );

            var result = await _commandHandler.HandleAsync(command);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ResponseCache(Duration = 300)]
        public async Task<IActionResult> GetOrder(int id)
        {
            var command = new GetOrderByIdCommand(id);
            var result = await _commandHandler.HandleAsync(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "OrderUpdate")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderRequest request)
        {
            var command = new UpdateOrderCommand(id, request.Status);
            var result = await _commandHandler.HandleAsync(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "OrderDelete")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var command = new DeleteOrderCommand(id);
            await _commandHandler.HandleAsync(command);
            return NoContent();
        }

        [HttpPost("{id}/process-payment")]
        [Authorize(Policy = "PaymentProcess")]
        public async Task<IActionResult> ProcessPayment(int id, [FromBody] ProcessPaymentRequest request)
        {
            var command = new ProcessPaymentCommand(_context, id);
            var result = await _commandHandler.HandleAsync(command);
            return Ok(result);
        }

        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> ListOrders([FromQuery] OrderListRequest request)
        {
            var command = new ListOrdersCommand(request.Page, request.PageSize);
            var result = await _commandHandler.HandleAsync(command);
            return Ok(result);
        }
    }
}
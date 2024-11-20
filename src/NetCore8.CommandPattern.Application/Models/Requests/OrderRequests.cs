
using NetCore8.CommandPattern.Infrastructure.Entities;

namespace NetCore8.CommandPattern.Application.Models.Requests
{
    public class CreateOrderRequest
    {
        public int UserId { get; set; }
        public List<OrderItemRequest> Items { get; set; }
    }

    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateOrderRequest
    {
        public OrderStatus Status { get; set; }
    }

    public class ProcessPaymentRequest
    {
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
    }

    public class OrderListRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
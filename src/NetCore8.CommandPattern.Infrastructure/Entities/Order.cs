namespace NetCore8.CommandPattern.Infrastructure.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public enum OrderStatus
    {
        Created,
        PaymentPending,
        PaymentCompleted,
        Shipped,
        Delivered,
        Cancelled,
        Completed
    }
}
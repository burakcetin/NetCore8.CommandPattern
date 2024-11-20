 
using Microsoft.EntityFrameworkCore;
using NetCore8.CommandPattern.Core.Models;
using NetCore8.CommandPattern.Infrastructure.Entities;

namespace NetCore8.CommandPattern.Infrastructure.Data.Configurations
{
    public static class SeedData
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            // Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "john.doe",
                    Email = "john.doe@example.com"
                },
                new User
                {
                    Id = 2,
                    Username = "jane.doe",
                    Email = "jane.doe@example.com"
                },
                new User
                {
                    Id = 3,
                    Username = "admin",
                    Email = "admin@example.com"
                }
            );

            // Products (Order Items için)
            var products = new[]
            {
                new { Id = 1, Name = "Laptop", Price = 1299.99m },
                new { Id = 2, Name = "Smartphone", Price = 699.99m },
                new { Id = 3, Name = "Headphones", Price = 199.99m },
                new { Id = 4, Name = "Tablet", Price = 499.99m },
                new { Id = 5, Name = "Smartwatch", Price = 299.99m }
            };

            // Orders
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    UserId = 1,
                    Status = OrderStatus.Completed,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Order
                {
                    Id = 2,
                    UserId = 1,
                    Status = OrderStatus.PaymentPending,
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new Order
                {
                    Id = 3,
                    UserId = 2,
                    Status = OrderStatus.Shipped,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new Order
                {
                    Id = 4,
                    UserId = 3,
                    Status = OrderStatus.Created,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            );

            // Order Items
            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 1,
                    Price = products[0].Price,
                    Quantity = 1
                },
                new OrderItem
                {
                    Id = 2,
                    OrderId = 1,
                    ProductId = 3,
                    Price = products[2].Price,
                    Quantity = 2
                },
                new OrderItem
                {
                    Id = 3,
                    OrderId = 2,
                    ProductId = 2,
                    Price = products[1].Price,
                    Quantity = 1
                },
                new OrderItem
                {
                    Id = 4,
                    OrderId = 3,
                    ProductId = 4,
                    Price = products[3].Price,
                    Quantity = 1
                },
                new OrderItem
                {
                    Id = 5,
                    OrderId = 3,
                    ProductId = 5,
                    Price = products[4].Price,
                    Quantity = 1
                }
            );

            // Payments
            modelBuilder.Entity<Payment>().HasData(
                new Payment
                {
                    Id = 1,
                    OrderId = 1,
                    Amount = 1499.97m, // Laptop + 2 Headphones
                    ProcessedAt = DateTime.UtcNow.AddDays(-5),
                    Status = PaymentStatus.Completed
                },
                new Payment
                {
                    Id = 2,
                    OrderId = 3,
                    Amount = 799.98m, // Tablet + Smartwatch
                    ProcessedAt = DateTime.UtcNow.AddDays(-2),
                    Status = PaymentStatus.Completed
                }
            );

            // Audit Logs
            modelBuilder.Entity<AuditLog>().HasData(
                new AuditLog
                {
                    Id = 1,
                    CommandType = "CreateOrderCommand",
                    CommandData = "{\"UserId\":1,\"Items\":[{\"ProductId\":1,\"Quantity\":1}]}",
                    UserId = "1",
                    ExecutedAt = DateTime.UtcNow.AddDays(-5),
                    IsSuccessful = true
                },
                new AuditLog
                {
                    Id = 2,
                    CommandType = "ProcessPaymentCommand",
                    CommandData = "{\"OrderId\":1,\"Amount\":1499.97}",
                    UserId = "1",
                    ExecutedAt = DateTime.UtcNow.AddDays(-5),
                    IsSuccessful = true
                },
                new AuditLog
                {
                    Id = 3,
                    CommandType = "UpdateOrderCommand",
                    CommandData = "{\"OrderId\":1,\"Status\":\"Completed\"}",
                    UserId = "1",
                    ExecutedAt = DateTime.UtcNow.AddDays(-5),
                    IsSuccessful = true
                }
            );
        }
    }
}
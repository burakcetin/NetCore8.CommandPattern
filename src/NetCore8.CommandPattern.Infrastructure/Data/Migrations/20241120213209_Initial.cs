using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NetCore8.CommandPattern.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommandType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommandData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AuditLogs",
                columns: new[] { "Id", "CommandData", "CommandType", "ErrorMessage", "ExecutedAt", "IsSuccessful", "UserId" },
                values: new object[,]
                {
                    { 1, "{\"UserId\":1,\"Items\":[{\"ProductId\":1,\"Quantity\":1}]}", "CreateOrderCommand", null, new DateTime(2024, 11, 15, 21, 32, 9, 279, DateTimeKind.Utc).AddTicks(2561), true, "1" },
                    { 2, "{\"OrderId\":1,\"Amount\":1499.97}", "ProcessPaymentCommand", null, new DateTime(2024, 11, 15, 21, 32, 9, 279, DateTimeKind.Utc).AddTicks(2562), true, "1" },
                    { 3, "{\"OrderId\":1,\"Status\":\"Completed\"}", "UpdateOrderCommand", null, new DateTime(2024, 11, 15, 21, 32, 9, 279, DateTimeKind.Utc).AddTicks(2563), true, "1" }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CreatedAt", "Status", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 11, 15, 21, 32, 9, 279, DateTimeKind.Utc).AddTicks(2464), 6, 1 },
                    { 2, new DateTime(2024, 11, 17, 21, 32, 9, 279, DateTimeKind.Utc).AddTicks(2470), 1, 1 },
                    { 3, new DateTime(2024, 11, 18, 21, 32, 9, 279, DateTimeKind.Utc).AddTicks(2472), 3, 2 },
                    { 4, new DateTime(2024, 11, 19, 21, 32, 9, 279, DateTimeKind.Utc).AddTicks(2473), 0, 3 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Username" },
                values: new object[,]
                {
                    { 1, "john.doe@example.com", "john.doe" },
                    { 2, "jane.doe@example.com", "jane.doe" },
                    { 3, "admin@example.com", "admin" }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "OrderId", "Price", "ProductId", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 1299.99m, 1, 1 },
                    { 2, 1, 199.99m, 3, 2 },
                    { 3, 2, 699.99m, 2, 1 },
                    { 4, 3, 499.99m, 4, 1 },
                    { 5, 3, 299.99m, 5, 1 }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "OrderId", "ProcessedAt", "Status" },
                values: new object[,]
                {
                    { 1, 1499.97m, 1, new DateTime(2024, 11, 15, 21, 32, 9, 279, DateTimeKind.Utc).AddTicks(2531), 1 },
                    { 2, 799.98m, 3, new DateTime(2024, 11, 18, 21, 32, 9, 279, DateTimeKind.Utc).AddTicks(2532), 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}

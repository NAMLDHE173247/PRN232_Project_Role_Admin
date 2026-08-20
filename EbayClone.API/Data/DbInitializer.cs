using EbayClone.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EbayClone.API.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        await dbContext.Database.MigrateAsync();
        var admin = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == "admin@gmail.com");
        if (admin is null)
        {
            admin = new User
            {
                Email = "admin@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FullName = "System Admin",
                Role = "Admin",
                Status = UserStatus.Active,
                ApprovalStatus = "Approved",
                ApprovedAt = DateTime.UtcNow
            };
            dbContext.Users.Add(admin);
            await dbContext.SaveChangesAsync();
        }

        if (await dbContext.Users.AnyAsync(x => x.Email == "demo.buyer@example.com")) return;

        var seller = new User
        {
            Email = "demo.seller@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo@123"),
            FullName = "Demo Seller",
            Role = "User",
            Status = UserStatus.Active,
            ApprovalStatus = "Approved",
            ApprovedBy = admin.Id,
            ApprovedAt = DateTime.UtcNow.AddDays(-20)
        };
        var buyer = new User
        {
            Email = "demo.buyer@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo@123"),
            FullName = "Demo Buyer",
            Role = "User",
            Status = UserStatus.Active,
            ApprovalStatus = "Approved",
            ApprovedBy = admin.Id,
            ApprovedAt = DateTime.UtcNow.AddDays(-18)
        };
        var pending = new User
        {
            Email = "demo.pending@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo@123"),
            FullName = "Demo Pending User",
            Role = "User",
            Status = UserStatus.Pending,
            ApprovalStatus = "PendingApproval"
        };
        dbContext.Users.AddRange(seller, buyer, pending);
        await dbContext.SaveChangesAsync();

        var headphones = new Product
        {
            Name = "Demo Wireless Headphones",
            Price = 129.99m,
            SellerId = seller.Id,
            Status = ProductStatus.Active
        };
        var keyboard = new Product
        {
            Name = "Demo Mechanical Keyboard",
            Price = 89.50m,
            SellerId = seller.Id,
            Status = ProductStatus.Active
        };
        var hiddenProduct = new Product
        {
            Name = "Demo Product - Hidden For Review",
            Price = 49.00m,
            SellerId = seller.Id,
            Status = ProductStatus.Hidden
        };
        dbContext.Products.AddRange(headphones, keyboard, hiddenProduct);
        await dbContext.SaveChangesAsync();

        var completedOrder = new OrderTable
        {
            BuyerId = buyer.Id,
            OrderDate = DateTime.UtcNow.AddDays(-7),
            TotalPrice = 219.49m,
            Status = "Completed"
        };
        var pendingOrder = new OrderTable
        {
            BuyerId = buyer.Id,
            OrderDate = DateTime.UtcNow.AddDays(-1),
            TotalPrice = 89.50m,
            Status = "Pending"
        };
        dbContext.Orders.AddRange(completedOrder, pendingOrder);
        await dbContext.SaveChangesAsync();

        dbContext.OrderItems.AddRange(
            new OrderItem { OrderId = completedOrder.Id, ProductId = headphones.Id, Quantity = 1, UnitPrice = headphones.Price },
            new OrderItem { OrderId = completedOrder.Id, ProductId = keyboard.Id, Quantity = 1, UnitPrice = keyboard.Price },
            new OrderItem { OrderId = pendingOrder.Id, ProductId = keyboard.Id, Quantity = 1, UnitPrice = keyboard.Price });
        dbContext.Payments.AddRange(
            new Payment { OrderId = completedOrder.Id, UserId = buyer.Id, Amount = completedOrder.TotalPrice, Method = "DemoCard", Status = "Paid", PaidAt = DateTime.UtcNow.AddDays(-7) },
            new Payment { OrderId = pendingOrder.Id, UserId = buyer.Id, Amount = pendingOrder.TotalPrice, Method = "DemoCard", Status = "Pending" });
        dbContext.ShippingInfos.AddRange(
            new ShippingInfo { OrderId = completedOrder.Id, Carrier = "Demo Express", TrackingNumber = "DEMO-TRACK-001", Status = "Delivered", EstimatedArrival = DateTime.UtcNow.AddDays(-3) },
            new ShippingInfo { OrderId = pendingOrder.Id, Carrier = "Demo Express", TrackingNumber = "DEMO-TRACK-002", Status = "Preparing", EstimatedArrival = DateTime.UtcNow.AddDays(3) });
        await dbContext.SaveChangesAsync();

        var assignedDispute = new Dispute
        {
            OrderId = completedOrder.Id,
            RaisedBy = buyer.Id,
            Description = "Demo dispute assigned to the admin team for review.",
            Status = nameof(DisputeStatus.Assigned),
            AssignedTo = admin.Id,
            AssignedAt = DateTime.UtcNow.AddDays(-2)
        };
        var openDispute = new Dispute
        {
            OrderId = pendingOrder.Id,
            RaisedBy = buyer.Id,
            Description = "Demo open dispute waiting for admin review.",
            Status = nameof(DisputeStatus.Open)
        };
        dbContext.Disputes.AddRange(assignedDispute, openDispute);
        dbContext.AuditLogs.AddRange(
            new AuditLog
            {
                ActorId = admin.Id,
                Action = "HIDE_PRODUCT",
                Resource = "PRODUCT",
                ResourceId = hiddenProduct.Id,
                Metadata = "{\"seed\":true,\"reason\":\"demo moderation data\"}",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-3)
            },
            new AuditLog
            {
                ActorId = admin.Id,
                Action = "ASSIGN_DISPUTE",
                Resource = "DISPUTE",
                ResourceId = assignedDispute.Id,
                Metadata = "{\"seed\":true,\"assignedTo\":1}",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
            });
        await dbContext.SaveChangesAsync();
    }
}

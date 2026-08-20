using EbayClone.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EbayClone.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<OrderTable> Orders => Set<OrderTable>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ShippingInfo> ShippingInfos => Set<ShippingInfo>();
    public DbSet<Dispute> Disputes => Set<Dispute>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(100);
            entity.Property(x => x.PasswordHash).HasColumnName("password").HasMaxLength(255);
            entity.Property(x => x.FullName).HasColumnName("username").HasMaxLength(100);
            entity.Property(x => x.Role).HasColumnName("role").HasMaxLength(20);
            entity.Property(x => x.Status).HasColumnName("Status").HasConversion<string>().HasMaxLength(20);
            entity.Property(x => x.ApprovalStatus).HasColumnName("ApprovalStatus").HasMaxLength(30);
            entity.Property(x => x.BannedReason).HasColumnName("BannedReason");
            entity.Property(x => x.ApprovedAt).HasColumnName("ApprovedAt");
            entity.Property(x => x.BannedAt).HasColumnName("BannedAt");
            entity.Property(x => x.ApprovedBy).HasColumnName("ApprovedBy");
            entity.Property(x => x.BannedBy).HasColumnName("BannedBy");
            entity.Ignore(x => x.CreatedAtUtc);
        });
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");
            entity.Property(x => x.Name).HasColumnName("title").HasMaxLength(255);
            entity.Property(x => x.Price).HasColumnName("price").HasPrecision(10, 2);
            entity.Property(x => x.SellerId).HasColumnName("sellerId");
            entity.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20);
            entity.Ignore(x => x.CreatedAtUtc);
        });
        modelBuilder.Entity<OrderTable>(entity =>
        {
            entity.ToTable("OrderTable");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.BuyerId).HasColumnName("buyerId");
            entity.Property(x => x.AddressId).HasColumnName("addressId");
            entity.Property(x => x.TotalPrice).HasColumnName("totalPrice").HasPrecision(10, 2);
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(50);
            entity.Property(x => x.OrderDate).HasColumnName("orderDate");
            entity.Ignore(x => x.Address);
            entity.Ignore(x => x.Buyer);
            entity.Ignore(x => x.Disputes);
            entity.Ignore(x => x.OrderItems);
            entity.Ignore(x => x.Payments);
            entity.Ignore(x => x.ReturnRequests);
            entity.Ignore(x => x.ShippingInfos);
        });
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItem");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.OrderId).HasColumnName("orderId");
            entity.Property(x => x.ProductId).HasColumnName("productId");
            entity.Property(x => x.Quantity).HasColumnName("quantity");
            entity.Property(x => x.UnitPrice).HasColumnName("unitPrice").HasPrecision(10, 2);
            entity.Ignore(x => x.Order);
            entity.Ignore(x => x.Product);
        });
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.OrderId).HasColumnName("orderId");
            entity.Property(x => x.UserId).HasColumnName("userId");
            entity.Property(x => x.Amount).HasColumnName("amount").HasPrecision(10, 2);
            entity.Property(x => x.Method).HasColumnName("method");
            entity.Property(x => x.Status).HasColumnName("status");
            entity.Property(x => x.PaidAt).HasColumnName("paidAt");
            entity.Ignore(x => x.Order);
            entity.Ignore(x => x.User);
        });
        modelBuilder.Entity<ShippingInfo>(entity =>
        {
            entity.ToTable("ShippingInfo");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.OrderId).HasColumnName("orderId");
            entity.Property(x => x.Carrier).HasColumnName("carrier");
            entity.Property(x => x.TrackingNumber).HasColumnName("trackingNumber");
            entity.Property(x => x.Status).HasColumnName("status");
            entity.Property(x => x.EstimatedArrival).HasColumnName("estimatedArrival");
            entity.Ignore(x => x.Order);
        });
        modelBuilder.Entity<Dispute>(entity =>
        {
            entity.ToTable("Dispute");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.OrderId).HasColumnName("orderId");
            entity.Property(x => x.RaisedBy).HasColumnName("raisedBy");
            entity.Property(x => x.Description).HasColumnName("description");
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            entity.Property(x => x.Resolution).HasColumnName("resolution");
            entity.Property(x => x.AssignedTo).HasColumnName("assignedTo");
            entity.Property(x => x.AssignedAt).HasColumnName("assignedAt");
            entity.Property(x => x.ResolvedBy).HasColumnName("resolvedBy");
            entity.Property(x => x.ResolvedAt).HasColumnName("resolvedAt");
            entity.Ignore(x => x.Order);
            entity.Ignore(x => x.RaisedByNavigation);
        });
        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("Review");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ProductId).HasColumnName("productId");
            entity.Property(x => x.ReviewerId).HasColumnName("reviewerId");
            entity.Property(x => x.Rating).HasColumnName("rating");
            entity.Property(x => x.Comment).HasColumnName("comment");
            entity.Property(x => x.CreatedAt).HasColumnName("createdAt");
            entity.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20);
            entity.Ignore(x => x.Product);
            entity.Ignore(x => x.Reviewer);
        });
        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.ToTable("Feedback");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.SellerId).HasColumnName("sellerId");
            entity.Property(x => x.AverageRating).HasColumnName("averageRating").HasPrecision(3, 2);
            entity.Property(x => x.TotalReviews).HasColumnName("totalReviews");
            entity.Property(x => x.PositiveRate).HasColumnName("positiveRate").HasPrecision(5, 2);
            entity.Ignore(x => x.Seller);
        });
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLog");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Action).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Resource).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Metadata).HasColumnType("nvarchar(max)");
        });
        modelBuilder.Entity<User>().HasMany(x => x.Products).WithOne(x => x.Seller).HasForeignKey(x => x.SellerId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<User>().Ignore(x => x.Orders);
    }
}

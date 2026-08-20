using EbayClone.API.Data;
using EbayClone.API.DTOs.Orders;
using Microsoft.EntityFrameworkCore;

namespace EbayClone.API.Repositories;

public class OrderRepository(AppDbContext dbContext) : IOrderRepository
{
    public async Task<(int Total, IReadOnlyList<OrderAdminDto> Items)> GetPageAsync(
        string? status,
        DateTime? from,
        DateTime? to,
        int? buyerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var orders = dbContext.Orders.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
            orders = orders.Where(order => order.Status == status.Trim());
        if (from.HasValue)
            orders = orders.Where(order => order.OrderDate >= from.Value);
        if (to.HasValue)
            orders = orders.Where(order => order.OrderDate <= to.Value);
        if (buyerId.HasValue)
            orders = orders.Where(order => order.BuyerId == buyerId.Value);

        var total = await orders.CountAsync(cancellationToken);
        var items = await (
            from order in orders
            join buyer in dbContext.Users.AsNoTracking()
                on order.BuyerId equals (int?)buyer.Id into buyers
            from buyer in buyers.DefaultIfEmpty()
            orderby order.Id descending
            select new OrderAdminDto(
                order.Id,
                order.BuyerId,
                buyer == null ? null : buyer.FullName,
                order.Status,
                order.TotalPrice ?? 0m,
                order.OrderDate))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (total, items);
    }

    public async Task<OrderDetailAdminDto?> GetDetailAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await (
            from currentOrder in dbContext.Orders.AsNoTracking()
            join buyer in dbContext.Users.AsNoTracking()
                on currentOrder.BuyerId equals (int?)buyer.Id into buyers
            from buyer in buyers.DefaultIfEmpty()
            where currentOrder.Id == id
            select new
            {
                currentOrder.Id,
                currentOrder.BuyerId,
                BuyerName = buyer == null ? null : buyer.FullName,
                BuyerEmail = buyer == null ? null : buyer.Email,
                currentOrder.Status,
                currentOrder.TotalPrice,
                currentOrder.OrderDate
            }).FirstOrDefaultAsync(cancellationToken);

        if (order is null) return null;

        var items = await (
            from item in dbContext.OrderItems.AsNoTracking()
            join product in dbContext.Products.AsNoTracking()
                on item.ProductId equals (int?)product.Id into products
            from product in products.DefaultIfEmpty()
            where item.OrderId == id
            orderby item.Id
            select new OrderItemAdminDto(
                item.Id,
                item.ProductId,
                product == null ? null : product.Name,
                item.Quantity ?? 0,
                item.UnitPrice ?? 0m))
            .ToListAsync(cancellationToken);

        var payments = await dbContext.Payments.AsNoTracking()
            .Where(payment => payment.OrderId == id)
            .OrderBy(payment => payment.Id)
            .Select(payment => new OrderPaymentAdminDto(
                payment.Amount ?? 0m,
                payment.Status,
                payment.PaidAt))
            .ToListAsync(cancellationToken);

        var shipping = await dbContext.ShippingInfos.AsNoTracking()
            .Where(info => info.OrderId == id)
            .OrderBy(info => info.Id)
            .Select(info => new OrderShippingAdminDto(
                info.Carrier,
                info.TrackingNumber,
                info.Status,
                info.EstimatedArrival))
            .ToListAsync(cancellationToken);

        return new OrderDetailAdminDto(
            order.Id,
            order.BuyerId,
            order.BuyerName,
            order.BuyerEmail,
            order.Status,
            order.TotalPrice ?? 0m,
            order.OrderDate,
            items,
            payments,
            shipping);
    }
}

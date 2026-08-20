using EbayClone.API.Data;
using EbayClone.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EbayClone.API.Repositories;

public class DashboardRepository(AppDbContext dbContext) : IDashboardRepository
{
    public Task<int> CountUsersAsync(CancellationToken cancellationToken = default) =>
        dbContext.Users.CountAsync(cancellationToken);

    public Task<int> CountProductsAsync(CancellationToken cancellationToken = default) =>
        dbContext.Products.CountAsync(cancellationToken);

    public Task<int> CountOrdersAsync(CancellationToken cancellationToken = default) =>
        dbContext.Orders.CountAsync(cancellationToken);

    public async Task<decimal> SumRevenueAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Payments
            .Where(payment => payment.Status == "Paid")
            .SumAsync(payment => (decimal?)payment.Amount, cancellationToken) ?? 0m;

    public Task<int> CountActiveUsersAsync(CancellationToken cancellationToken = default) =>
        dbContext.Users.CountAsync(user => user.Status == UserStatus.Active, cancellationToken);

    public Task<int> CountBannedUsersAsync(CancellationToken cancellationToken = default) =>
        dbContext.Users.CountAsync(user => user.Status == UserStatus.Banned, cancellationToken);

    public Task<int> CountHiddenProductsAsync(CancellationToken cancellationToken = default) =>
        dbContext.Products.CountAsync(product => product.Status == ProductStatus.Hidden, cancellationToken);

    public Task<int> CountPendingDisputesAsync(CancellationToken cancellationToken = default) =>
        dbContext.Disputes.CountAsync(dispute => dispute.Status == nameof(DisputeStatus.Open), cancellationToken);
}

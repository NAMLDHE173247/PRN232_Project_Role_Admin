using EbayClone.API.Data;
using EbayClone.API.DTOs.Disputes;
using EbayClone.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EbayClone.API.Repositories;

public class DisputeRepository(AppDbContext dbContext) : IDisputeRepository
{
    public async Task<(int Total, IReadOnlyList<DisputeDto> Items)> GetPageAsync(
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var disputes = dbContext.Disputes.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
            disputes = disputes.Where(dispute => dispute.Status == status.Trim());

        var total = await disputes.CountAsync(cancellationToken);
        var pageQuery = disputes
            .OrderByDescending(dispute => dispute.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        var items = await BuildDtoQuery(pageQuery)
            .ToListAsync(cancellationToken);
        return (total, items);
    }

    public Task<DisputeDto?> GetDtoByIdAsync(int id, CancellationToken cancellationToken = default) =>
        BuildDtoQuery(dbContext.Disputes.AsNoTracking().Where(dispute => dispute.Id == id))
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Dispute?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Disputes.FirstOrDefaultAsync(dispute => dispute.Id == id, cancellationToken);

    public Task<bool> IsAdminAsync(int userId, CancellationToken cancellationToken = default) =>
        dbContext.Users.AnyAsync(user => user.Id == userId && user.Role == "Admin", cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);

    private IQueryable<DisputeDto> BuildDtoQuery(IQueryable<Dispute> disputes)
    {
        return
            from dispute in disputes
            join raiser in dbContext.Users.AsNoTracking()
                on dispute.RaisedBy equals (int?)raiser.Id into raisers
            from raiser in raisers.DefaultIfEmpty()
            join admin in dbContext.Users.AsNoTracking()
                on dispute.AssignedTo equals (int?)admin.Id into admins
            from admin in admins.DefaultIfEmpty()
            select new DisputeDto(
                dispute.Id,
                dispute.OrderId,
                dispute.RaisedBy,
                raiser == null ? null : raiser.FullName,
                dispute.Description,
                dispute.Status,
                dispute.Resolution,
                dispute.AssignedTo,
                admin == null ? null : admin.FullName,
                dispute.AssignedAt,
                dispute.ResolvedBy,
                dispute.ResolvedAt);
    }
}

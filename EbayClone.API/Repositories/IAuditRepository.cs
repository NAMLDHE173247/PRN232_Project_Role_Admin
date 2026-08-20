using EbayClone.API.Models;

namespace EbayClone.API.Repositories;

public interface IAuditRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<(int Total, IReadOnlyList<AuditLog> Items)> GetPageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

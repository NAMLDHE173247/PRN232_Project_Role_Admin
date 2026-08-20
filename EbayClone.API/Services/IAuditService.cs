using EbayClone.API.DTOs.Audit;

namespace EbayClone.API.Services;

public interface IAuditService
{
    Task<PagedAuditResultDto> GetPageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

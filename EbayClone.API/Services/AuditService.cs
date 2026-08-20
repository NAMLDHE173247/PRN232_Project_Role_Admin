using EbayClone.API.DTOs.Audit;
using EbayClone.API.Repositories;

namespace EbayClone.API.Services;

public class AuditService(IAuditRepository auditRepository) : IAuditService
{
    public async Task<PagedAuditResultDto> GetPageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var result = await auditRepository.GetPageAsync(page, pageSize, cancellationToken);
        var items = result.Items.Select(log => new AuditLogDto(
            log.Id,
            log.ActorId,
            log.Action,
            log.Resource,
            log.ResourceId,
            log.Metadata,
            log.CreatedAtUtc)).ToList();
        return new PagedAuditResultDto(page, pageSize, result.Total, items);
    }
}

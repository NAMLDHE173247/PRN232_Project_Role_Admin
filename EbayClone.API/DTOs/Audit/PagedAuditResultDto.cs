namespace EbayClone.API.DTOs.Audit;

public record PagedAuditResultDto(
    int Page,
    int PageSize,
    int Total,
    IReadOnlyList<AuditLogDto> Items);
